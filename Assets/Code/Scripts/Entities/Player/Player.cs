using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    /*
     * Zmienna listowa przechowująca aktualne typy obrażeń
     */
    [Serializable]
    public class ElementalType
    {
        [SerializeField] public string name;
        public Sprite icon;
        public Color elementalColor;
    }

    [Header("Elemental Types")] public List<ElementalType> ElementalTypes = new List<ElementalType>();

    public int UsedElementalTypeId = 0;
    [ReadOnly] public String UsedElementalName = "Normal";

    /*
     * Zmienne dostępne w edytorze
     */
    [Header("Player variables")] public float jumpForce = 6f;

    public Animator animator;
    public int attackState = 0; // Aktualny stan ataku
    public bool isAttacking = false; // Czy trwa atak
    public bool isGrounded = false; // Czy dotyka ziemii

    /*
     * Zmienne lokalne
     */
    private Rigidbody2D playerBody;
    private PlayerInventory playerEq;
    private Collider2D ignoredObject;
    private CapsuleCollider2D boxCollider;
    private GameObject swordHitbox;
    private Vector3 previousPosition;
    private float attackTimeout = 1.0f; // Czas na zakończenie sekwencji ataku
    private float lastAttackTime = 0; // aktualny pomiędzy atakami
    private float attackCooldown = 0.3f; // cooldown ponmiędzy atakami
    private Coroutine attackCoroutine;
    private FloorDetector FloorDetector;
    private bool isChargingAttack = false;
    private float keyHoldTime = 0.0f;
    float holdTimeThreshold = 1.7f;
    private EntityStatus playerStatus;
    [SerializeField] private float parryWindow;
    public bool isBlocking;
    public bool isParrying;
    public float cooldownBetweenBlocks;
    private bool canBlock = true;
    private PauseMenuBehaviour pauseMenu;
    private Image elementalIconComponent;
    public Vector3 lastSafePosition;
    public float playerVoidLevel;
    private SoundManager _soundManager;
    
    [Header("Stair stuff")]
    public LayerMask stairLayer;
    public bool onStairs = false;
    public float stairAngle = 0f;
    public Vector2 stairMovementMultiplier = Vector2.one;

    private void Start()
    {
        // pobieranie rigidbody
        playerBody = GetComponent<Rigidbody2D>();
        playerEq = gameObject.GetComponent<PlayerInventory>();
        FloorDetector = transform.Find("FloorDetector").gameObject.GetComponent<FloorDetector>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        playerStatus = GetComponent<EntityStatus>();
        swordHitbox = transform.Find("SwordHitbox").gameObject;
        animator = GetComponentInChildren<Animator>();

        // pauseMenu = GameObject.Find("Pause Menu Interface").GetComponent<PauseMenuBehaviour>();

        // elementalIconComponent = GameObject.Find("UserInterface").transform.Find("Main User Interface").transform
        //     .Find("Elemental").transform.Find("ElementalIcon").gameObject.GetComponent<Image>();
        // if (elementalIconComponent == null)
        // {
        //     Debug.LogError("ElementalIconComponent nie został znaleziony w Start().");
        // }
    }

    private void Awake()
    {
        _soundManager = GetComponent<SoundManager>();
    }

    private void Update()
    {
        isGrounded = (boxCollider.GetContacts(new ContactPoint2D[16]) > 0); // && Mathf.Abs(playerBody.velocity.y) < 0.01f @Wojtek oby to nie bylo wazne

        /*
         * Zapisywanie bezpiecznej lokacji do skakania
         */
        // if (isGrounded)
        // {
        //     lastSafePosition = gameObject.transform.position;
        // }
        // else
        // {
        //     if (gameObject.transform.position.y <= playerVoidLevel)
        //     {
        //         lastSafePosition.y += 2.0f;
        //         playerBody.MovePosition(lastSafePosition);
        //         playerBody.velocity = Vector2.zero; // Reset prędkości
        //     }
        // }

        float horizontalInput = Input.GetAxis("Horizontal");

        /*
         * Przesyłanie odpowiednich zmiennych do animatora
         */
        animator.SetFloat("PlayerSpeed", Mathf.Abs(playerBody.velocity.x));
        // animator.SetFloat("PlayerVelocity", playerBody.velocity.y);
        // animator.SetInteger("PlayerAttackState", attackState);
        animator.SetBool("IsPlayerAttacking", isAttacking);
        // animator.SetBool("IsGrounded", isGrounded);
        // animator.SetBool("IsChargingAttack", isChargingAttack);
        // animator.SetFloat("ChargingTime", keyHoldTime);
        animator.SetBool("IsBlocking", isBlocking);

        /*
         * Blokowanie chodzenia do tyłu, gdy gracz atakuje, lub blokuje
         */
        // if (isAttacking &&
        //     ((horizontalInput < 0 && playerStatus.isFacedRight) ||
        //      (horizontalInput > 0 && !playerStatus.isFacedRight)))
        // {
        // else if (isBlocking) horizontalInput = 0;
        if (isAttacking)
        {
            horizontalInput = 0;
        }
        else if (isBlocking) horizontalInput = 0;

        /*
         * przemieszczanie w osi x, prędkość poruszania się zależna od tego czy gracz atakuje
         */
        DetectStairs();
        if (onStairs)
        {
            if (!isAttacking)
            {
                playerBody.velocity = new Vector2(-horizontalInput * playerStatus.GetMovementSpeed() * stairMovementMultiplier.x,-horizontalInput * playerStatus.GetMovementSpeed() * stairMovementMultiplier.y);
            }
            else
            {
                playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed() * 0.6f,
                    playerBody.velocity.y);
            }
        }
        else
        {
            if (!isAttacking)
            {
                playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed(), playerBody.velocity.y);
            }
            else
            {
                playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed() * 0.6f,
                    playerBody.velocity.y);
            }
        }

        /*
         * Nieznaczne wydłużanie hitboxa ataków podczas biegu
         */
        // if ((Mathf.Abs(playerBody.velocity.x) >= playerStatus.GetMovementSpeed()) ||
        //     (horizontalInput != 0 && isChargingAttack))
        // {
        //     BoxCollider2D swordCollider = swordHitbox.GetComponent<BoxCollider2D>();
        //     swordCollider.size = new Vector2(playerStatus.attackRange * 1.5f, 0.3f);
        //     swordCollider.offset = new Vector2(playerStatus.attackRange, 0f);
        // }
        // else
        // {
        //     BoxCollider2D swordCollider = swordHitbox.GetComponent<BoxCollider2D>();
        //     swordCollider.size = new Vector2(playerStatus.attackRange, 0.3f);
        //     swordCollider.offset = new Vector2(playerStatus.attackRange / 2, 0f);
        // }

        /*
         * skakanie
         */
        if ((Input.GetKeyDown(InputManager.JumpKey) || Input.GetKeyDown(InputManager.PadButtonJump)) &&
            !isAttacking && !isBlocking && isGrounded)
        {
            _soundManager.PlaySound(0);
            Jump();
        }

        /*
         * Zmiana kierunku gracza
         */
        if (Input.GetKey(InputManager.MoveLeftKey) && playerStatus.isFacedRight && (Time.timeScale != 0) &&
            !isAttacking && !isBlocking)
        {
            playerStatus.isFacedRight = false;
            transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        if (Input.GetKey(InputManager.MoveRightKey) && !playerStatus.isFacedRight && (Time.timeScale != 0) &&
            !isAttacking && !isBlocking)
        {
            playerStatus.isFacedRight = true;
            transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        /*
         * Atak, oraz charge attack
         */
        if (Input.GetKey(InputManager.AttackKey) && !isAttacking && !isBlocking)
        {
            if (keyHoldTime < holdTimeThreshold)
            {
                keyHoldTime += Time.deltaTime;
                isChargingAttack = true;
            }
        }

        if (isChargingAttack)
        {
            horizontalInput = 0;
        }


        if (Input.GetKeyUp(InputManager.AttackKey))
        {
            if (isChargingAttack)
            {
                if (keyHoldTime >= holdTimeThreshold)
                {
                    PerformChargeAttack();
                }
                else
                {
                    // Rozpocznij atak od początku sekwencji
                    StartAttack();
                }
            }
            else if (isGrounded && isAttacking)
            {
                // Gracz kontynuuje sekwencję ataku
                ContinueAttack();
            }

            isChargingAttack = false;
            keyHoldTime = 0.0f;
        }

        if (canBlock)
        {
            /*
             * Parowanie
             */
            if (!isAttacking && !isChargingAttack && Input.GetKeyDown(InputManager.BlockKey))
            {
                isParrying = true;
                StartCoroutine(Parry());
            }
        
            /*
             * Blokowanie
             */
            if (Input.GetKey(InputManager.BlockKey))
            {
                isBlocking = true;
                canBlock = false;
                StartCoroutine(EnableBlockingAfterDuration(cooldownBetweenBlocks));
            }
            else isBlocking = false;
        
            /*
             * Przełączanie animacji blokowania
             */
            if (Input.GetKeyDown(InputManager.BlockKey)) animator.Play("blockAttack");
        }

        /*
         * Przejście przez podłoże
         */
        if (FloorDetector.isFloorPassable && isGrounded &&
            Input.GetKeyDown(InputManager.MoveDownKey))
        {
            DisableCollisionForDuration(0.3f);
        }
    }

    private IEnumerator EnableBlockingAfterDuration(float duration)
    {
        // małe okno pomiędzy parowaniami
        yield return new WaitForSeconds(duration);
        canBlock = true;
    }
    
    void DetectStairs()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, stairLayer);
        
        if (hit)
        {
            onStairs = true;
            Vector2 slopeNormalPerp = Vector2.Perpendicular(hit.normal);
            stairMovementMultiplier = slopeNormalPerp.normalized;
            stairAngle = Vector2.Angle(hit.normal, Vector2.up);
            
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
        }
        else
        {
            onStairs = false;
            stairMovementMultiplier = Vector2.one;
        }
    }

    private void Jump()
    {
        Vector2 jumpVector = new Vector2(0, jumpForce * 10);
        float playerBodyVelocity = playerBody.GetPointVelocity(jumpVector).y;

        if (playerBodyVelocity < 0.01f) // Sprawdzamy, czy postać jest na ziemi
        {
            playerBody.AddForce(Vector2.up * jumpForce * 10, ForceMode2D.Impulse);
        }
    }

    private IEnumerator Parry()
    {
        yield return new WaitForSeconds(parryWindow);
        isParrying = false;
    }

    private void PerformChargeAttack()
    {
        if (isChargingAttack && (keyHoldTime >= holdTimeThreshold))
        {
            float howFar = 14.0f;
            float diection = (playerStatus.isFacedRight) ? 1 : -1;

            Vector3 movement = new Vector3(1.0f * howFar * 1000 * diection, 100.0f, 0);
            playerBody.AddForce(movement);

            DealDamage(playerStatus.GetAttackDamageCount() * 4);
            animator.Play("heavyAttack_2");
        }
    }

    private void DisableCollisionForDuration(float duration)
    {
        // if (!playerEq.isPickingItem)
        // {
            ignoredObject = FloorDetector.collidingObject.GetComponent<Collider2D>();

            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), ignoredObject, true);
            Invoke("EnableCollision", duration);
        // }
    }

    // Włączenie kolizji ponownie.
    private void EnableCollision()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), ignoredObject, false);
        ignoredObject = null;
    }

    private void DealDamage(float damageToDeal)
    {
        // sprawdzanie czy gracz atakuje przeciwnika
        List<GameObject> collidingObjects = gameObject.GetComponent<EntityStatus>().detectedTargets;
        
        if (collidingObjects.Count > 0)
        {
            foreach (var entity in collidingObjects)
            {
                // zadawanie obrażeń
                entity.GetComponent<EntityStatus>().DealDamage(damageToDeal);
                // swordHitbox.gameObject.GetComponent<ParticleSystem>().Play();
            }
        }
    }


    private void StartAttack()
    {
        attackCoroutine = StartCoroutine(AttackTimeout());
        isAttacking = true;
        attackState = 1;
        movePlayerOnAttack(3.0f);
        animator.Play("Attack_1");
        DealDamage(playerStatus.GetAttackDamageCount());
    }

    private void ContinueAttack()
    {

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(AttackTimeout());

        // Sprawdź, czy minęło wystarczająco dużo czasu między atakami
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            movePlayerOnAttack(3.0f);
            if (attackState == 4)
            {
                // Gracz zaczyna nową sekwencję ataku
                attackState = 1;
                animator.Play("Attack_1");
                DealDamage(playerStatus.GetAttackDamageCount());
            }
            else
            {
                // Kontynuuj sekwencję ataku
                attackState++;

                movePlayerOnAttack(3.0f);

                if (attackState != 0) animator.Play("Attack_" + attackState.ToString());

                DealDamage(playerStatus.GetAttackDamageCount());
            }

            // Aktualizuj czas ostatniego ataku
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator AttackTimeout()
    {
        yield return new WaitForSeconds(attackTimeout);

        // Przerwanie ataku po timeout
        isAttacking = false;
        attackState = 0;
    }

    private void movePlayerOnAttack(float howFar)
    {
        // delikatne przesunięcie gracza po ataku
        if (playerStatus.isFacedRight)
        {
            Vector3 movement = new Vector3(1.0f * howFar * 1000, 0, 0);
            playerBody.AddForce(movement);
        }
        else
        {
            Vector3 movement = new Vector3(-1.0f * howFar * 1000, 0, 0);
            playerBody.AddForce(movement);
        }
    }

    public void ChangeElementalType(int TypeId)
    {
        if (TypeId >= 0 && TypeId < ElementalTypes.Count)
        {
            String elementalName = ElementalTypes[TypeId].name;
            UsedElementalTypeId = TypeId;
            UsedElementalName = elementalName;

            if (elementalIconComponent == null)
            {
                Debug.LogWarning("ElementalIconComponent jest null, próbuję przypisać go ponownie.");
                elementalIconComponent = GameObject.Find("UserInterface").transform.Find("Main User Interface")
                    .transform.Find("Elemental").transform.Find("ElementalIcon").gameObject.GetComponent<Image>();

                if (elementalIconComponent == null)
                {
                    Debug.LogError("Nie udało się przypisać elementalIconComponent w ChangeElementalType.");
                    return;
                }
            }

            elementalIconComponent.sprite = ElementalTypes[TypeId].icon;
        }
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private SerializedProperty selectedElementalTypeProp;

    private void OnEnable()
    {
        selectedElementalTypeProp = serializedObject.FindProperty("UsedElementalTypeId");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Sprawdzenie, czy edytowany obiekt jest jednym obiektem
        if (serializedObject.isEditingMultipleObjects)
        {
            EditorGUILayout.HelpBox("Multi-object editing not supported", MessageType.Error);
            return;
        }

        // Wyświetlanie listy rozwijanej z numerami od 0 do 5
        selectedElementalTypeProp.intValue = EditorGUILayout.Popup("Choose elemental type",
            selectedElementalTypeProp.intValue, new string[] { "0", "1", "2", "3", "4", "5" });

        // Przycisk do zmiany rodzaju elementu
        if (GUILayout.Button("Change elemental"))
        {
            Player script = (Player)target;
            script.ChangeElementalType(selectedElementalTypeProp.intValue);
        }

        serializedObject.ApplyModifiedProperties();

        // Reszta standardowego interfejsu
        DrawDefaultInspector();
    }
}
#endif
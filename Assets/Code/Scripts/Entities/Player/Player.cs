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
    [Header("Player variables")]
    public float jumpForce = 6f;
    public Animator animator;
    public int attackState = 0; // Aktualny stan ataku
    public bool isAttacking = false; // Czy trwa atak
    public bool isGrounded = false; // Czy dotyka ziemii
    public bool isBlocking;
    public bool isParrying;
    public float cooldownBetweenBlocks;
    public float dashDistance = 5f; // Maksymalny dystans dasha
    public float dashCooldown = 1f; // Czas odnowienia dasha
    public LayerMask obstacleLayer; // Warstwa przeszkód
    public float playerVoidLevel; // Próg, poniżej którego dash nie działa


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
    private bool canBlock = true;
    private PauseMenuBehaviour pauseMenu;
    private Image elementalIconComponent;
    private Vector3 lastSafePosition;
    private SoundManager _soundManager;
    private float _lastDashTime;
    
    [Header("Stair stuff")]
    public LayerMask stairLayer;
    public bool onStairs = false;
    public float stairAngle = 0f;
    public Vector2 stairMovementMultiplier = Vector2.one;
    public bool canJump = true;
    public bool isJumping = false;

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

    public void PlayPlayerSFXSingle(AudioClip audioClip)
    {
        if (WorldSoundFXManager.instance == null) return;
        float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
        WorldSoundFXManager.instance.PlaySoundFX(audioClip, AudioListener.volume, randomPitch);
    }
    public void PlayPlayerSFXArray(AudioClip[] audioArray)
    {
        if (WorldSoundFXManager.instance == null) return;
        float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(audioArray, AudioListener.volume, randomPitch);
    }

    private void Update()
    {
        isGrounded = (boxCollider.GetContacts(new ContactPoint2D[16]) > 0); // && Mathf.Abs(playerBody.velocity.y) < 0.01f; //@Wojtek oby to nie bylo wazne

        if (playerBody.velocity.y <= 0.0f)
        {
            isJumping = false;
        }
        if (isGrounded && !isJumping)
        {
            canJump = true;
        }
        
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
        animator.SetFloat("PlayerVelocity", playerBody.velocity.y);
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
        
        
        if (isGrounded && onStairs && !isJumping)
        {
            playerBody.velocity = new Vector2(-horizontalInput * playerStatus.GetMovementSpeed() * stairMovementMultiplier.x,
                                                -horizontalInput * playerStatus.GetMovementSpeed() * stairMovementMultiplier.y);
        }
        else if(!onStairs)
        {
            playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed(), playerBody.velocity.y);
        }

        /*
         * if (!isAttacking)
            {
                playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed(), playerBody.velocity.y);
            }
            else
            {
                playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed() * 0.6f,
                    playerBody.velocity.y);
            }
         */
        
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
            //_soundManager.PlaySound(0);
            if (WorldSoundFXManager.instance == null) return;
            float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.playerJumpSFX, AudioListener.volume, randomPitch);
            Jump();
        }

        /*
         * Zmiana kierunku gracza
         */
        if ((Input.GetKey(InputManager.MoveLeftKey) || Input.GetAxis("Horizontal") < 0) && playerStatus.isFacedRight && (Time.timeScale != 0) &&
            !isAttacking && !isBlocking)
        {
            playerStatus.isFacedRight = false;
            transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        if ((Input.GetKey(InputManager.MoveRightKey) || Input.GetAxis("Horizontal") > 0) && !playerStatus.isFacedRight && (Time.timeScale != 0) &&
            !isAttacking && !isBlocking)
        {
            playerStatus.isFacedRight = true;
            transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        /*
         * Atak, oraz charge attack
         */
        if ((Input.GetKey(InputManager.AttackKey) || Input.GetKey(InputManager.PadButtonAttack)) && !isAttacking && !isBlocking && (Time.timeScale > 0))
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


        if (Input.GetKeyUp(InputManager.AttackKey) || Input.GetKeyUp(InputManager.PadButtonAttack))
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
            if (!isAttacking && !isChargingAttack && ( Input.GetKeyDown(InputManager.BlockKey) || Input.GetKeyDown(InputManager.PadButtonBlock)))
            {
                isParrying = true;
                StartCoroutine(Parry());
            }
        
            /*
             * Blokowanie
             */
            if (Input.GetKey(InputManager.BlockKey) || Input.GetKey(InputManager.PadButtonBlock))
            {
                isBlocking = true;
                canBlock = false;
                StartCoroutine(EnableBlockingAfterDuration(cooldownBetweenBlocks));
            }
            else isBlocking = false;
        
            /*
             * Przełączanie animacji blokowania
             */
            if (Input.GetKeyDown(InputManager.BlockKey) || Input.GetKeyDown(InputManager.PadButtonBlock)) animator.Play("blockAttack");
        }

        /*
         * Przejście przez podłoże
         */
        if (FloorDetector.isFloorPassable && isGrounded &&
            (Input.GetKeyDown(InputManager.MoveDownKey) ||
             Input.GetAxis("Vertical") < 0 ) )
        {
            DisableCollisionForDuration(0.3f);
        }

        /*
         * Dash
         */
        if (Time.time >= _lastDashTime + dashCooldown)
        {
            if (Input.GetKeyDown(InputManager.DodgeKey) || Input.GetKeyDown(InputManager.PadButtonInteract))
            {
                Dash();
            }
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
        // Array to store contact points
        ContactPoint2D[] contacts = new ContactPoint2D[16];
        // Get the number of contact points
        int contactCount = boxCollider.GetContacts(contacts);

        // Reset stair detection
        onStairs = false;
        stairMovementMultiplier = Vector2.one;

        // Iterate through all contact points
        for (int i = 0; i < contactCount; i++)
        {
            // Check if the contact point is on the "Stairs" layer
            if (stairLayer == (stairLayer | (1 << contacts[i].collider.gameObject.layer)))
            {
                // The player is on stairs
                onStairs = true;

                // Calculate the slope's perpendicular direction for movement
                Vector2 slopeNormalPerp = Vector2.Perpendicular(contacts[i].normal).normalized;
                stairMovementMultiplier = slopeNormalPerp;

                // Calculate the angle of the slope (optional, for debugging or advanced behavior)
                stairAngle = Vector2.Angle(contacts[i].normal, Vector2.up);

                // Debug visualization
                Debug.DrawRay(contacts[i].point, contacts[i].normal, Color.green); // Draw contact normal
                Debug.DrawRay(contacts[i].point, slopeNormalPerp, Color.red); // Draw slope perpendicular direction

                break; // Exit the loop once stairs are detected
            }
        }
    }

    private void Jump()
    {
        if (canJump)
        {
            canJump = false;
            isJumping = true;
            if (onStairs)
            {
                onStairs = false; // Temporarily disable stairs effect
            }
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

    // STARA WERSJA JEŚLI NOWA BY SIĘ BUGOWAŁA

    //private void DisableCollisionForDuration(float duration)
    //{
    //    // if (!playerEq.isPickingItem)
    //    // {
    //    ignoredObject = FloorDetector.collidingObject.GetComponent<Collider2D>();

    //    Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), ignoredObject, true);
    //    Invoke("EnableCollision", duration);
    //    // }
    //}

    //// Włączenie kolizji ponownie.
    //private void EnableCollision()
    //{
    //    Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), ignoredObject, false);
    //    ignoredObject = null;
    //}

    // NOWA WERSJA WYŁĄCZANIA COLLIDERÓW, PRZEZ KTÓRE MOŻNA SPAŚĆ
    private void DisableCollisionForDuration(float duration)
    {
        if (FloorDetector.collidingObject == null)
            return;

        Collider2D colliderToIgnore = FloorDetector.collidingObject.GetComponent<Collider2D>();
        if (colliderToIgnore == null)
            return;

        Collider2D playerCollider = gameObject.GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            Debug.LogWarning("Dodaj Collider2D do gracza!");
            return;
        }

        Physics2D.IgnoreCollision(playerCollider, colliderToIgnore, true);

        StartCoroutine(EnableCollisionAfterDelay(playerCollider, colliderToIgnore, duration));
    }

    private IEnumerator EnableCollisionAfterDelay(Collider2D playerCollider, Collider2D colliderToEnable, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (playerCollider != null && colliderToEnable != null)
        {
            Physics2D.IgnoreCollision(playerCollider, colliderToEnable, false);
        }
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
        PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackSFX);
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

                PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackSFX);

                DealDamage(playerStatus.GetAttackDamageCount());
            }
            else
            {
                // Kontynuuj sekwencję ataku
                attackState++;

                movePlayerOnAttack(3.0f);

                if (attackState != 0) animator.Play("Attack_" + attackState.ToString());

                PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackSFX);

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

    private void Dash()
    {
        Vector2 dashDirection = GetDashDirection();
        Vector2 startPosition = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(startPosition, dashDirection, dashDistance, obstacleLayer);
        Vector2 targetPosition = (hit.collider != null) ? hit.point - dashDirection * 0.1f : startPosition + (dashDirection * dashDistance);

        transform.position = targetPosition;
        _lastDashTime = Time.time;
    }

    private Vector2 GetDashDirection()
    {
        Vector2 direction = Vector2.zero;

        // Pad
        Vector2 gamepadInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (gamepadInput.magnitude > 0.1f)
        {
            direction = gamepadInput.normalized;
        }
        else
        {
            // Klawiatura
            bool up = Input.GetKey(KeyCode.W);
            bool down = Input.GetKey(KeyCode.S);
            bool left = Input.GetKey(KeyCode.A);
            bool right = Input.GetKey(KeyCode.D);

            if (up && right) direction = new Vector2(1, 1).normalized;
            else if (up && left) direction = new Vector2(-1, 1).normalized;
            else if (down && right) direction = new Vector2(1, -1).normalized;
            else if (down && left) direction = new Vector2(-1, -1).normalized;
            else if (up) direction = Vector2.up;
            else if (down) direction = Vector2.down;
            else if (left) direction = Vector2.left;
            else if (right) direction = Vector2.right;
        }

        return direction;
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
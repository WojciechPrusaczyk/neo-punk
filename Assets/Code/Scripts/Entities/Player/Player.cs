using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        public Enums.ElementalType elementalType;
        public int elementalAttacksSequences;
    }

    [Header("Elemental Types")] public List<ElementalType> ElementalTypes = new List<ElementalType>();

    public int UsedElementalTypeId = 0;
    [ReadOnly] public String UsedElementalName = "Normal";
    [ReadOnly] public Enums.ElementalType UsedElemental = Enums.ElementalType.Normal;
    [ReadOnly] public int usedElementalSequences = 4;

    /*
     * Zmienne dostępne w edytorze
     */
    [Header("Player variables")]
    public float jumpForce = 6f;
    public Animator animator;
    public int attackState = 0; // Aktualny stan ataku
    public bool isAttacking = false; // Czy trwa atak
    public bool canWalkAfterAttack = false; // Czy właśnie jest animacja ataku
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
    [HideInInspector] public EntityStatus playerStatus;

    public bool isInDialogue = false;

    private Rigidbody2D playerBody;
    private PlayerInventoryInterface playerEq;
    private Collider2D ignoredObject;
    private CapsuleCollider2D boxCollider;
    private GameObject swordHitbox;
    private Vector3 previousPosition;
    private float attackTimeout = 1.0f; // Czas na zakończenie sekwencji ataku
    //private float attackBreakTimeout = 0.1f; // Czas trwania animacji po natychmiastowy przerwaniu chodzeniem
    private float lastAttackTime = 0; // aktualny pomiędzy atakami
    private float attackCooldown = 0.3f; // cooldown ponmiędzy atakami
    private Coroutine attackCoroutine;
    private FloorDetector FloorDetector;
    private bool isChargingAttack = false;
    private float keyHoldTime = 0.0f;
    float holdTimeThreshold = 1.7f;
    [SerializeField] private float parryWindow;
    private bool canBlock = true;
    private PauseMenuBehaviour pauseMenu;
    private Image elementalIconComponent;
    private Vector3 lastSafePosition;
    private SoundManager _soundManager;
    private float _lastDashTime;
    private bool wasDamagedRecently = false;
    private MainUserInterfaceController mainUserInterfaceController;
    private PlayerInventoryInterface playerInventoryInterface;
    private HitboxBehaviour footHitbox;
    private HitboxBehaviour swordHitboxComponent;
    private List<GameObject> collidingObjects = new List<GameObject>();
    
    [Header("Stair stuff")]
    public LayerMask stairLayer;
    public bool onStairs = false;
    public float stairAngle = 0f;
    public Vector2 stairMovementMultiplier = Vector2.one;
    public bool canJump = true;
    public bool isJumping = false;
    public bool isJumpAttacking = false;
    
    [Header("Wall jump stuff")]
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public float wallSlideSpeed = 1.5f;
    public float wallJumpHorizontalForce = 4f;
    public float wallJumpVerticalForce = 6f;
    public float wallCheckDistance = 0.1f;
    public float groundCheckDistance = 0.1f;
    
    public Vector2 wallCheckOffset = new Vector2(0f, 0.1f);


    public bool isTouchingWall = false;
    public bool isWallSliding = false;
    public int wallDirection = 0;
    private bool wallJumpLock;
    [SerializeField] private float wallJumpLockTime;
    [SerializeField] private float minWallUnstickSpeed;
    
    public bool externalForce = false;
    
    private void Awake()
    {
        // pobieranie rigidbody
        playerBody = GetComponent<Rigidbody2D>();
        playerEq = gameObject.GetComponent<PlayerInventoryInterface>();
        FloorDetector = transform.Find("FloorDetector").gameObject.GetComponent<FloorDetector>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        playerStatus = GetComponent<EntityStatus>();
        swordHitbox = transform.Find("SwordHitbox").gameObject;
        swordHitboxComponent = transform.Find("SwordHitbox").GetComponent<HitboxBehaviour>();
        animator = GetComponentInChildren<Animator>();
        footHitbox = transform.Find("FloorDetector").gameObject.GetComponent<HitboxBehaviour>();

        var mainUserInterfaceRoot = GameObject.Find("MainUserInterfaceRoot");

        mainUserInterfaceController = mainUserInterfaceRoot.GetComponentInChildren<MainUserInterfaceController>();
        playerInventoryInterface = mainUserInterfaceRoot.GetComponentInChildren<PlayerInventoryInterface>();

        // pauseMenu = GameObject.Find("Pause Menu Interface").GetComponent<PauseMenuBehaviour>();

        // elementalIconComponent = GameObject.Find("UserInterface").transform.Find("Main User Interface").transform
        //     .Find("Elemental").transform.Find("ElementalIcon").gameObject.GetComponent<Image>();
        // if (elementalIconComponent == null)
        // {
        //     Debug.LogError("ElementalIconComponent nie został znaleziony w Start().");
        // }
        _soundManager = GetComponent<SoundManager>();
        if (playerStatus != null)
        {
            playerStatus.OnPlayerDamageTaken += OnPlayerDamaged;
            playerStatus.OnPlayerDeath += OnPlayerDeath;
        }
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    { 
        // Zapisanie danych postaci do pliku
        currentCharacterData.currentHealth = playerStatus.GetHp();

        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.characterName = playerStatus.entityName;
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        // Załadowanie danych postaci z pliku
        playerStatus.SetHp(currentCharacterData.currentHealth);
        Vector3 newPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition,
            currentCharacterData.zPosition);
        transform.position = newPosition;
        playerStatus.entityName = currentCharacterData.characterName;
    }

    public void PlayPlayerSFXSingle(AudioClip audioClip, Enums.SoundType soundType, float pitchMultiplier = 1f)
    {
        if (WorldSoundFXManager.instance == null)
            return;

        if (WorldSoundFXManager.instance.gameState == Enums.GameState.Paused)
            return;

        float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
        WorldSoundFXManager.instance.PlaySoundFX(audioClip, soundType, randomPitch * pitchMultiplier);
    }
    public void PlayPlayerSFXArray(AudioClip[] audioArray, Enums.SoundType soundType, float pitchMultiplier = 1f)
    {
        if (WorldSoundFXManager.instance == null)
            return;

        if (WorldSoundFXManager.instance.gameState == Enums.GameState.Paused)
            return;

        float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(audioArray, soundType, randomPitch * pitchMultiplier);
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (playerStatus != null && playerStatus.isDead)
            return;
        
        Vector2 groundOrigin = new Vector2(
            boxCollider.bounds.center.x,
            boxCollider.bounds.center.y - boxCollider.bounds.extents.y
        );

        RaycastHit2D groundHit = Physics2D.Raycast(
            groundOrigin,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        isGrounded = (groundHit.collider != null);

        Debug.DrawRay(groundOrigin, Vector2.down * groundCheckDistance, Color.green);

        // Tworzenie listy możliwych obiektów do ataku
        collidingObjects = gameObject.GetComponent<EntityStatus>().detectedTargets;
        
        if (playerBody.velocity.y <= 0.1f)
        {
            isJumping = false;
        }
        if ((onStairs || isGrounded) && !isJumping)
        {
            canJump = true;
        }

        /*
         * Logika animacji ataku z wyskoku
         */
        // Jeśli można użyć ataku z wyskoku
        if (!isGrounded && UsedElemental == Enums.ElementalType.Storm)
        {
            // Jeśli gracz go ztriggeruje
            if ((Input.GetKeyDown(InputManager.AttackKey) || Input.GetKeyDown(InputManager.PadButtonAttack)) && !isJumpAttacking)
            {
                isJumpAttacking = true;
            }
        }
        else isJumpAttacking = false;

        footHitbox.enabled = isJumpAttacking;
        swordHitboxComponent.enabled = !isJumpAttacking;

        /*
         * Logika ataku z wyskoku
         */
        if (isJumpAttacking)
        {
            foreach (var entity in collidingObjects)
            {
                var enemyType = entity.GetComponent<EntityStatus>().entityType;

                entity.GetComponent<EntityStatus>().DealDamage(playerStatus.GetAttackDamageCount() * 1.5f);
                // swordHitbox.gameObject.GetComponent<ParticleSystem>().Play();
                collidingObjects.Remove(entity);

                if (WorldSoundFXManager.instance)
                {
                    if (enemyType == Enums.EntityType.Cyber)
                        PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackMetalSFX, Enums.SoundType.SFX, 2f);
                    else
                        PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackFleshSFX, Enums.SoundType.SFX, 2f);
                }
            }
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

        /*
         * Przesyłanie odpowiednich zmiennych do animatora
         */
        animator.SetFloat("PlayerSpeed", Mathf.Abs(playerBody.velocity.x));
        animator.SetFloat("PlayerVelocity", playerBody.velocity.y);
        // animator.SetInteger("PlayerAttackState", attackState);
        animator.SetBool("IsPlayerAttacking", isAttacking);
        animator.SetBool("IsOnStairs", onStairs);
        animator.SetBool("IsJumpAttacking", isJumpAttacking);
        // animator.SetBool("IsGrounded", isGrounded);
        // animator.SetBool("IsChargingAttack", isChargingAttack);
        // animator.SetFloat("ChargingTime", keyHoldTime);
        animator.SetBool("IsBlocking", isBlocking);
        animator.SetBool("isSliding", isWallSliding);

        /*
         * Blokowanie chodzenia do tyłu, gdy gracz atakuje, lub blokuje
         */
        // if (isAttacking &&
        //     ((horizontalInput < 0 && playerStatus.isFacedRight) ||
        //      (horizontalInput > 0 && !playerStatus.isFacedRight)))
        // {
        // else if (isBlocking) horizontalInput = 0;

        if (canWalkAfterAttack)
            horizontalInput = 0;

        if (isBlocking)
            horizontalInput = 0;

        /*
         * przemieszczanie w osi x, prędkość poruszania się zależna od tego czy gracz atakuje
         */
        
        DetectStairs();
        
        if (onStairs && !isJumping && !wallJumpLock && !externalForce)
        {
            playerBody.velocity = new Vector2(
                -horizontalInput * playerStatus.GetMovementSpeed() * stairMovementMultiplier.x,
                -horizontalInput * playerStatus.GetMovementSpeed() * stairMovementMultiplier.y
            );
        }
        else if (!onStairs && !wallJumpLock && !externalForce)
        {
            playerBody.velocity = new Vector2(
                horizontalInput * playerStatus.GetMovementSpeed(),
                playerBody.velocity.y
            );
        }

        bool isRunning = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
        if (isAttacking && playerStatus.detectedTargets.Count <= 0 && !wallJumpLock && !isRunning)
        {
            playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed() * 0.01f, playerBody.velocity.y);
        }
        // else
        // {
        //     playerBody.velocity = new Vector2(horizontalInput * playerStatus.GetMovementSpeed() * 0.6f,
        //         playerBody.velocity.y);
        // }
        
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
        CheckForWall();

        if (!wallJumpLock && !isGrounded && isTouchingWall)
        {
            isWallSliding = true;
            playerBody.velocity = new Vector2(playerBody.velocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }

        bool jumpPressed = Input.GetKeyDown(InputManager.JumpKey) ||
                           Input.GetKeyDown(InputManager.PadButtonJump);

        if (jumpPressed && !isAttacking && !isBlocking)
        {
            if ((isGrounded || onStairs) && !isJumping)
            {
                Jump();
            }
            else if (isWallSliding)
            {
                WallJump();
            }
        }

        if (isGrounded)
        {
            wallJumpLock = false;
        }

        /*
         * Zmiana kierunku gracza
         */
        if (!wallJumpLock)
        {
            if ((Input.GetKey(InputManager.MoveLeftKey) || Input.GetAxis("Horizontal") < 0) &&
                playerStatus.isFacedRight &&
                (Time.timeScale != 0) &&
                !isBlocking)
            {
                playerStatus.isFacedRight = false;
                transform.Rotate(new Vector3(0f, 180f, 0f));
            }

            if ((Input.GetKey(InputManager.MoveRightKey) || Input.GetAxis("Horizontal") > 0) &&
                !playerStatus.isFacedRight &&
                (Time.timeScale != 0) &&
                !isBlocking )
            {
                playerStatus.isFacedRight = true;
                transform.Rotate(new Vector3(0f, 180f, 0f));
            }
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


        if ((Input.GetKeyUp(InputManager.AttackKey) || Input.GetKeyUp(InputManager.PadButtonAttack)) && !isJumpAttacking)
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
            else if ((isGrounded || onStairs) && isAttacking)
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
                if (WorldSoundFXManager.instance != null)
                    if (!isBlocking)
                        PlayPlayerSFXArray(WorldSoundFXManager.instance.playerBlockSFX, Enums.SoundType.SFX, 2f);

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
            DisableCollisionForDuration(0.4f);
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

        /*
         * Sygnalizacja otrzymania obrażeń
         */
        if (wasDamagedRecently)
        {
            wasDamagedRecently = false;
            FindFirstObjectByType<CameraShaker>()?.DoScreenShake();
        }

        var selectedElemental = ElementalTypes[UsedElementalTypeId];
        if (mainUserInterfaceController != null && playerInventoryInterface != null)
        {
            mainUserInterfaceController.ChangeElementalType(selectedElemental.icon, selectedElemental.name, selectedElemental.elementalColor);
            playerInventoryInterface.ChangeElementalType(selectedElemental.icon, selectedElemental.name, selectedElemental.elementalColor);
        } else
            Debug.LogError("Nie znaleziono elementów interfejsu.");
    }

    private IEnumerator EnableBlockingAfterDuration(float duration)
    {
        // małe okno pomiędzy parowaniami
        yield return new WaitForSeconds(duration);
        canBlock = true;
    }

    public void TeleportPlayerToDrone(int droneID, string droneName)
    {
        if (WorldObjectManager.instance == null)
        {
            Debug.LogError("WorldObjectManager is not initialized.");
            return;
        }
        InteractableDrone drone = WorldObjectManager.instance.GetDroneByID(droneID);
        if (drone == null)
        {
            Debug.LogError($"Drone with ID {droneID} not found.");
            return;
        }
        transform.position = new Vector3(drone.transform.position.x, drone.transform.position.y + 1.0f, transform.position.z);

        playerBody.velocity = Vector2.zero;

        if (WorldSaveGameManager.instance != null)
        {
            WorldSaveGameManager.instance.currentCharacterData.lastVisitedDrone = new DroneIdentifier
            {
                sceneName = drone.gameObject.scene.name,
                droneID = drone.ID
            };
            WorldSaveGameManager.instance.currentCharacterData.lastVisitedDroneName = string.IsNullOrEmpty(droneName) ? "Unknown" : droneName;
            WorldSaveGameManager.instance.SaveGame();
        }

        if (WorldSoundFXManager.instance != null)
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.droneTeleport, Enums.SoundType.SFX);
        
        UserInterfaceController.instance.ActivateInterface(0);
    }
    
    
    void DetectStairs()
    {
        ContactPoint2D[] contacts = new ContactPoint2D[16];
        int contactCount = boxCollider.GetContacts(contacts);

        onStairs = false;
        stairMovementMultiplier = Vector2.one;

        for (int i = 0; i < contactCount; i++)
        {
            if (stairLayer == (stairLayer | (1 << contacts[i].collider.gameObject.layer)))
            {
                onStairs = true;

                Vector2 slopeNormalPerp = Vector2.Perpendicular(contacts[i].normal).normalized;
                stairMovementMultiplier = slopeNormalPerp;
                stairAngle = Vector2.Angle(contacts[i].normal, Vector2.up);

                Debug.DrawRay(contacts[i].point, contacts[i].normal, Color.green);
                Debug.DrawRay(contacts[i].point, slopeNormalPerp, Color.red);
                break;
            }
        }
    }

    private void Jump()
    {
        if (isInDialogue)
            return;
        if (UserInterfaceController.instance.isUIMenuActive)
            return;

        if (canJump)
        {
            canJump = false;
            isJumping = true;
            if (onStairs)
            {
                onStairs = false;
            }
            playerBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (WorldSoundFXManager.instance == null) return;
            float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.playerJumpSFX, Enums.SoundType.SFX, randomPitch);
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
        // Z tego co widzę, to inventory nie jest nigdzie podpięte, więc narazie to zostaje zakomentowane.
        // Po dodaniu inventory do gracza, można to odkomentować.
        //if (playerEq.isPickingItem)
        //    return;

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
        if (collidingObjects.Count > 0)
        {
            foreach (var entity in collidingObjects)
            {
                // zadawanie obrażeń

                var enemyType = entity.GetComponent<EntityStatus>().entityType;
                
                entity.GetComponent<EntityStatus>().DealDamage(damageToDeal);
                // swordHitbox.gameObject.GetComponent<ParticleSystem>().Play();

                if (WorldSoundFXManager.instance)
                {
                    if (enemyType == Enums.EntityType.Cyber)
                        PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackMetalSFX, Enums.SoundType.SFX, 2f);
                    else
                        PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackFleshSFX, Enums.SoundType.SFX, 2f);
                }
            }
        }
    }


    private void StartAttack()
    {
        if (UserInterfaceController.instance.isUIMenuActive) return;
        attackCoroutine = StartCoroutine(AttackTimeout());
        isAttacking = true;
        attackState = 1;

        bool isRunning = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.6f;

        if (playerStatus.detectedTargets.Count <= 0)
            movePlayerOnAttack(3.0f);
        else
            movePlayerOnAttack(-0.5f);

        string animationName = "";
        switch (UsedElemental)
        {
            case Enums.ElementalType.Normal:
                animationName = isRunning ? "RunAttack_1" : "Attack_1";
                break;
            case Enums.ElementalType.Storm:
                animationName = isRunning ? "RunAttackStorm_1" : "StormAttack_1";
                break;
            case Enums.ElementalType.Bloody:
                animationName = isRunning ? "RunAttackBloody_1" : "BloodyAttack_1";
                break;
        }

        animator.Play(animationName);
        DealDamage(playerStatus.GetAttackDamageCount());
        if (WorldSoundFXManager.instance)
            PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackSFX, Enums.SoundType.SFX);
    }

    private void ContinueAttack()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackTimeout());

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            bool isRunning = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.6f;
            int maxCombo = isRunning ? 2 : usedElementalSequences;

            attackState++;
            if (attackState > maxCombo)
                attackState = 1;

            if (!isRunning)
            {
                if (playerStatus.detectedTargets.Count <= 0)
                    movePlayerOnAttack(3.0f);
                else
                    movePlayerOnAttack(-0.5f);
            }

            string animationName = "";
            switch (UsedElemental)
            {
                case Enums.ElementalType.Normal:
                    animationName = isRunning ? $"RunAttack_{attackState}" : $"Attack_{attackState}";
                    break;
                case Enums.ElementalType.Storm:
                    animationName = isRunning ? $"RunAttackStorm_{attackState}" : $"StormAttack_{attackState}";
                    break;
                case Enums.ElementalType.Bloody:
                    animationName = isRunning ? $"RunAttackBloody_{attackState}" : $"BloodyAttack_{attackState}";
                    break;
            }

            animator.Play(animationName);
            DealDamage(playerStatus.GetAttackDamageCount());
            if (WorldSoundFXManager.instance)
                PlayPlayerSFXArray(WorldSoundFXManager.instance.playerAttackSFX, Enums.SoundType.SFX);

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
            var selectedElemental = ElementalTypes[TypeId];
            UsedElementalTypeId = TypeId;
            UsedElementalName = selectedElemental.name;
            UsedElemental = selectedElemental.elementalType;
            usedElementalSequences = selectedElemental.elementalAttacksSequences;
        }
    }

    private void Dash()
    {
        Vector2 dashDirection = GetDashDirection();
        Vector3 startPosition = transform.position; // Vector3, by zachować Z

        RaycastHit2D hit = Physics2D.Raycast(startPosition, dashDirection, dashDistance, obstacleLayer);
        Vector3 targetPosition;

        if (hit.collider != null)
        {
            targetPosition = hit.point - (Vector2)(dashDirection * 0.2f);
            targetPosition.z = startPosition.z;
        }
        else
        {
            targetPosition = startPosition + (Vector3)(dashDirection * dashDistance);
            targetPosition.z = startPosition.z;
        }

        if (WorldSoundFXManager.instance)
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.dashSFX, Enums.SoundType.Dialogue);

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

    private void OnPlayerDamaged()
    {
        wasDamagedRecently = true;
    }

    private void OnPlayerDeath()
    {
        //throw new NotImplementedException();
    }
    
    private void CheckForWall()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y) + wallCheckOffset;
        float dirX = playerStatus.isFacedRight ? +1f : -1f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * dirX, wallCheckDistance, wallLayer);
        Debug.DrawRay(origin, Vector2.right * dirX * wallCheckDistance, Color.yellow);

        if (hit.collider != null)
        {
            isTouchingWall = true;
            wallDirection = dirX > 0 ? +1 : -1;
        }
        else
        {
            // Also check the opposite side (in case player is facing away but still touching a wall)
            RaycastHit2D hitOpposite = Physics2D.Raycast(origin, Vector2.right * -dirX, wallCheckDistance, wallLayer);
            if (hitOpposite.collider != null)
            {
                isTouchingWall = true;
                // If the first ray missed but we hit on the “other” side, invert direction
                wallDirection = dirX > 0 ? -1 : +1;
            }
            else
            {
                isTouchingWall = false;
                wallDirection = 0;
            }
        }
    }
    
    private void WallJump()
    {
        if (wallJumpLock) return;

        StartCoroutine(WallJumpLockRoutine());

        float velX = -wallDirection * wallJumpHorizontalForce;
        float velY =  wallJumpVerticalForce;

        if ((wallDirection < 0 && !playerStatus.isFacedRight) ||
            (wallDirection > 0 && playerStatus.isFacedRight))
        {
            transform.Rotate(new Vector3(0f, 180f, 0f));
            playerStatus.isFacedRight = !playerStatus.isFacedRight;
        }

        playerBody.velocity = new Vector2(velX, velY);
    }
    
    private IEnumerator WallJumpLockRoutine()
    {
        wallJumpLock = true;

        yield return new WaitForSeconds(wallJumpLockTime);

        while (isTouchingWall && Mathf.Abs(playerBody.velocity.x) < minWallUnstickSpeed)
            yield return null;

        wallJumpLock = false;
    }
    
    
    public IEnumerator ApplyKnockback(Vector2 knockback, float knockbackTime)
    {
        externalForce = true;
        playerBody.AddForce(knockback, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackTime);
        externalForce = false;
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

        // Czy edytowany obiekt jest jednym obiektem
        if (serializedObject.isEditingMultipleObjects)
        {
            EditorGUILayout.HelpBox("Multi-object editing not supported", MessageType.Error);
            return;
        }

        selectedElementalTypeProp.intValue = EditorGUILayout.Popup("Choose elemental type",
            selectedElementalTypeProp.intValue, new string[] { "0", "1", "2", "3", "4", "5" });

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
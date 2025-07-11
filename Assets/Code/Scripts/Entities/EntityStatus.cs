using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EntityStatus : MonoBehaviour
{
    [Header("Entity Status")]
    public string entityName = "";
    public int entityLevel = 1;
    public int entityExperiencePoints = 0;
    [ReadOnly] public int entityExperienceToNextLvl = 50;

    public Inactive.ObservableVariable<float> entityHealthPoints;
    public Inactive.ObservableVariable<float> entityMaxHealth;
    public float BaseMaxHealth;
    
    public int droppedXp = 0;
    public int gold = 0;
    public float AttackDamage = 10.0f;
    public List<AttackType> AttackTypes;
    public float MovementSpeed = 5.0f;
    public bool isFacedRight = true;
    public bool isDead = false;

    [Header("Boss Settings")]
    public bool isEnemy = false;
    public bool isBoss = false;

    [Header("Entity Settings")]
    public float deathAnimationLength = 1.0f;
    public List<GameObject> detectedTargets;
    public float attackRange;
    public Color lightDamageColor;
    public Color heavyDamageColor;
    public Color deathColor;
    public GameObject healthBar;
    public bool isAlerted;
    
    public Enums.EntityType entityType = Enums.EntityType.Human;
    public Enums.EnemyType enemyType = Enums.EnemyType.ShivernDog;

    [Header("Entity FX")]
    [SerializeField] private GameObject healFX;
    [SerializeField] private Transform healFXParent;
    private Coroutine entityHealFXCoroutine;

    UI_YouDiedPopUp_Controller youDiedPopUpController;

    private GameObject mainUserInterface;
    private SpriteRenderer spriteRenderer;
    private GameObject player;
    private GameObject entityObject;
    private float BaseAttackDamage = 0;
    private LootTable lootTable;
    //private MissionTracker missionTracker;
    private Volume postProcessVolume;
    public event System.Action<GameObject, float> OnPlayerDamageTaken;
    public event System.Action OnPlayerDeath;
    public event System.Action OnEntityDeath;

    [Header("Incoming Damage Settings")]
    // Wartość wyrażona w procentach, która odpowiada za % otrzymywanych obrażeń
    public float incomingDamagePercent = 1.0f;
    /*
     * TODO: dodać base ad, oraz hp, aby uniknąć możliwości zmieniania statystyk w nieskończoność
     */
    private void Awake()
    {
        mainUserInterface = GameObject.Find("Main User Interface");
        player = GameObject.Find("Player");
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        lootTable = GetComponent<LootTable>();
        //missionTracker = player.GetComponent<MissionTracker>();
        if (gameObject.CompareTag("Player"))
        {
            if (youDiedPopUpController != null)
                return;

            youDiedPopUpController = FindFirstObjectByType<UserInterfaceController>().GetComponentInChildren<UI_YouDiedPopUp_Controller>();
        }

        BaseAttackDamage = AttackDamage;
        BaseMaxHealth = entityMaxHealth.value;
    }

    private void Start()
    {
        entityHealthPoints.OnChange += (oldVal, newVal) => OnEntityHealthPointsChange(oldVal, newVal);
        entityMaxHealth.OnChange += (oldVal, newVal) => OnEntityMaxHealthPointsChange(oldVal, newVal);
    }

    private void OnDestroy()
    {
        entityHealthPoints.OnChange -= (oldVal, newVal) => OnEntityHealthPointsChange(oldVal, newVal);
        entityMaxHealth.OnChange -= (oldVal, newVal) => OnEntityMaxHealthPointsChange(oldVal, newVal);
    }

    private void OnEnable()
    {
        isDead = false;
    }

    /*
     * Nazwa
     */
    public void SetName(string name)
    {
        this.entityName = name;
    }
    public string GetName()
    {
        return this.entityName;
    }
    
    /*
     * Poziom doświadczenia
     */
    public void SetLevel(int level)
    {
        this.entityLevel = level;
    }
    public int GetLevel()
    {
        return this.entityLevel;
    }
    
    /*
     * Ilość punktów doświadczenia
     */
    public void SetXp(int xp)
    {
        this.entityExperiencePoints = xp;
    }
    public int GetXp()
    {
        return this.entityExperiencePoints;
    }
    public void AddXp(int xpAmount)
    {
        while (xpAmount > 0)
        {
            int xpToLvlUp = entityExperienceToNextLvl - GetXp();
            
            // Jeżeli nie mamy wystaczająco xp do lvl up
            if (xpAmount < xpToLvlUp)
            {
                SetXp( GetXp() + xpAmount );
                break;
            }
            if ( xpToLvlUp <= xpAmount )
            {
                SetLevel( GetLevel() + 1 );
                SetExpToNextLVl(  Convert.ToInt32(Convert.ToDouble(GetExpToNextLVl()) * 1.25 ) );
                xpAmount -= xpToLvlUp;
                this.entityExperiencePoints = 0;
            }
        }
    }
    
    /*
     * Punkty życia
     */
    public void SetHp(float hp)
    {
        this.entityHealthPoints.value = hp;
    }
    public float GetHp()
    {
        return this.entityHealthPoints.value;
    }

    public float GetBaseMaxHealth()
    {
        return BaseMaxHealth;
    }
    
    /*
     * Maksymalne punkty życia
     */
    public void SetMaxHp(float maxHp)
    {
        this.entityMaxHealth.value = maxHp;
    }
    public float GetMaxHp()
    {
        return this.entityMaxHealth.value;
    }

    public void DealDamage(float damage, GameObject attackingEntity = null)
    {
        if (isDead)
            return;

        // zawsze podawać attackingEntity, gdy przeciwnik atakuje gracza
        // każde attackingEntity MUSI przy ataku mieć obliczane "isFacedRight"
        if (gameObject.CompareTag("Player"))
        {
            bool isBLocking = gameObject.GetComponent<Player>().isBlocking;
            bool isParrying = gameObject.GetComponent<Player>().isParrying;
            GameObject playerAppearance = gameObject.transform.Find("PlayerAppearance").gameObject;
            bool isPlayerFacedToEnemy = false;
            
            if (attackingEntity)
            {
                EntityStatus playerStatus = gameObject.GetComponent<EntityStatus>();
                EntityStatus enemyStatus = attackingEntity.GetComponent<EntityStatus>();
                Debug.Log(playerStatus);
                Debug.Log(enemyStatus);
                isPlayerFacedToEnemy = (playerStatus.isFacedRight && !enemyStatus.isFacedRight) ||
                                       (!playerStatus.isFacedRight && enemyStatus.isFacedRight);
                //Debug.Log("isPlayerFacedToEnemy: "+ isPlayerFacedToEnemy);
            }
            // Debug.Log(isParrying);

            if (isParrying && isPlayerFacedToEnemy)
            {
                // gracz sparował cios
                float parryingDamageReduction = 0f; // 0 = 100% redukcji
                if ( damage * parryingDamageReduction * incomingDamagePercent >= GetHp() )
                {
                    /*
                     * Gracz ginie
                     * TODO: animacja śmierci
                     */
                    PlayerDeathEvent();
                } else if (damage * parryingDamageReduction * incomingDamagePercent < GetHp())
                {
                    if (WorldSoundFXManager.instance)
                        PlayPlayerSFXSingle(WorldSoundFXManager.instance.playerParrySFX, Enums.SoundType.SFX, 2f);

                    if(WorldSoundFXManager.instance)
                    // gracz otrzymuje obrażenia
                        GettingDamageEvent(damage * parryingDamageReduction * incomingDamagePercent, true);
                    
                    // odgrywanie animacji po sparowaniu
                    playerAppearance.GetComponent<Animator>().Play("parryAttack");
                }
                
                
            } else if (isBLocking && isPlayerFacedToEnemy)
            {
                float blockingDamageReduction = 0.6f; // 0.6 = 40% redukji

                OnPlayerDamageTaken?.Invoke(attackingEntity, damage * blockingDamageReduction * incomingDamagePercent);
                // gracz zablokował cios
                if ( damage * blockingDamageReduction * incomingDamagePercent >= GetHp() )
                {
                    /*
                     * Gracz ginie
                     * TODO: animacja śmierci
                     */
                    PlayerDeathEvent();
                } else if (damage * blockingDamageReduction * incomingDamagePercent < GetHp())
                {
                    // gracz otrzymuje obrażenia
                    
                    //if (null != missionTracker) missionTracker.AddDamageTaken(damage * blockingDamageReduction * incomingDamagePercent);
                    GettingDamageEvent(damage * blockingDamageReduction * incomingDamagePercent, true);
                }
            }
            else
            {
                //StartCoroutine(SygnalizeGettingDamage());
                // gracz nie sparował, ani nie zablokował ciosu
                if ( damage * incomingDamagePercent >= GetHp() )
                {
                    /*
                     * Gracz ginie
                     * TODO: animacja śmierci
                     */
                    PlayerDeathEvent();
                } else if (damage * incomingDamagePercent < GetHp())
                {
                    // gracz otrzymuje obrażenia
                    //if (null != missionTracker) missionTracker.AddDamageTaken(damage * incomingDamagePercent);
                    GettingDamageEvent(damage * incomingDamagePercent, true);
                    OnPlayerDamageTaken?.Invoke(attackingEntity, damage * incomingDamagePercent);
                    
                    // animacja paska hp sygnalizująca otrzymanie obrażeń
                }
            }
        }
        else
        {
            // Kod dla wszystkich encji poza graczem
            if ( damage * incomingDamagePercent >= GetHp() )
            {
                if (isBoss)
                {

                }
                // Encja ginie
                DeathEvent();
                OnEntityDeath?.Invoke();

            } else if (damage * incomingDamagePercent < GetHp())
            {
                // encja otrzymuje obrażenia
                GettingDamageEvent(damage, false);
            }
        }
    }

    void DeathEvent()
    {
        if (isDead)
            return;
        isDead = true;

        // Reportujemy śmierć encji, aby zaktualizować cele gracza
        if (isEnemy)
        {
            if (PlayerObjectiveTracker.instance != null)
            {
                PlayerObjectiveTracker.instance.ReportEnemyKilled(this.enemyType);
            }
            else
            {
                Debug.LogWarning("PlayerObjectiveTracker.instance is null. Cannot report enemy kill.");
            }
        }


        player.GetComponent<EntityStatus>().AddXp(droppedXp);


        BossData bossData = gameObject.GetComponentInParent<BossData>();
        if (bossData != null)
        {
            bossData.OnDeath();
        }
        else
        {
            StartCoroutine(DeathAnimation(deathColor, 0.1f));
        }
    }

    public void PlayerDeathEvent()
    {
        if (isDead)
            return;
        isDead = true;

        EntityStatus playerStatus = player.GetComponent<EntityStatus>();
        OnPlayerDeath?.Invoke();
        WorldSaveGameManager.instance.LoadGame(4f);
        youDiedPopUpController.ActivateInterface(6);

        // przelanie expa z gracza na przeciwnika
        AddXp(playerStatus.GetXp());
        playerStatus.SetXp( 0 );
        
        StartCoroutine(DeathAnimation(deathColor, 0.1f));
    }

    public void PlayHealFX()
    {
        if (entityHealFXCoroutine != null)
            StopCoroutine(entityHealFXCoroutine);

        entityHealFXCoroutine = StartCoroutine(PlayHealFXCoroutine());
    }

    private IEnumerator PlayHealFXCoroutine()
    {
        if (healFX == null || healFXParent == null)
        {
            Debug.LogWarning("Heal FX or Heal FX Parent is not assigned.");
            yield return null;
        }

        GameObject healEffect = Instantiate(healFX, healFXParent.position, Quaternion.identity, healFXParent);
        float healEffectDuration = healEffect.GetComponent<ParticleSystem>().main.duration;

        yield return new WaitForSeconds(healEffectDuration > 0?healEffectDuration:2f);
        Destroy(healEffect);
    }

    void GettingDamageEvent( float damage, bool isPlayer = false )
    {

        // Debug.Log("Zadaję obrażenia");
        SetHp(GetHp() - damage);

        if (!isPlayer) return;

        StartCoroutine(SygnalizeGettingDamage());
    }

    private IEnumerator SygnalizeGettingDamage()
    {
        // w tym momencie gameObject musi być graczem
        Image healthBarImage = healthBar.GetComponent<Image>();
        Color currentColor = healthBarImage.color;

        // Ustawienie fill %
        var fillPercentage = player.GetComponent<EntityStatus>().GetHp() / player.GetComponent<EntityStatus>().GetMaxHp();
        healthBarImage.fillAmount = fillPercentage;
        
        // animacja otrzymania obrażeń
        healthBarImage.color = heavyDamageColor;

        MoveHpBar(0, 3);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, -6);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, 6);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, -6);
        yield return new WaitForSeconds(0.05f);
        MoveHpBar(0, 3);

        healthBarImage.color = currentColor;
    }

    private void OnEntityHealthPointsChange(float oldValue, float newValue)
    {
        if (gameObject.GetComponent<Player>())
        {
            // Ustawienie fill % dla paska zdrowia gracza
            var fillPercentage = newValue / GetMaxHp();
            healthBar.GetComponent<Image>().fillAmount = fillPercentage;

            if (entityHealthPoints.value <= 0)
            {
                PlayerDeathEvent();
            }
        }
    }

    void OnEntityMaxHealthPointsChange(float oldValue, float newValue)
    {
        if (gameObject.GetComponent<Player>())
        {
            var fillPercentage = entityHealthPoints / GetMaxHp();
            healthBar.GetComponent<Image>().fillAmount = fillPercentage;
        }
    }

    void MoveHpBar(float xOffset, float yOffset)
    {
        // Pobierz bieżące pozycje
        Vector3 currentPosition = healthBar.transform.localPosition;

        // Dodaj przesunięcie
        currentPosition.x += xOffset;
        currentPosition.y += yOffset;

        // Ustaw nową pozycję
        healthBar.transform.localPosition = currentPosition;
    }
    
    private IEnumerator ChangeColorForInterval(Color color, float duration)
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = Color.white;
        }
        else
        {
            Debug.Log("Nie wykryto spriteRenderer");
        }
    }
    
    private IEnumerator DeathAnimation(Color color, float duration)
    {
        if (spriteRenderer && entityHealthPoints > 0)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = Color.white;
            var entityAnimator = gameObject.GetComponentInChildren<Animator>();
            SetHp(0);
            entityAnimator.SetTrigger("HasDied");
            yield return new WaitForSeconds(deathAnimationLength);
            gameObject.SetActive(false);
        }
    }
    
    /*
     * Punkty doświadczenia do następnego poziomu
     */
    public void SetExpToNextLVl(int expToNextLvl)
    {
        this.entityExperienceToNextLvl = expToNextLvl;
    }
    public int GetExpToNextLVl()
    {
        return this.entityExperienceToNextLvl;
    }

    public float GetBaseAttackDamage()
    {
        return this.BaseAttackDamage;
    }
    
    /*
     * Ilość złota
     */
    public void SetGold(int gold)
    {
        this.gold = gold;
    }
    public int GetGold()
    {
        return this.gold;
    }
    public void AddGold(int gold)
    {
        this.gold += gold;

        /*
         * Jeśli encja jest graczem, to wyświetlamy złoto w UI
         */
        if ( gameObject.CompareTag("Player") )
        {
            // NAPRAW KTOŚ TO BO CRASHUJE!
            //GameObject UiGoldCount = mainUserInterface.transform.Find("Gold/Count").gameObject;
            //if (UiGoldCount)
            //{
            //    UiGoldCount.GetComponent<TextMeshProUGUI>().text = Convert.ToString( this.gold );
            //}
        }
    }
    
    /*
     * Obrażenia
     */
    public void SetAttackDamageCount(float AttackDamage)
    {
        this.AttackDamage = AttackDamage;
    }
    public float GetAttackDamageCount()
    {
        return this.AttackDamage;
    }

    /*
     * Prędkość ruchu
     */
    public void SetMovementSpeed(float MovementSpeed)
    {
        this.MovementSpeed = MovementSpeed;
    }
    public float GetMovementSpeed()
    {
        return this.MovementSpeed;
    }

    public void SetIsAlerted(bool IsAlerted)
    {
        this.isAlerted = IsAlerted;
    }

    public bool GetIsAlerted()
    {
        return this.isAlerted;
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
}

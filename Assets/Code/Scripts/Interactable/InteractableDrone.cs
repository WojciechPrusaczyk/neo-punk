using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class InteractableDrone : Interactable
{
    [Header("Drone Settings")]
    public int ID;
    public string droneName;
    public Sprite backgroundImage;

    [Header("Drone Activation Data")]
    public bool isActivated = false;
    public Inactive.ObservableVariable<bool> isEnemyNearby;
    private float searchRadius = 6.5f;

    [Header("Interaction floating text")]
    public CanvasGroup interactionTextCanvas;

    private Coroutine interactionHealCoroutine;
    private Coroutine dimOutCoroutine;

    private float activatedYOffset = 0.3f;
    private float activatedLightIntensity = 7f;
    private float activatedLightRadiusOuter = 7f;

    // Drone light
    private Light2D droneLight;

    private UI_DroneInterface_Controller droneController;

    protected override void Awake()
    {
        base.Awake();

        if (UserInterfaceController.instance == null)
            return;

        var allInterfaces = UserInterfaceController.instance.GetInterfaces();
        if (allInterfaces == null)
            return;

        int interfaceIndex = allInterfaces.FindIndex(x => x != null && x.interfaceRoot != null && x.interfaceRoot.name == "DroneInterface");
        if (interfaceIndex == -1)
            return;

        droneLight = GetComponentInChildren<Light2D>();

        var droneInterface = allInterfaces[interfaceIndex];

        droneController = droneInterface.interfaceRoot.GetComponent<UI_DroneInterface_Controller>();
        if (droneController == null)
            return;

        if (isActivated)
            droneLight.transform.position += new Vector3(0, activatedYOffset, 0);
    }

    private void OnEnable()
    {
        if (animator == null)
            return;

        if (isActivated)
        {
            animator.CrossFade("Drone_Active", 0.1f, 0, 0f);
            droneLight.intensity = activatedLightIntensity;
            droneLight.pointLightOuterRadius = activatedLightRadiusOuter;
        }
    }

    protected override void Start()
    {
        base.Start();

        isEnemyNearby.OnChange += (oldVal, newVal) => OnIsEnemyNearbyChanged(oldVal, newVal);
    }

    protected override void Update()
    {
        base.Update();

        if (WorldGameManager.instance == null || WorldAIManager.instance == null)
            return;

        // Stw�rz okr�g wok� drona, aby sprawdzi�, czy w pobli�u s� wrogowie
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);
        isEnemyNearby.value = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<EntityStatus>() != null && collider.GetComponent<EntityStatus>().isEnemy)
            {
                isEnemyNearby.value = true;
                break;
            }
        }
    }

    private void OnIsEnemyNearbyChanged(bool oldVal, bool newVal)
    {
        if (newVal)
        {
            CloseUIOnExit();

            // Ukryj ikon� interakcji
            if (instantiatedIcon != null)
            {
                Destroy(instantiatedIcon);
                instantiatedIcon = null;
            }

            if (isActivated)
            {
                if (dimOutCoroutine != null)
                {
                    StopCoroutine(dimOutCoroutine);
                }
                dimOutCoroutine = StartCoroutine(DimOutLight(.5f));
            }
        }
        else
        {
            if (instantiatedIcon == null && isPlayerInRange)
                CreateIcon(transform);

            interactableCollider.enabled = false;
            interactableCollider.enabled = true;
        }
    }

    protected override void Interact()
    {
        if (droneController.isDroneUIActive)
            return;

        if (isInteractionOnCooldown)
            return;

        if (isEnemyNearby)
            return;

        StartCoroutine(InteractionCooldown());

        if (WorldSaveGameManager.instance == null)
            return;

        // Pr�ba odczytania stanu ogniska z pliku zapisu
        // Je�li nie ma w pliku lub jest false to isActivated zostanie ustawione na false (TryGetValue to bezpieczna metoda, kt�ra pr�buje odczyta� warto�� z s�ownika SerializeDictionary)
        // Je�li jest w pliku zapisu i jest true to isActivated zostanie ustawione na true
        WorldSaveGameManager.instance.currentCharacterData.activeDrones.TryGetValue(ID, out isActivated);

        // Zmiana ID ostatnio odwiedzonego ogniska
        WorldSaveGameManager.instance.currentCharacterData.lastVisitedDroneIndex = ID;
        WorldSaveGameManager.instance.currentCharacterData.lastVisitedDroneName = string.IsNullOrEmpty(droneName) ? "Unknown" : droneName;

        // Je�li ognisko nie jest aktywowane to dodajemy je do s�ownika activeCampfires i ustawiamy isActivated na true
        if (!isActivated)
        {
            if (animator != null)
            {
                Debug.Log("Activating drone with ID: " + ID);
                animator.SetBool("IsActivated", true);
            }
            // Dodaj drona do aktywnych dron�w w zapisie gry
            WorldSaveGameManager.instance.currentCharacterData.activeDrones.Add(ID, true);

            // Pu�� d�wi�k aktywacji drona
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.droneActivation, Enums.SoundType.SFX);

            isActivated = true;
        }
        else
        {
            RemoveIcon();

            if (droneController != null)
            {
                droneController.ActivateInterface(ID);

                if (WorldAIManager.instance != null)
                    WorldAIManager.instance.RespawnAllEnemies();

                WorldGameManager.instance.player.playerStatus.detectedTargets.Clear();
            }
        }

        // Zapisanie stanu gry
        WorldSaveGameManager.instance.SaveGame();
    }

    private IEnumerator HealInteractionTask(float duration)
    {
        
        interactionTextCanvas.alpha = 1f;
        yield return new WaitForSeconds(duration);

        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            interactionTextCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    protected override void CloseUI()
    {
        if (droneController.isDroneUIActive)
            droneController.DeactivateInterface();

        if (instantiatedIcon == null)
            CreateIcon(transform);
    }

    protected override void PrepareInteractable()
    {
        base.PrepareInteractable();

        if (WorldSaveGameManager.instance != null)
            WorldSaveGameManager.instance.currentCharacterData.activeDrones.TryGetValue(ID, out isActivated);
    }

    protected override void CloseUIOnExit()
    {
        if (droneController.isDroneUIActive)
            droneController.DeactivateInterface();
    }

    protected override void PrepareTriggerEnterPlayer(Collider2D collision)
    {
        if (isEnemyNearby)
            return;

        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            animator.SetBool("IsPlayerNearby", true);

            // Wy�wietl ikon� interakcji
            if (!droneController.isDroneUIActive && instantiatedIcon == null)
                CreateIcon(transform);

            if (!isActivated)
                return;

            // Leczenie gracza
            HealPlayer();
        }
    }

    protected void HealPlayer()
    {
        if (interactionTextCanvas != null)
        {
            if (WorldGameManager.instance != null)
                WorldGameManager.instance.player.playerStatus.PlayHealFX();

            var playerHealth = WorldGameManager.instance.player.playerStatus;

            // Ustawienie punkt�w zdrowia gracza na maksymalne zdrowie
            playerHealth.entityHealthPoints.value = playerHealth.entityMaxHealth.value;

            if (playerHealth.entityHealthPoints.value > playerHealth.entityMaxHealth.value)
            {
                playerHealth.entityHealthPoints.value = playerHealth.entityMaxHealth;
            }

            if (interactionHealCoroutine != null)
                StopCoroutine(interactionHealCoroutine);

            interactionHealCoroutine = StartCoroutine(HealInteractionTask(1.5f));
        }
    }

    protected void ChangeLightIntensity(float lightIntensity)
    {
        if (droneLight == null)
            return;

        if (isEnemyNearby)
            return;

        droneLight.intensity = lightIntensity;
    }

    private IEnumerator DimOutLight(float duration)
    {
        if (droneLight == null)
            yield break;
        float startIntensity = droneLight.intensity;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            droneLight.intensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / duration);
            yield return null;
        }
        droneLight.intensity = 0f;
    }

    protected void ChangeLightRadiusOuter(float radius)
    {
        if (droneLight == null)
            return;

        droneLight.pointLightOuterRadius = radius;
    }

    public void MoveLightUp()
    {
        StartCoroutine(MoveLightUpCoroutine(activatedYOffset, .7f, 0f));
    }

    protected IEnumerator MoveLightUpCoroutine(float yAmount, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Przesuni�cie �wiat�a w g�r�
        Vector3 startPosition = droneLight.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, yAmount, 0);
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / time);
            droneLight.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        droneLight.transform.position = endPosition;
    }
}

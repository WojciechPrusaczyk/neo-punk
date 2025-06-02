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

    [Header("Interaction floating text")]
    public CanvasGroup interactionTextCanvas;

    private Coroutine interactionHealCoroutine;

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

    protected override void Interact()
    {
        if (droneController.isDroneUIActive)
            return;

        if (isInteractionOnCooldown)
            return;

        RemoveIcon();

        StartCoroutine(InteractionCooldown());

        if (WorldSaveGameManager.instance == null)
            return;

        // Próba odczytania stanu ogniska z pliku zapisu
        // Jeœli nie ma w pliku lub jest false to isActivated zostanie ustawione na false (TryGetValue to bezpieczna metoda, która próbuje odczytaæ wartoœæ z s³ownika SerializeDictionary)
        // Jeœli jest w pliku zapisu i jest true to isActivated zostanie ustawione na true
        WorldSaveGameManager.instance.currentCharacterData.activeDrones.TryGetValue(ID, out isActivated);

        // Zmiana ID ostatnio odwiedzonego ogniska
        WorldSaveGameManager.instance.currentCharacterData.lastVisitedDroneIndex = ID;

        // Jeœli ognisko nie jest aktywowane to dodajemy je do s³ownika activeCampfires i ustawiamy isActivated na true
        if (!isActivated)
        {
            if (animator != null)
            {
                Debug.Log("Activating drone with ID: " + ID);
                animator.SetBool("IsActivated", true);
            }
            WorldSaveGameManager.instance.currentCharacterData.activeDrones.Add(ID, true);
            isActivated = true;
        }
        else
        {
            if (droneController != null)
            {
                droneController.ActivateInterface(ID);

                if (WorldAIManager.instance != null)
                    WorldAIManager.instance.RespawnAllEnemies();

                // Leczenie gracza
                if (interactionTextCanvas != null)
                {
                    if (WorldGameManager.instance != null)
                        WorldGameManager.instance.player.playerStatus.PlayHealFX();

#if UNITY_EDITOR
                    var playerHealth = WorldGameManager.instance.player.playerStatus;
                    playerHealth.entityHealthPoints += 10;
                    if (playerHealth.entityHealthPoints > playerHealth.entityMaxHelath)
                    {
                        playerHealth.entityHealthPoints = playerHealth.entityMaxHelath;
                    }
#endif

                    if (interactionHealCoroutine != null)
                        StopCoroutine(interactionHealCoroutine);

                    interactionHealCoroutine = StartCoroutine(HealInteractionTask(1.5f));
                }
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

        if (instantiatedIcon == null && isPlayerInRange)
        {
            CreateIcon(transform);
        }
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
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            animator.SetBool("IsPlayerNearby", true);

            // Wyœwietl ikonê interakcji
            if (!droneController.isDroneUIActive)
            {
                CreateIcon(transform);
            }
        }
    }

    protected void ChangeLightIntensity(float lightIntensity)
    {
        if (droneLight == null)
            return;

        droneLight.intensity = lightIntensity;
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
        // Przesuniêcie œwiat³a w górê
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

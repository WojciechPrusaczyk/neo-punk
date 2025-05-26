using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class InteractableCampfire : Interactable
{
    [Header("Campfire Settings")]
    public int ID;
    public string campfireName;
    public Sprite backgroundImage;

    [Header("Campfire Activation Data")]
    public bool isActivated = false;
    public Animator campfireAnimator;

    public GameObject activatedCampfireFX;
    public GameObject instantiatedActiveCampfireFX;

    // Œwiat³o ogniska
    private float startLightIntensity = 0f;
    private float endLightIntensity = 1f;
    private float duration = 1f;
    private float activatedYOffset = 0.4f;
    private float activatedLightIntensity = 7f;
    private float activatedLightRadiusOuter = 7f;

    private Light2D campfireLight;

    private bool isInteractionOnCooldown = false;
    private float interactionCooldownDuration = .5f;

    private UI_CampfireInterface_Controller campfireController;

    protected override void Awake()
    {
        base.Awake();

        if (UserInterfaceController.instance == null)
            return;

        var allInterfaces = UserInterfaceController.instance.GetInterfaces();
        if (allInterfaces == null)
            return;

        int interfaceIndex = allInterfaces.FindIndex(x => x != null && x.interfaceRoot != null && x.interfaceRoot.name == "CampfireInterface");
        if (interfaceIndex == -1)
            return;

        campfireLight = GetComponentInChildren<Light2D>();
        campfireAnimator = GetComponent<Animator>();

        var campfireInterfaceEntry = allInterfaces[interfaceIndex];

        campfireController = campfireInterfaceEntry.interfaceRoot.GetComponent<UI_CampfireInterface_Controller>();
        if (campfireController == null)
            return;

        campfireLight.transform.position += new Vector3(0, activatedYOffset, 0);
    }

    private void OnEnable()
    {
        if (campfireAnimator == null)
            return;

        if (isActivated)
        {
            campfireAnimator.CrossFade("Campfire_Active", 0.1f, 0, 0f);
            campfireLight.intensity = activatedLightIntensity;
            campfireLight.pointLightOuterRadius = activatedLightRadiusOuter;
        }
    }

    protected override void Interact()
    {
        if (campfireController.isCampfireUIActive)
            return;

        if (isInteractionOnCooldown)
            return;

        StartCoroutine(InteractionCooldown());

        if (WorldSaveGameManager.instance == null)
            return;

        // Próba odczytania stanu ogniska z pliku zapisu
        // Jeœli nie ma w pliku lub jest false to isActivated zostanie ustawione na false (TryGetValue to bezpieczna metoda, która próbuje odczytaæ wartoœæ z s³ownika SerializeDictionary)
        // Jeœli jest w pliku zapisu i jest true to isActivated zostanie ustawione na true
        WorldSaveGameManager.instance.currentCharacterData.activeCampfires.TryGetValue(ID, out isActivated);

        // Jeœli ognisko nie jest aktywowane to dodajemy je do s³ownika activeCampfires i ustawiamy isActivated na true
        if (!isActivated)
        {
            if (campfireAnimator != null)
            {
                Debug.Log("Activating campfire with ID: " + ID);
                campfireAnimator.SetBool("IsActivated", true);
                StartCoroutine(MoveLightUp(activatedYOffset, 1.5f, .5f));
            }
            WorldSaveGameManager.instance.currentCharacterData.activeCampfires.Add(ID, true);
            isActivated = true;
        }
        else
        {
            if (campfireController != null)
            {
                campfireController.ActivateInterface(ID);
            }
        }

#if UNITY_EDITOR
        var playerHealth = WorldGameManager.instance.player.playerStatus;
        playerHealth.entityHealthPoints += 10;
        if (playerHealth.entityHealthPoints > playerHealth.entityMaxHelath)
        {
            playerHealth.entityHealthPoints = playerHealth.entityMaxHelath;
        }
#endif

        // Zmiana ID ostatnio odwiedzonego ogniska
        WorldSaveGameManager.instance.currentCharacterData.lastVisitedCampfireIndex = ID;

        // Zapisanie stanu gry
        WorldSaveGameManager.instance.SaveGame();
    }

    private IEnumerator InteractionCooldown()
    {
        isInteractionOnCooldown = true;
        yield return new WaitForSeconds(interactionCooldownDuration);
        isInteractionOnCooldown = false;
    }

    protected override void CloseUI()
    {
        if (campfireController.isCampfireUIActive)
            campfireController.DeactivateInterface();

        if (instantiatedIcon == null && InteractIcon != null && isPlayerInRange)
        {
            Vector3 positionAboveCampfire = new Vector3(transform.position.x, transform.position.y + interactableIconYOffset, transform.position.z);
            InstantiateInteractionIcon(InteractIcon, positionAboveCampfire, 0);
        }
    }

    protected override void PrepareInteractable()
    {
        base.PrepareInteractable();

        if (WorldSaveGameManager.instance != null)
            WorldSaveGameManager.instance.currentCharacterData.activeCampfires.TryGetValue(ID, out isActivated);
    }

    protected override void CloseUIOnExit()
    {
        if (campfireController.isCampfireUIActive)
            campfireController.DeactivateInterface();
    }

    protected override void PrepareTriggerEnterPlayer(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Wyœwietl ikonê interakcji
            if (InteractIcon != null && !campfireController.isCampfireUIActive)
            {
                Vector3 positionAboveCampfire = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                InstantiateInteractionIcon(InteractIcon, positionAboveCampfire, interactableIconYOffset);
            }
        }
    }

    protected void ChangeLightIntensity(float lightIntensity)
    {
        if (campfireLight == null)
            return;

        campfireLight.intensity = lightIntensity;
    }

    protected void ChangeLightRadiusOuter(float radius)
    {
        if (campfireLight == null)
            return;

        campfireLight.pointLightOuterRadius = radius;
    }

    protected IEnumerator MoveLightUp(float yAmount, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Przesuniêcie œwiat³a w górê
        Vector3 startPosition = campfireLight.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, yAmount, 0);
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / time);
            campfireLight.transform.position = Vector3.Lerp(startPosition, endPosition, t);
        }
        yield return null;
    }
}

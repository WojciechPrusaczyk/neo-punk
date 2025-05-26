using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class InteractableCampfire : Interactable
{
    public int ID;
    public string campfireName;
    public Sprite backgroundImage;

    public bool isActivated = false;
    public GameObject activatedCampfireFX;
    public GameObject instantiatedActiveCampfireFX;

    // Œwiat³o ogniska
    private float startLightIntensity = 0f;
    private float endLightIntensity = 1f;
    private float duration = 1f;

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

        var campfireInterfaceEntry = allInterfaces[interfaceIndex];

        campfireController = campfireInterfaceEntry.interfaceRoot.GetComponent<UI_CampfireInterface_Controller>();
        if (campfireController == null)
            return;
    }

    protected override void Interact()
    {
        if (campfireController.isCampfireUIActive)
            return;

        if (interactableCollider != null)
        {
            interactableCollider.enabled = false;

            if (!isActivated)
                StartCoroutine(RestoreColliderAfterDelay(colliderDisableTimer));
            else
                StartCoroutine(RestoreColliderAfterDelay(0.1f));
        }

        if (WorldSaveGameManager.instance == null)
            return;

        // Próba odczytania stanu ogniska z pliku zapisu
        // Jeœli nie ma w pliku lub jest false to isActivated zostanie ustawione na false (TryGetValue to bezpieczna metoda, która próbuje odczytaæ wartoœæ z s³ownika SerializeDictionary)
        // Jeœli jest w pliku zapisu i jest true to isActivated zostanie ustawione na true
        WorldSaveGameManager.instance.currentCharacterData.activeCampfires.TryGetValue(ID, out isActivated);

        // Jeœli ognisko nie jest aktywowane to dodajemy je do s³ownika activeCampfires i ustawiamy isActivated na true
        if (!isActivated)
        {
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

        // Stworzenie efektu œwietlnego
        CreateActivatedFX();

        // Zmiana ID ostatnio odwiedzonego ogniska
        WorldSaveGameManager.instance.currentCharacterData.lastVisitedCampfireIndex = ID;

        // Zapisanie stanu gry
        WorldSaveGameManager.instance.SaveGame();
    }

    protected override void CloseUI()
    {
        campfireController.DeactivateInterface();

        if (instantiatedIcon == null && InteractIcon != null && isPlayerInRange)
        {
            Vector3 positionAboveCampfire = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            InstantiateInteractionIcon(InteractIcon, positionAboveCampfire, interactableIconYOffset);
        }
    }

    protected override void PrepareInteractable()
    {
        base.PrepareInteractable();

        if (WorldSaveGameManager.instance != null)
            WorldSaveGameManager.instance.currentCharacterData.activeCampfires.TryGetValue(ID, out isActivated);

        CreateActivatedFX();
    }

    private void CreateActivatedFX()
    {
        if (isActivated)
        {
            if (instantiatedActiveCampfireFX != null)
                return;

            instantiatedActiveCampfireFX = Instantiate(activatedCampfireFX, transform.position, Quaternion.identity);
            instantiatedActiveCampfireFX.transform.SetParent(transform);
            var campfireLight = instantiatedActiveCampfireFX.GetComponentInChildren<Light2D>();

            if (campfireLight != null)
            {
                campfireLight.intensity = startLightIntensity;
                StartCoroutine(FadeInLight(campfireLight));
            }
        }
    }

    private IEnumerator FadeInLight(Light2D campfireLight)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            campfireLight.intensity = Mathf.Lerp(startLightIntensity, endLightIntensity, elapsedTime / duration);
            yield return null;
        }
        campfireLight.intensity = endLightIntensity;
    }

    protected override void CloseUIOnExit()
    {
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
}

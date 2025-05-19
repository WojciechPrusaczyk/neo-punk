using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractableCampfire : Interactable
{
    public int ID;
    public bool isActivated = false;
    public GameObject activatedCampfireFX;
    public GameObject instantiatedActiveCampfireFX;

    // Œwiat³o ogniska
    private float startLightIntensity = 0f;
    private float endLightIntensity = 1f;
    private float duration = 1f;


    protected override void Interact()
    {
        base.Interact();

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

        // Stworzenie efektu œwietlnego
        CreateActivatedFX();

        // Zmiana ID ostatnio odwiedzonego ogniska
        WorldSaveGameManager.instance.currentCharacterData.lastVisitedCampfireIndex = ID;

        // Zapisanie stanu gry
        WorldSaveGameManager.instance.SaveGame();
    }

    protected override void PrepareInteractable()
    {
        base.PrepareInteractable();

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
}

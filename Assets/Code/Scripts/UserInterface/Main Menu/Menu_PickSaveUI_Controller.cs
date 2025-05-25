using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enums;

public class Menu_PickSaveUI_Controller : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Button CharacterSlot1;
    [SerializeField] private Button CharacterSlot2;
    [SerializeField] private Button CharacterSlot3;
    [SerializeField] private Menu_StartGameUI_Controller startGameUIController;

    public void ShowSelectionMenu(bool show)
    {
        if (menu != null)
        {
            menu.gameObject.SetActive(show);
        }
    }

    public void ChangeCharacterSlot(int ID)
    {
        CharacterSlots characterSlot = (CharacterSlots)ID;
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
        menu.SetActive(false);
        SceneManager.LoadScene("InitialLevel");
        MusicManager.instance.PlaySong(1);
        WorldSaveGameManager.instance.LoadGame();
    }

    public void DisableUnexistingSaveLoadButtons()
    {
        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(0))
        {
            CharacterSlot1.interactable = true;
        }
        else
        {
            CharacterSlot1.interactable = false;
        }

        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(1))
        {
            CharacterSlot2.interactable = true;
        }
        else
        {
            CharacterSlot2.interactable = false;
        }

        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(2))
        {
            CharacterSlot3.interactable = true;
        }
        else
        {
            CharacterSlot3.interactable = false;
        }
    }

    public void GoBack()
    {
        if (startGameUIController != null)
        {
            startGameUIController.ShowSelectionMenu(true);
        }
        else
        {
            Debug.LogWarning("StartGameUI_Controller is not assigned in the inspector.");
        }
        menu.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Enums;

public class Menu_PickSaveUI_Controller : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject pickSaveMenu;
    [SerializeField] private GameObject confirmMenu;

    [Header("Buttons")]
    [SerializeField] private Button CharacterSlot1;
    [SerializeField] private Button CharacterSlot2;
    [SerializeField] private Button CharacterSlot3;
    [SerializeField] private Button DeleteSaveGame1;
    [SerializeField] private Button DeleteSaveGame2;
    [SerializeField] private Button DeleteSaveGame3;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button DeleteYesButton;
    [SerializeField] private Button DeleteNoButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI lastDroneSave1;
    [SerializeField] private TextMeshProUGUI lastDroneSave2;
    [SerializeField] private TextMeshProUGUI lastDroneSave3;

    [Header("Controllers")]
    [SerializeField] private Menu_StartGameUI_Controller startGameUIController;

    private bool hasPickedButton = false;

    private MenuBehaviour menuBehaviour;

    private void Awake()
    {
        menuBehaviour = GetComponent<MenuBehaviour>();
    }

    public void ShowSelectionMenu(bool show)
    {
        hasPickedButton = false;

        if (pickSaveMenu == null)
            return;

        pickSaveMenu.gameObject.SetActive(show);
    }

    public void ChangeCharacterSlot(int ID)
    {
        CharacterSlots characterSlot = (CharacterSlots)ID;
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
        pickSaveMenu.SetActive(false);
        SceneManager.LoadScene("InitialLevel");
        MusicManager.instance.PlaySong(1);
        WorldSaveGameManager.instance.LoadGame();
    }

    public void DeleteCharacterSlotSave(int ID)
    {
        confirmMenu.GetComponent<UI_DeleteSaveGameHelper>().characterSlot = (CharacterSlots)ID;

        if (confirmMenu != null && !confirmMenu.activeSelf)
        {
            ToggleCharacterButtons(false);
            DeleteNoButton.Select();
            confirmMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Confirm menu is not assigned or already active in the inspector.");
        }
    }

    public void ConfirmSaveRemoval()
    {
        hasPickedButton = false;

        int ID = (int)confirmMenu.GetComponent<UI_DeleteSaveGameHelper>().characterSlot;

        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(ID))
        {
            WorldSaveGameManager.instance.DeleteGame(ID);
            ToggleCharacterButtons(true);
            DisableUnexistingSaveLoadButtons();
            confirmMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No save file exists for the selected character slot.");
        }
    }

    public void CancelSaveRemoval()
    {
        hasPickedButton = false;

        if (confirmMenu != null && confirmMenu.activeSelf)
        {
            confirmMenu.SetActive(false);
            ToggleCharacterButtons(true);
            DisableUnexistingSaveLoadButtons();
        }
        else
        {
            Debug.LogWarning("Confirm menu is not assigned or already inactive in the inspector.");
        }
    }

    public void DisableUnexistingSaveLoadButtons()
    {
        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(0))
        {
            lastDroneSave1.text = WorldSaveGameManager.instance.characterSlot01.lastVisitedDroneName;
            CharacterSlot1.interactable = true;
            DeleteSaveGame1.interactable = true;
            if (!hasPickedButton)
            {
                CharacterSlot1.Select();
                hasPickedButton = true;
            }
        }
        else
        {
            lastDroneSave1.text = "";
            CharacterSlot1.interactable = false;
            DeleteSaveGame1.interactable = false;
        }

        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(1))
        {
            lastDroneSave2.text = WorldSaveGameManager.instance.characterSlot02.lastVisitedDroneName;
            CharacterSlot2.interactable = true;
            DeleteSaveGame2.interactable = true;
            if (!hasPickedButton)
            {
                CharacterSlot2.Select();
                hasPickedButton = true;
            }
        }
        else
        {
            lastDroneSave2.text = "";
            CharacterSlot2.interactable = false;
            DeleteSaveGame2.interactable = false;
        }

        if (WorldSaveGameManager.instance.CheckIfSaveFileExists(2))
        {
            lastDroneSave3.text = WorldSaveGameManager.instance.characterSlot03.lastVisitedDroneName;
            CharacterSlot3.interactable = true;
            DeleteSaveGame3.interactable = true;
            if (!hasPickedButton)
            {
                CharacterSlot3.Select();
                hasPickedButton = true;
            }
        }
        else
        {
            lastDroneSave3.text = "";
            CharacterSlot3.interactable = false;
            DeleteSaveGame3.interactable = false;
        }
    }

    public void GoBack()
    {
        hasPickedButton = false;
        if (startGameUIController != null)
        {
            startGameUIController.ShowSelectionMenu(true);
        }
        else
        {
            Debug.LogWarning("StartGameUI_Controller is not assigned in the inspector.");
        }
        pickSaveMenu.SetActive(false);
    }

    private void ToggleCharacterButtons(bool status)
    {
        CharacterSlot1.interactable = status;
        CharacterSlot2.interactable = status;
        CharacterSlot3.interactable = status;
        DeleteSaveGame1.interactable = status;
        DeleteSaveGame2.interactable = status;
        DeleteSaveGame3.interactable = status;
        BackButton.interactable = status;
    }
}

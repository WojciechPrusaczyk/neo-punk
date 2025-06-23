using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_StartGameUI_Controller : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private Button newGame;
    [SerializeField] private Button loadGame;
    [SerializeField] private Menu_PickSaveUI_Controller pickSaveUIController;

    [SerializeField] private GameObject noSaveSlots;
    [SerializeField] private Button noSaveSlot_ConfirmButton;

    MenuBehaviour menuBehaviour;

    private void Awake()
    {
        menuBehaviour = FindFirstObjectByType<MenuBehaviour>();
    }

    public void ShowSelectionMenu(bool show)
    {
        if (menu != null)
        {
            menu.gameObject.SetActive(show);
            newGame.Select();
        }
    }

    public void NewGameButton()
    {
        menu.SetActive(false);

        WorldSaveGameManager.instance.AttemptToCreateNewGame();

        if (!WorldSaveGameManager.instance.HasFreeCharacterSlot())
        {
            NoSlotUI(true);
        }
    }

    public void LoadGameButton()
    {
        if (pickSaveUIController != null)
        {
            pickSaveUIController.ShowSelectionMenu(true);
        }
        else
        {
            Debug.LogWarning("PickSaveUI_Controller is not assigned in the inspector.");
        }
        menu.SetActive(false);
    }

    public void NoSlotUI(bool status)
    {
        if (noSaveSlots != null && noSaveSlot_ConfirmButton != null)
        {
            noSaveSlots.SetActive(status);
            if (status)
            {
                menuBehaviour.ToggleButtons(false);
                noSaveSlot_ConfirmButton.Select();
            }
        }
    }

    public void GoBack()
    {
        menu.SetActive(false);
    }
}

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

    public void ShowSelectionMenu(bool show)
    {
        if (menu != null)
        {
            menu.gameObject.SetActive(show);
        }
    }

    public void NewGameButton()
    {
        menu.SetActive(false);

        WorldSaveGameManager.instance.AttemptToCreateNewGame();
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

    public void GoBack()
    {
        menu.SetActive(false);
    }
}

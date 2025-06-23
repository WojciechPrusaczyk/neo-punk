using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{

    public Button playButton;
    public Button exitButton;

    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private Animator animator;

    [SerializeField] Menu_StartGameUI_Controller menuStartGameUIController;
    public bool isMenuActive = false;

    void Awake()
    {
        // if (null != cursorTexture)
        // {
        //     Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        // }
        // else
        // {
        //     Debug.LogError("Not found cursor texture in MenuBehaviour.");
        // }

        animator = GetComponent<Animator>();
        if (null == animator)
        {
            Debug.LogError("Not found animator component in MenuBehaviour.");
        }
    }

    private void Start()
    {
        if (playButton != null)
        {
            SelectPlayButton();
        }
    }

    private void Update()
    { 
    }

    public void LoadGame()
    {
        if (isMenuActive)
            return;

        if (menuStartGameUIController != null)
        {
            menuStartGameUIController.ShowSelectionMenu(true);
            ChangeMenuActiveState(true);
        }
        else
        {
            Debug.LogError("Menu_StartGameUI_Controller not found in MenuBehaviour.");
        }
    }

    public void ChangeMenuActiveState(bool state)
    {
        isMenuActive = state;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SelectPlayButton()
    {
        if (playButton != null)
        {
            playButton.Select();
        }
    }

    public void ExitHover()
    {
        if (null != animator)
        {
            animator.SetBool("exitHover", true);
        }
        else
        {
            Debug.LogError("Not found animator component in MenuBehaviour.");
        }
    }

    public void ExitHoverExit()
    {
        if (null != animator)
        {
            animator.SetBool("exitHover", false);
        }
        else
        {
            Debug.LogError("Not found animator component in MenuBehaviour.");
        }
    }

    public void PlayHover()
    {
        if (null != animator)
        {
            animator.SetBool("playHover", true);
        }
        else
        {
            Debug.LogError("Not found animator component in MenuBehaviour.");
        }
    }

    public void PlayHoverExit()
    {
        if (null != animator)
        {
            animator.SetBool("playHover", false);
        }
        else
        {
            Debug.LogError("Not found animator component in MenuBehaviour.");
        }
    }

    public void ToggleButtons(bool status)
    {
        if (playButton == null || exitButton == null)
            return;

        playButton.interactable = status;
        exitButton.interactable = status;
    }

    public void ToggleButtonsIfFreeCharacterSlots()
    {
        if (WorldSaveGameManager.instance.HasFreeCharacterSlot())
        {
            ToggleButtons(true);
        }
        else
        {
            ToggleButtons(false);
        }
    }
}
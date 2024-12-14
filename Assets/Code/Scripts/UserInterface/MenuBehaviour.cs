using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private Animator animator;

    void Awake()
    {
        if (null != cursorTexture)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
        else
        {
            Debug.LogError("Not found cursor texture in MenuBehaviour.");
        }

        animator = GetComponent<Animator>();
        if (null == animator)
        {
            Debug.LogError("Not found animator component in MenuBehaviour.");
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("InitialLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject PauseUi;
    private GameObject PauseUiCanvas;
    private PlayerInventory PlayerInv;

    private GameObject MainUi;
    public bool IsGamePaused;
    void Awake()
    {
        IsGamePaused = false;
        Cursor.visible = false;
        
        PauseUi = gameObject;
        PauseUiCanvas = PauseUi.transform.GetChild(0).gameObject;
        PauseUiCanvas.SetActive(false);
        PlayerInv = GameObject.Find("Player").GetComponent<PlayerInventory>();
        
        MainUi = GameObject.Find("Main User Interface");
    }

    void Update()
    {
        if ( !PlayerInv.isEquipmentShown && IsGamePaused && (Input.GetKeyDown(InputManager.PauseMenuKey) || Input.GetKeyDown(KeyCode.Escape) ) )
        {
            buttonResume();
        }
        else if ( !PlayerInv.isEquipmentShown && ( Input.GetKeyDown(InputManager.PauseMenuKey) || Input.GetKeyDown(KeyCode.Escape) ) )
        {
            Pause();
        }
    }

    private void Pause()
    {
        IsGamePaused = true;
        Cursor.visible = true;
        MainUi.SetActive(false);
        PauseUiCanvas.SetActive(true);
        Time.timeScale = 0;
    }

    public void buttonResume()
    {
        IsGamePaused = false;
        Cursor.visible = false;
        PauseUiCanvas.SetActive(false);
        MainUi.SetActive(true);
        Time.timeScale = 1;
    }

    public void buttonOptions()
    {
        
    }

    public void buttonQuitToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void buttonQuitGame()
    {
        Application.Quit();
    }
}
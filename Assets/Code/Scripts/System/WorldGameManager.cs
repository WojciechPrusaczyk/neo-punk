using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGameManager : MonoBehaviour
{
    public static WorldGameManager instance;
    // This script will store the most important references
    [Header("Entities")]
    public Player player;

    [Header("Important objects")]
    public GameObject mainCamera;
    public GameObject _camera;

    [Header("Controls")]
    /// <summary>
    /// false = using keyboard and mouse, true = using gamepad
    /// </summary>
    public bool isUsingGamePad = false;

    private List<KeyCode> GamePadKeyCodes = new List<KeyCode>
    {
        KeyCode.JoystickButton0,
        KeyCode.JoystickButton1,
        KeyCode.JoystickButton2,
        KeyCode.JoystickButton3,
        KeyCode.JoystickButton4,
        KeyCode.JoystickButton5,
        KeyCode.JoystickButton6,
        KeyCode.JoystickButton7,
        KeyCode.JoystickButton8,
        KeyCode.JoystickButton9,
        KeyCode.JoystickButton10,
        KeyCode.JoystickButton11,
        KeyCode.JoystickButton12,
        KeyCode.JoystickButton13,
        KeyCode.JoystickButton14,
        KeyCode.JoystickButton15,
        KeyCode.JoystickButton16,
        KeyCode.JoystickButton17,
        KeyCode.JoystickButton18,
        KeyCode.JoystickButton19
    };

    string prefix = "WORLD GAME MANAGER || ";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in GamePadKeyCodes)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    isUsingGamePad = true;
                    return;
                }
                else
                {
                    isUsingGamePad = false;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FindData());
    }

    private void FindDataInScene()
    {
        mainCamera = GameObject.Find("Main Camera");
        if (mainCamera == null)
        {
            Debug.LogError($"{prefix} Main Camera not found in the scene!");
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            player = null;
            _camera = null;
            return;
        }

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            return;
        }

        _camera = GameObject.Find("Camera");
        if (_camera == null)
        {
            return;
        }
    }
    private IEnumerator FindData()
    {
        yield return new WaitForEndOfFrame();
        FindDataInScene();
    }
}

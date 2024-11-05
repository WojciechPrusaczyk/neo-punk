using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class ComputerInterfaceController : MonoBehaviour
{
    public bool isShown;
    public Sprite StarImage;
    private VisualElement InterfaceRoot;
    private GameObject MainUserInterface;
    private Button startMissionButton;
    private Button closeButton;

    /*
     * UI elements
     */
    private Label nameLabel;
    private Label objectiveLabel;
    private Label descriptionLabel;
    private Label codeLabel;
    private VisualElement MissionUi;
    private VisualElement MissionUiPlaceholder;
    private VisualElement statsDetails;


    private void Awake()
    {
        MainUserInterface = GameObject.Find("UserInterface").transform.Find("Main User Interface").gameObject;
    }

    private void Start()
    {
        HideComputerInterface();
    }

    private void OnEnable()
    {
        // Ładujemy UXML
        var uiDocument = GetComponent<UIDocument>();
        InterfaceRoot = uiDocument.rootVisualElement;

        /*
         * pola informacji o misji
         */
        nameLabel = InterfaceRoot.Q<Label>("NameLabel");
        objectiveLabel = InterfaceRoot.Q<Label>("ObjectiveLabel");
        descriptionLabel = InterfaceRoot.Q<Label>("DescriptionLabel");
        codeLabel = InterfaceRoot.Q<Label>("CodeLabel");
        MissionUi = InterfaceRoot.Q<VisualElement>("Main");
        MissionUiPlaceholder = InterfaceRoot.Q<VisualElement>("Placeholder");

        // przyciski w interfejsie użytkownika
        startMissionButton = InterfaceRoot.Q<Button>("StartMissionButton");
        closeButton = InterfaceRoot.Q<Button>("CloseButton");
        closeButton.clicked += OnCloseButtonClick;
        //startMissionButton.clicked += StartMission;

        // ładowanie pól do statystyk
        statsDetails = InterfaceRoot.Q<VisualElement>("DetailsStats");
        
        var missionButtons = InterfaceRoot.Query<Button>(className: "MissionsTreeMissionButton").ToList();
        foreach (var button in missionButtons)
        {
            /*
             * Zmieniono koncepcje misji, ae zostawiam, dla przykładu
             */
            // button.clicked += () => { ShowMissionDetails(button.text); };
        }

        StartCoroutine(CodeAnmiation(0.5f));
    }

    private void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Escape))
        {
            HideComputerInterface();
        }
    }

    public void ShowComputerInterface()
    {
        isShown = true;
        gameObject.SetActive(true);
        MainUserInterface.SetActive(false);
        Cursor.visible = true;
    }

    public void HideComputerInterface()
    {
        isShown = false;
        gameObject.SetActive(false);
        MainUserInterface.SetActive(true);
        Cursor.visible = false;
    }

    private void OnCloseButtonClick()
    {
        HideComputerInterface();
    }
    
    /*
     * TODO: przerobić misje na te pasujące do zmienionej koncepcji misji
     */

    // private void StartMission()
    // {
    //     if (null != SelectedMission)
    //     {
    //         SceneManager.LoadScene(SelectedMission.SceneName);
    //     }
    // }
    //
    // public void ShowMissionDetails(string MissionName)
    // {
    //     SelectedMission = null;
    //     SelectedMission = Resources.Load<MissionData>("Missions/" + MissionName + "/MissionData");
    //     if (null != SelectedMission)
    //     {
    //         MissionUi.style.display = DisplayStyle.Flex;
    //         MissionUiPlaceholder.style.display = DisplayStyle.None;
    //
    //         StartCoroutine(PrintName(SelectedMission.MissionName, 0.1f));
    //         StartCoroutine(PrintObjective(SelectedMission.Objective, 0.05f));
    //         StartCoroutine(PrintDescription(SelectedMission.Description, 0.01f));
    //
    //         /*
    //          * Wyświetlenie wyników najlepszych
    //          */
    //
    //         int CurrentSlot = PlayerPrefs.GetInt("SaveSlot");
    //         float BestTime =
    //             PlayerPrefs.GetFloat("Save" + CurrentSlot.ToString() + "_" + SelectedMission.MissionName + "_BestTime",
    //                 0.0f);
    //         int BestTakedowns =
    //             PlayerPrefs.GetInt(
    //                 "Save" + CurrentSlot.ToString() + "_" + SelectedMission.MissionName + "_BestTakedowns", 0);
    //         float BestDamageTaken =
    //             PlayerPrefs.GetFloat(
    //                 "Save" + CurrentSlot.ToString() + "_" + SelectedMission.MissionName + "_BestDamageTaken", 0.0f);
    //
    //         StartCoroutine(PrintRating(statsDetails, BestTime, BestTakedowns, BestDamageTaken));
    //         // Show best time
    //         /*_time.Q<Label>("TimeBest").text = "Best: " + BestTime.ToString("0:00");
    //         _takedowns.Q<Label>("TakedownsBest").text = "Best: " + BestTakedowns.ToString();
    //         _damageTaken.Q<Label>("DamageTakenBest").text = "Best: " + BestDamageTaken.ToString("0.00");*/
    //     }
    //     else
    //     {
    //         MissionUi.style.display = DisplayStyle.None;
    //         MissionUiPlaceholder.style.display = DisplayStyle.Flex;
    //         Debug.LogError("Not found mission named: " + MissionName);
    //     }
    // }

    IEnumerator PrintName(string name, float letterPause)
    {
        nameLabel.text = "";
        for (int i = 0; i < name.Length; i++)
        {
            nameLabel.text += name[i];
            yield return new WaitForSeconds(letterPause);
        }
    }

    IEnumerator PrintObjective(string name, float letterPause)
    {
        objectiveLabel.text = "";
        for (int i = 0; i < name.Length; i++)
        {
            objectiveLabel.text += name[i];
            yield return new WaitForSeconds(letterPause);
        }
    }

    IEnumerator PrintDescription(string name, float letterPause)
    {
        descriptionLabel.text = "";
        for (int i = 0; i < name.Length; i++)
        {
            descriptionLabel.text += name[i];
            yield return new WaitForSeconds(letterPause);
        }
    }

    IEnumerator CodeAnmiation(float blinkingBreak)
    {
        while (true)
        {
            codeLabel.text += " |";
            yield return new WaitForSeconds(blinkingBreak);
            codeLabel.text = codeLabel.text.Remove(codeLabel.text.Length - 2);
            yield return new WaitForSeconds(blinkingBreak);
        }
    }
    //
    // IEnumerator PrintRating(VisualElement root, float bestTime, int bestTakedowns, float bestDamageTaken)
    // {
    //     float pauseBetweenStars = 0.5f;
    //
    //     root.Q<Label>("TimeBest").text = (bestTime > 0) ? "Best: " + bestTime.ToString("0:00") : "No records";
    //     root.Q<Label>("TakedownsBest").text = (bestTakedowns > 0) ? "Best: " + bestTakedowns.ToString() : "No records";
    //     root.Q<Label>("DamageTakenBest").text =
    //         (bestDamageTaken > 0) ? "Best: " + bestDamageTaken.ToString("0.00") : "No records";
    //
    //     yield return new WaitForSeconds(pauseBetweenStars);
    //
    //     var timeStars = root.Q<VisualElement>("TimeRate").Query<VisualElement>("Star").ToList();
    //     var takedownsStars = root.Q<VisualElement>("TakedownsRate").Query<VisualElement>("Star").ToList();
    //     var damageStars = root.Q<VisualElement>("DamageTakenRate").Query<VisualElement>("Star").ToList();
    //
    //     if (bestTime <= SelectedMission.OneStarTime && timeStars.Count > 0 && bestTime > 0)
    //     {
    //         timeStars[0].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     if (bestTakedowns >= SelectedMission.OneStarTakedowns && takedownsStars.Count > 0 && bestTakedowns > 0)
    //     {
    //         takedownsStars[0].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     if (bestDamageTaken <= SelectedMission.OneStarDamageTaken && damageStars.Count > 0 && bestDamageTaken > 0)
    //     {
    //         damageStars[0].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     yield return new WaitForSeconds(pauseBetweenStars);
    //
    //     if (bestTime <= SelectedMission.TwoStarTime && timeStars.Count > 1 && bestTime > 0)
    //     {
    //         timeStars[1].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     if (bestTakedowns >= SelectedMission.TwoStarTakedowns && takedownsStars.Count > 1 && bestTakedowns > 0)
    //     {
    //         takedownsStars[1].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     if (bestDamageTaken <= SelectedMission.TwoStarDamageTaken && damageStars.Count > 1 && bestTakedowns > 0)
    //     {
    //         damageStars[1].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     yield return new WaitForSeconds(pauseBetweenStars);
    //
    //     if (bestTime <= SelectedMission.ThreeStarTime && timeStars.Count > 2 && bestTime > 0)
    //     {
    //         timeStars[2].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     if (bestTakedowns >= SelectedMission.ThreeStarTakedowns && takedownsStars.Count > 2 && bestTakedowns > 0)
    //     {
    //         takedownsStars[2].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    //
    //     if (bestDamageTaken <= SelectedMission.ThreeStarDamageTaken && damageStars.Count > 2 && bestDamageTaken > 0)
    //     {
    //         damageStars[2].style.backgroundImage = new StyleBackground(StarImage);
    //     }
    // }
}
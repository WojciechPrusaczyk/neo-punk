using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Enums;

public class DialogScript : MonoBehaviour
{
    public List<DialogData> dialogs = new List<DialogData>();
    [SerializeField] private DialogData dialogData = null;

    private Player player;

    [SerializeField] private VisualElement root;
    private VisualElement characterOnePictureElement;
    private VisualElement characterTwoPictureElement;
    private Label dialogTextElement;
    private Label characterOneNameElement;
    private Label characterTwoNameElement;
    private Coroutine typingCoroutine;
    
    private int currentLineIndex = 0;

    private UserInterfaceController userInterfaceController;
    public GameObject mainUserInterfaceControllerObject;

    public List<MissionInfo> missionQueue;

    private void Awake()
    {
        mainUserInterfaceControllerObject = GameObject.Find("MainUserInterfaceRoot").gameObject;
        userInterfaceController = mainUserInterfaceControllerObject.GetComponent<UserInterfaceController>();

        player = FindFirstObjectByType<Player>();
    }

    private void OnEnable()
    {
        gameObject.SetActive(true);
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("ERROR DIALOG! NIE MA UIDOCUMENT");
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("ERROR DIALOG! NIE MA ROOT VISUALELEMENT");
        }


        LoadUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (typingCoroutine != null && dialogTextElement.text != dialogData.lines[currentLineIndex].text)
            {
                StopCoroutine(typingCoroutine);
                dialogTextElement.text = dialogData.lines[currentLineIndex].text;
            }
            else
            {
                NextLine();
            }
        }
    }

    private void LoadUI()
    {
        characterOnePictureElement = root.Q<VisualElement>("CharacterOnePicture");
        characterTwoPictureElement = root.Q<VisualElement>("CharacterTwoPicture");
        characterOneNameElement = root.Q<Label>("CharacterName1");
        characterTwoNameElement = root.Q<Label>("CharacterName2");
        dialogTextElement = root.Q<Label>("DialogText");

        if (characterOnePictureElement == null ||
            characterTwoPictureElement == null ||
            characterOneNameElement == null ||
            characterTwoNameElement == null ||
            dialogTextElement == null)
        {
            Debug.LogError("Error, brakuje ktoregos z elementow UI Dialogow.");
            this.enabled = false;
            return;
        }

        if (dialogData == null)
        {
            characterOneNameElement.text = "";
            characterTwoNameElement.text = "";
            characterOnePictureElement.style.backgroundImage = null;
            characterTwoPictureElement.style.backgroundImage = null;
        } else
        {
            characterOneNameElement.text = dialogData.characterOneName.ToString();
            characterTwoNameElement.text = dialogData.characterTwoName.ToString();

            characterOnePictureElement.style.backgroundImage = new StyleBackground(dialogData.characterOnePicture);
            characterTwoPictureElement.style.backgroundImage = new StyleBackground(dialogData.characterTwoPicture);
        }
    }

    //Funkcja do ustawiania osoby ktora teraz mowi
    //Pierwszy element(obrazek) osoby mowiacej sie podswietla, drugi element(obrazek), element(text) osoby mowiacej sie podswietla, drugi element(text) 
    private void SetActiveCharacter(VisualElement photoActiveElement, VisualElement photoInactiveElement, VisualElement textActiveElement, VisualElement textInactiveElement)
    {
        if (characterOnePictureElement == null || characterTwoPictureElement == null ||
            characterOneNameElement == null || characterTwoNameElement == null || dialogTextElement == null)
        {
            Debug.LogError("ERROR DIALOG! Nie znaleziono elementów UI w hierarchii.");
            return;
        }
        
        // Reset klas
        photoActiveElement.RemoveFromClassList("picture_inactive");
        photoActiveElement.AddToClassList("picture_active");

        photoInactiveElement.RemoveFromClassList("picture_active");
        photoInactiveElement.AddToClassList("picture_inactive");

        textActiveElement.RemoveFromClassList("character_name_inactive");
        textActiveElement.AddToClassList("character_name_active");

        textInactiveElement.RemoveFromClassList("character_name_active");
        textInactiveElement.AddToClassList("character_name_inactive");

        // Dodanie aktywnych/nieaktywnych klas
        photoActiveElement.AddToClassList("picture_active");
        photoInactiveElement.AddToClassList("picture_inactive");

        textActiveElement.AddToClassList("character_name_active");
        textInactiveElement.AddToClassList("character_name_inactive");
        
    }

    //Włączenie dialogu
    public void StartDialog(int dialogIndex)
    {

        if (player == null)
        {
            Debug.LogError("Player was not found, cannot proceed with dialogues.");
            return;
        }

        if (player.isInDialogue)
            return;

        if (dialogs[dialogIndex] && dialogs[dialogIndex].lines == null || dialogs[dialogIndex].lines.Count == 0)
        {
            Debug.LogWarning("ERROR DIALOG! NIE MA TEKSTU");
            return;
        }

        if ( dialogs[dialogIndex] && !dialogs[dialogIndex].repeatable && dialogs[dialogIndex].hasBeenAlreadySeen)
        {
            Debug.LogWarning("ERROR DIALOG! HAS BEEN ALREADY SEEN");
            return;
        }

        if (dialogIndex >= 0 && dialogIndex < dialogs.Count)
        {
            dialogData = dialogs[dialogIndex];
        }
        else
        {
            return;
        }

        player.isInDialogue = true;

        currentLineIndex = 0;

        userInterfaceController.ActivateInterface(InterfaceType.DialogueInterface);
        ShowLine(0);
    }
    
    private void ShowLine(int lineIndex)
    {
        if (lineIndex < 0 || lineIndex >= dialogData.lines.Count)
        {
            return;
        }
        
        var line = dialogData.lines[lineIndex];
        
        dialogTextElement.text = line.text;

        if (null != typingCoroutine)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TextAnimation(line.text));
        
        if (line.speakerName == Talkers.CharacterOne)
        {
            SetActiveCharacter(characterOnePictureElement, characterTwoPictureElement, characterOneNameElement, characterTwoNameElement);
        }
        else if (line.speakerName == Talkers.CharacterTwo)
        {
            SetActiveCharacter(characterTwoPictureElement, characterOnePictureElement, characterTwoNameElement, characterOneNameElement);
        }
        else
        {
            Debug.LogWarning("ERROR DIALOG! SHOWLINE");
        }
    }

    //Funkcja nastepnej opcji dialogowej
    public void NextLine()
    {
        currentLineIndex++;
        
        if (currentLineIndex >= dialogData.lines.Count)
        {
            EndDialog();
            return;
        }
        ShowLine(currentLineIndex);
    }
    
    //Funkcja animacji pojawiania sie liter
    private IEnumerator TextAnimation(string textAnimation)
    {
        dialogTextElement.text = "";
        foreach (char letter in textAnimation)
        {
            dialogTextElement.text += letter;
            yield return new WaitForSecondsRealtime(0.05f); //Tutaj zwieksza sie predkosc wyswietlanego tekstu
        }
    }

    //Funkcja zamykania UI
    private void EndDialog()
    {
        StartCoroutine(WaitForEndOfFrameThenEndDialogue());
    }

    private IEnumerator WaitForEndOfFrameThenEndDialogue()
    {
        yield return new WaitForEndOfFrame();
        if (player == null)
        {
            Debug.LogError("Player was not found, cannot proceed with ending dialogues.");
            yield break;
        }

        player.isInDialogue = false;

        userInterfaceController.ActivateInterface(InterfaceType.MainUserInterface);

        dialogData.hasBeenAlreadySeen = true;
        dialogData = null;

        AddAllQueuedMissionsToPlayer();
    }

    public void AddMissionToQueue(MissionInfo mission)
    {
        if (mission == null)
            return;

        if (missionQueue == null)
            return;

        if (missionQueue.Contains(mission))
            return;

        missionQueue.Add(mission);
    }

    public void ClearMissionQueue()
    {
        if (missionQueue == null)
            return;

        missionQueue.Clear();
    }

    public void AddAllQueuedMissionsToPlayer()
    {
        if (missionQueue == null || missionQueue.Count == 0)
            return;

        foreach (var mission in missionQueue)
        {
            if (mission == null)
                continue;
            PlayerObjectiveTracker.instance.AddNewMission(mission);
        }
        ClearMissionQueue();
    }
}

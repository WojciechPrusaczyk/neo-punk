using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogScript : MonoBehaviour
{
    public List<DialogData> dialogs = new List<DialogData>();
    [SerializeField] private DialogData dialogData = null;

    private VisualElement root;
    private VisualElement characterOnePictureElement;
    private VisualElement characterTwoPictureElement;
    private Label dialogTextElement;
    private Label characterOneNameElement;
    private Label characterTwoNameElement;
    private Coroutine typingCoroutine;
    
    private int currentLineIndex = 0;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("ERROR DIALOG! NIE MA UIDOCUMENT");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("ERROR DIALOG! NIE MA ROOT VISUALELEMENT");
            return;
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

    public object Assets { get; set; }

    private void LoadUI()
    {
        characterOnePictureElement = root.Q<VisualElement>("CharacterOnePicture");
        characterTwoPictureElement = root.Q<VisualElement>("CharacterTwoPicture");
        characterOneNameElement = root.Q<Label>("CharacterName1");
        characterTwoNameElement = root.Q<Label>("CharacterName2");
        dialogTextElement = root.Q<Label>("DialogText");
        
        
        characterOneNameElement.text = dialogData.characterOneName.ToString();
        characterTwoNameElement.text = dialogData.characterTwoName.ToString();

        characterOnePictureElement.style.backgroundImage = new StyleBackground(dialogData.characterOnePicture);
        characterTwoPictureElement.style.backgroundImage = new StyleBackground(dialogData.characterTwoPicture);
        
        //Check
        if (characterOnePictureElement == null || characterTwoPictureElement == null ||
            characterOneNameElement == null || characterTwoNameElement == null || dialogTextElement == null)
        {
            Debug.LogError("ERROR DIALOG! NIE MA ELEMENTOW UI");
            return;
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
        
        photoActiveElement.ClearClassList();
        textActiveElement.ClearClassList();
        textInactiveElement.ClearClassList();
        photoInactiveElement.ClearClassList();
        
        photoActiveElement.AddToClassList("picture_active");
        textActiveElement.AddToClassList("character_name_active");

        
        photoInactiveElement.AddToClassList("picture_inactive");
        textInactiveElement.AddToClassList("character_name_inactive");
        
    }

    //Włączenie dialogu
    private void StartDialog(int dialogIndex)
    {
        if (dialogIndex >= 0 && dialogIndex < dialogs.Count)
        {
            dialogData = dialogs[dialogIndex];
        }
        else
        {
            return;
        }
        if (dialogData.lines == null || dialogData.lines.Count == 0)
        {
            Debug.LogWarning("ERROR DIALOG! NIE MA TEKSTU");
            dialogData = null;
            return;
        }

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
            SetActiveCharacter(characterTwoPictureElement, characterOnePictureElement, characterTwoNameElement, characterTwoNameElement);
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
                    yield return new WaitForSeconds(0.05f); //Tutaj zwieksza sie predkosc wyswietlanego tekstu
        }
    }
    
    //Funkcja zamykania UI
    private void EndDialog()
    {
        dialogData = null;
        var uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument != null)
        {
            uiDocument.enabled = false; // Wyłączenie UIDocument
        }
        Debug.Log("Koniec dialogu");
    }
}

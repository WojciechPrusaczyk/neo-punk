using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [TextArea]
    public string[] introTexts;

    public float textSpeed = 0.05f;
    public string sceneToLoad;

    private Animator animator;
    private TMP_Text introTextComponent;
    private Coroutine typingCoroutine;
    private int currentSceneIndex = 0;
    private bool canAdvance = false;
    private bool skipRequested = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        introTextComponent = GetComponentInChildren<TMP_Text>(true);
    }

    private void Start()
    {
        ShowCurrentSceneText();
    }

    private void Update()
    {
        if (skipRequested) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            skipRequested = true;
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
        {
            if (canAdvance)
            {
                NextScene();
            }
            else
            {
                CompleteTextImmediately();
            }
        }
    }

    private void ShowCurrentSceneText()
    {
        introTextComponent.text = "";
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        if (currentSceneIndex < introTexts.Length && !string.IsNullOrEmpty(introTexts[currentSceneIndex]))
        {
            typingCoroutine = StartCoroutine(TypeText(introTexts[currentSceneIndex]));
        }
        else
        {
            canAdvance = true;
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        canAdvance = false;
        foreach (char c in fullText)
        {
            introTextComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        canAdvance = true;
    }

    private void CompleteTextImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            introTextComponent.text = introTexts[currentSceneIndex];
            canAdvance = true;
        }
    }

    public void NextScene()
    {
        currentSceneIndex++;
        if (currentSceneIndex >= introTexts.Length)
        {
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        animator.SetTrigger("NextScene");
        ShowCurrentSceneText();
    }
}
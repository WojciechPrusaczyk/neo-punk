using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exitSceneGate : MonoBehaviour
{
    public bool open;
    private bool previousOpen;

    public string nextSceneName;
    private Animator animator;
    private bool playerInTrigger = false;
    private GameObject tooltipButton;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        tooltipButton = transform.Find("tooltipButton")?.gameObject;
        if (tooltipButton != null) tooltipButton.SetActive(false);
    }

    private void Update()
    {
        if (open != previousOpen)
        {
            if (open)
                animator.SetTrigger("open");
            else
                animator.SetTrigger("close");

            previousOpen = open;
        }

        // Obsługa przejścia sceny
        if (playerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
            else
                Debug.LogWarning("Nie przypisano nazwy sceny do przejścia!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            animator.SetTrigger("open");
            if (tooltipButton != null) tooltipButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            animator.SetTrigger("close");
            if (tooltipButton != null) tooltipButton.SetActive(false);
        }
    }
}
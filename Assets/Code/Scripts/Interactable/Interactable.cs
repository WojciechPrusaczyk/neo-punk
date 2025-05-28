using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public Collider2D interactableCollider;
    public float colliderDisableTimer = 2f;
    
    [SerializeField] protected float interactableIconYOffset = 1.5f;

    [SerializeField] protected GameObject InteractIcon;
    [SerializeField] protected GameObject instantiatedIcon;

    protected Animator animator;

    protected bool isPlayerInRange = false;

    protected virtual void Awake()
    {
        interactableCollider = GetComponent<Collider2D>();
        if (interactableCollider == null)
        {
            Debug.LogError("Do działania systemu interakcji wymagany jest Collider2D.");
        }
        animator = GetComponent<Animator>();

        PrepareInteractable();
    }

    private void Update()
    {
        // Sprawdzanie, czy gracz jest w zasięgu interakcji i czy naciśnięto klawisz interakcji
        if (isPlayerInRange && (Input.GetKeyDown(InputManager.InteractKey) || Input.GetKeyDown(InputManager.PadButtonInteract)))
        {
            Interact();
        }
        else if (Input.GetKeyDown(InputManager.PauseMenuKey) || Input.GetKeyDown(InputManager.PadButtonPauseMenu))
        {
            CloseUI();
        }
    }

    // Tworzenie ikony interakcji
    protected void InstantiateInteractionIcon(GameObject iconPrefab, Vector3 position, float yOffset = 1.5f)
    {
        instantiatedIcon = Instantiate(iconPrefab, new Vector3(position.x, position.y + yOffset, position.z), Quaternion.identity);
        instantiatedIcon.transform.SetParent(transform);
    }

    protected virtual void CloseUI()
    {
        Debug.Log("Closing Interactable UI");
    }

    // Tutaj logika jest pusta, ponieważ każdy obiekt będzie miał swoją własną logikę.
    // To jest klasa bazowa, więc nie ma tu żadnej implementacji.
    protected virtual void Interact()
    {
        if (interactableCollider != null)
        {
            interactableCollider.enabled = false;
            StartCoroutine(RestoreColliderAfterDelay(colliderDisableTimer));
        }
    }

    protected IEnumerator RestoreColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (interactableCollider != null)
        {
            interactableCollider.enabled = true;
        }
    }

    // Przygotowanie obiektu podczas ładowania pliku zapisu.
    // Podobnie jak w przypadku Interact(), każdy obiekt będzie miał swoją własną logikę.
    protected virtual void PrepareInteractable()
    {
        if (WorldSaveGameManager.instance == null)
            return;
    }

    protected virtual void CloseUIOnExit()
    {
        // Domyślna implementacja nie robi nic, ale może być nadpisana w klasach dziedziczących
        Debug.Log("Left the Interactable area, closing UI");
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        PrepareTriggerEnterPlayer(collision);
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        PrepareTriggerExitPlayer(collision);
    }

    protected virtual void PrepareTriggerEnterPlayer(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            animator.SetBool("IsPlayerNearby", true);

            // Wyświetl ikonę interakcji
            if (InteractIcon != null)
            {
                Vector3 positionAboveCampfire = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
                InstantiateInteractionIcon(InteractIcon, positionAboveCampfire);
            }
        }
    }

    protected virtual void PrepareTriggerExitPlayer(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            animator.SetBool("IsPlayerNearby", false);
            CloseUIOnExit();

            // Ukryj ikonę interakcji
            if (instantiatedIcon != null)
            {
                Destroy(instantiatedIcon);
                instantiatedIcon = null;
            }
        }
    }

    protected virtual void OnDestroy()
    {
    }
}

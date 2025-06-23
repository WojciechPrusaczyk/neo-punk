using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public Collider2D interactableCollider;

    // Interaction
    [SerializeField] protected bool isInteractionOnCooldown = false;
    protected float interactionCooldownDuration = .5f;

    [SerializeField] protected float interactableIconYOffset = 1.5f;

    [SerializeField] protected GameObject InteractIcon;
    [SerializeField] protected GameObject instantiatedIcon;

    protected Animator animator;

    [SerializeField] protected bool isPlayerInRange = false;

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

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
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
    protected void InstantiateInteractionIcon(GameObject iconPrefab, Vector3 position)
    {
        instantiatedIcon = Instantiate(iconPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
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
        if (isInteractionOnCooldown)
            return;

        RemoveIcon();

        StartCoroutine(InteractionCooldown());
    }
    protected IEnumerator InteractionCooldown()
    {
        isInteractionOnCooldown = true;
        yield return new WaitForSeconds(interactionCooldownDuration);
        isInteractionOnCooldown = false;
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
        if (instantiatedIcon == null)
            CreateIcon(transform);
    }

    protected virtual void RemoveIcon()
    {
        if (instantiatedIcon != null)
        {
            Destroy(instantiatedIcon);
            instantiatedIcon = null;
        }
    }

    protected virtual void CreateIcon(Transform _transform)
    {
        if (InteractIcon != null)
        {
            Vector3 positionAbove = new Vector3(_transform.position.x, _transform.position.y + interactableIconYOffset, _transform.position.z);
            InstantiateInteractionIcon(InteractIcon, positionAbove);
        }
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

            if (instantiatedIcon == null)
                CreateIcon(transform);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
    public Collider2D interactableCollider;
    [SerializeField] GameObject InteractIcon;
    [SerializeField] GameObject instantiatedIcon;

    protected bool isPlayerInRange = false;

    private void Awake()
    {
        interactableCollider = GetComponent<Collider2D>();
        if (interactableCollider == null)
        {
            Debug.LogError("Do dzia�ania systemu interakcji wymagany jest Collider2D.");
        }

        PrepareInteractable();
    }

    private void Update()
    {
        // Sprawdzanie, czy gracz jest w zasi�gu interakcji i czy naci�ni�to klawisz interakcji
        if (isPlayerInRange && (Input.GetKeyDown(InputManager.InteractKey) || Input.GetKeyDown(InputManager.PadButtonInteract)))
        {
            Interact();
        }
    }

    // Tworzenie ikony interakcji
    protected void InstantiateInteractionIcon(GameObject iconPrefab, Vector3 position)
    {
        instantiatedIcon = Instantiate(iconPrefab, position, Quaternion.identity);
        instantiatedIcon.transform.SetParent(transform);
    }

    // Tutaj logika jest pusta, poniewa� ka�dy obiekt b�dzie mia� swoj� w�asn� logik�.
    // To jest klasa bazowa, wi�c nie ma tu �adnej implementacji.
    protected virtual void Interact()
    {
    }

    // Przygotowanie obiektu podczas �adowania pliku zapisu.
    // Podobnie jak w przypadku Interact(), ka�dy obiekt b�dzie mia� swoj� w�asn� logik�.
    protected virtual void PrepareInteractable()
    {
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Wy�wietl ikon� interakcji
            if (InteractIcon != null)
            {
                Vector3 positionAboveCampfire = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
                InstantiateInteractionIcon(InteractIcon, positionAboveCampfire);
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Ukryj ikon� interakcji
            if (instantiatedIcon != null)
            {
                Destroy(instantiatedIcon);
                instantiatedIcon = null;
            }
        }
    }
}

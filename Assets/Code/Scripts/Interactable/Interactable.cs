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
            Debug.LogError("Do dzia³ania systemu interakcji wymagany jest Collider2D.");
        }

        PrepareInteractable();
    }

    private void Update()
    {
        // Sprawdzanie, czy gracz jest w zasiêgu interakcji i czy naciœniêto klawisz interakcji
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

    // Tutaj logika jest pusta, poniewa¿ ka¿dy obiekt bêdzie mia³ swoj¹ w³asn¹ logikê.
    // To jest klasa bazowa, wiêc nie ma tu ¿adnej implementacji.
    protected virtual void Interact()
    {
    }

    // Przygotowanie obiektu podczas ³adowania pliku zapisu.
    // Podobnie jak w przypadku Interact(), ka¿dy obiekt bêdzie mia³ swoj¹ w³asn¹ logikê.
    protected virtual void PrepareInteractable()
    {
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Wyœwietl ikonê interakcji
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

            // Ukryj ikonê interakcji
            if (instantiatedIcon != null)
            {
                Destroy(instantiatedIcon);
                instantiatedIcon = null;
            }
        }
    }
}

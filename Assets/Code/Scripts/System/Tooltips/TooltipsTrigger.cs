using Tooltips;
using UnityEngine;
using UnityEngine.Events;

public class TooltipTrigger : MonoBehaviour
{
    [Tooltip("Wybierz metodę, która ma być wywołana przy wejściu w strefę.")]
    [SerializeField]
    private UnityEvent onTriggerEnter;
    
    private TooltipsController tooltipsController;
    private void Awake()
    {
        tooltipsController = GameObject.Find("UserInterface").transform.Find("Tooltips")
            .GetComponent<TooltipsController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onTriggerEnter.Invoke();
            gameObject.SetActive(false);
        }
    }
}
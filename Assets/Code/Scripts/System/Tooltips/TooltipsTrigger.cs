using Tooltips;
using UnityEngine;
using UnityEngine.Events;

public class TooltipTrigger : MonoBehaviour
{
    public int tooltipIndex;
    private TooltipsController tooltipsController;
    private void Awake()
    {
        tooltipsController = GameObject.Find("MainUserInterfaceRoot")?.GetComponentInChildren<TooltipsController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tooltipsController.ShowTooltip(tooltipIndex);
            gameObject.SetActive(false);
        }
    }
}
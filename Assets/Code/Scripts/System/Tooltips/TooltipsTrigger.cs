
using System.Linq;
using Tooltips;
using UnityEngine;
using UnityEngine.Events;

public class TooltipTrigger : MonoBehaviour
{
    public int tooltipIndex;
    public bool hasBeenTriggered = false;
    private TooltipsController tooltipsController;
    private void Awake()
    {
        tooltipsController = GameObject.Find("MainUserInterfaceRoot").transform.Find("Tooltips").GetComponent<TooltipsController>();

        if (WorldSaveGameManager.instance.currentCharacterData.tutorialTexts.ContainsKey(tooltipIndex))
        {
            hasBeenTriggered = WorldSaveGameManager.instance.currentCharacterData.tutorialTexts[tooltipIndex];
            gameObject.SetActive(false);
        }
        else
        {
            hasBeenTriggered = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tooltipsController.ShowTooltip(tooltipIndex);
            hasBeenTriggered = true;

            if (!WorldSaveGameManager.instance.currentCharacterData.tutorialTexts.ContainsKey(tooltipIndex))
            {
                WorldSaveGameManager.instance.currentCharacterData.tutorialTexts.Add(tooltipIndex, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.tutorialTexts[tooltipIndex] = true;
            }
            gameObject.SetActive(false);
        }
    }
}
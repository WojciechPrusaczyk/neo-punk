using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Tooltips
{
    
    [System.Serializable]
    public class Tooltip
    {
        public Sprite image = null;
        
        [TextArea(3, 20)]
        public String text = null;
        public bool wasTooltipShown = false;
    }

    public class TooltipsController : MonoBehaviour
    {
        public List<Tooltip> tooltips = new List<Tooltip>();
        
        private bool IsTooltipMenuShown;
        private Tooltip shownTooltip = null;
        public Image imageObject = null;
        public TextMeshProUGUI textObject = null;
        private UserInterfaceController uiController = null;

        private void Awake()
        {
            gameObject.SetActive(false);
            uiController = GetComponentInParent<UserInterfaceController>();
        }

        private void Update()
        {

            if (IsTooltipMenuShown && null != shownTooltip)
            {
                Time.timeScale = 0;

                // closing tooltip
                if (Input.GetKeyDown(InputManager.InteractKey) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(InputManager.PadButtonInteract))
                {
                    CloseTooltip();
                    Time.timeScale = 1;
                }
            }
        }

        public void ShowTooltip(int tooltipNumber)
        {
            if (OptionsManager.GetShowTips())
            {
                if (tooltipNumber < tooltips.Count && !tooltips[tooltipNumber].wasTooltipShown)
                {
                    shownTooltip = tooltips[tooltipNumber];
                    imageObject.sprite = shownTooltip.image;
                    textObject.text = shownTooltip.text;
                    IsTooltipMenuShown = true;
                    uiController.ActivateInterface(gameObject);
                    shownTooltip.wasTooltipShown = true;
                }
                else
                {
                    Debug.Log("Tooltip with provided index does not exist, or was already shown.");
                }
            }
        }

        public void CloseTooltip()
        {
            uiController.ActivateInterface(uiController.defaultInterface);
            shownTooltip = null;
            imageObject.sprite = null;
            textObject.text = "";
            Time.timeScale = 1;
            IsTooltipMenuShown = false;
        }
    }
}

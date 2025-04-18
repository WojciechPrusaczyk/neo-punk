using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsHandler : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    private GameObject MainUi;
    private List<TextMeshProUGUI> itemsCooldowns = new List<TextMeshProUGUI>();
    private PlayerInventory playerInventory;

    private void Start()
    {
        // Ustawianie pustych przedmiotów na starcie gry
        for (int i = 0; i < 4; i++)
        {
            items.Add(null); // Puste sloty na przedmioty
        }

        MainUi = GameObject.Find("Main User Interface");
        GameObject itemsCooldownsParent = MainUi.transform.Find("ItemsCooldowns").gameObject;

        for (int i = 0; i < 4; i++)
        {
            GameObject itemCooldownObject = itemsCooldownsParent.transform.GetChild(i).gameObject;
            TextMeshProUGUI itemCooldownTextComponent = itemCooldownObject.GetComponent<TextMeshProUGUI>();

            itemsCooldowns.Add(itemCooldownTextComponent);
        }

        playerInventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (null != items[0])
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
            UsePassive(0);
            UpdateCooldownTimer(0);
        }

        if (null != items[1])
        {
            if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
            UsePassive(1);
            UpdateCooldownTimer(1);
        }

        if (null != items[2])
        {
            if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
            UsePassive(2);
            UpdateCooldownTimer(2);
        }

        if (null != items[3])
        {
            if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3);
            UsePassive(3);
            UpdateCooldownTimer(3);
        }
    }

    public void AddItem(ItemData itemData, GameObject objectToDelete)
    {
        if (playerInventory == null)
        {
            Debug.LogError("Nie znaleziono PlayerInventory");
            return;
        }

        playerInventory.isPlayerPickingItem = true;
        StartCoroutine(WaitForAction(itemData, objectToDelete));
    }

    private IEnumerator WaitForAction(ItemData itemData, GameObject pickedObject)
    {
        playerInventory.ShowEquipment();
        while (playerInventory.isEquipmentShown)
        {
            // Wyświetlenie podnoszonego przedmiotu na UI
            playerInventory.PickupItem(itemData);

            if (Input.GetKeyDown(InputManager.InteractKey))
            {
                ItemData currentItem = items[playerInventory.selectedItemIndex];

                if (currentItem != null && currentItem.itemName == itemData.itemName)
                {
                    Debug.Log("Istnieje już taki item w ekwipunku");
                }
                else
                {
                    items[playerInventory.selectedItemIndex] = null;
                    items[playerInventory.selectedItemIndex] = itemData;
                    playerInventory.SetImageAtSlot(itemData);

                    playerInventory.EndPickingItem();
                    Destroy(pickedObject.gameObject.transform.parent.gameObject);
                    playerInventory.HideEquipment();
                    playerInventory.isPlayerPickingItem = false;
                }
            }

            yield return null;
        }
    }

    public void UseItem(int itemPos)
    {
        ItemData usedItem = items[itemPos];
        if (usedItem != null && usedItem.currentCooldown <= 0)
        {
            usedItem.itemAbility.Use();
            usedItem.currentCooldown = usedItem.cooldown;
            StartCoroutine(CooldownTimer(usedItem, itemPos));

            // Wyczernienie przedmiotu
            try
            {
                Image itemImage = MainUi.transform.Find("Items").transform.GetChild(itemPos).GetComponent<Image>();
                itemImage.color = new Color32(55, 55, 55, 255);
            }
            catch (Exception)
            {
            }
        }
    }

    private IEnumerator CooldownTimer(ItemData item, int itemPos)
    {
        while (item.currentCooldown > 0)
        {
            yield return new WaitForSeconds(1.0f);
            item.currentCooldown -= 1.0f;
        }

        item.currentCooldown = 0;

        try
        {
            Image itemImage = MainUi.transform.Find("Items").transform.GetChild(itemPos).GetComponent<Image>();
            itemImage.color = Color.white;
        }
        catch (Exception)
        {
        }
    }

    public void UsePassive(int itemPos)
    {
        ItemData usedItem = items[itemPos];
        if (null != usedItem)
        {
            usedItem.itemAbility.Apply();
        }
    }

    public void OnItemDisband(int itemPos)
    {
        ItemData usedItem = items[itemPos];
        usedItem.itemAbility.Remove();
    }

    // Metoda aktualizująca timer na UI
    private void UpdateCooldownTimer(int itemPos)
    {
        ItemData item = items[itemPos];
        if (item != null && item.currentCooldown > 0)
        {
            itemsCooldowns[itemPos].text = item.currentCooldown.ToString();
        }
        else
        {
            itemsCooldowns[itemPos].text = "";
        }
    }
}
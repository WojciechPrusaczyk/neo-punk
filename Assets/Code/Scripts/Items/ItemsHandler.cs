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
    private MainUserInterfaceController MainUiController;
    private List<TextMeshProUGUI> itemsCooldowns = new List<TextMeshProUGUI>();
    private PlayerInventoryInterface playerInventory;

    private void Awake()
    {
        // Ustawianie pustych przedmiotów na starcie gry
        for (int i = 0; i < 4; i++)
        {
            items.Add(null); // Puste sloty na przedmioty
        }

        MainUi = GameObject.Find("MainUserInterfaceRoot").transform.Find("MainUserInterface").gameObject;
        MainUiController = MainUi.GetComponent<MainUserInterfaceController>();
        playerInventory = GameObject.Find("MainUserInterfaceRoot").transform.Find("EquipmentInterface").GetComponent<PlayerInventoryInterface>();
    }

    private void Update()
    {
        if (null != items[0])
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
            UsePassive(0);
            MainUiController.SetItemCooldownAtSlot(0);
        }


        if (null != items[1])
        {
            if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
            UsePassive(1);
            MainUiController.SetItemCooldownAtSlot(1);
        }

        if (null != items[2])
        {
            if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
            UsePassive(2);
            MainUiController.SetItemCooldownAtSlot(2);
        }

        if (null != items[3])
        {
            if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3);
            UsePassive(3);
            MainUiController.SetItemCooldownAtSlot(3);
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

    public void AddItemsToPlayerInventoryInstantly(string name, int slotIndex)
    {
        if (playerInventory == null) return;

        ItemData itemData = ScriptableObjectManager.instance.GetItemData(name);
        if (itemData == null)
        {
            Debug.LogError($"ItemData with name {name} not found.");
            return;
        }
        bool duplicateInEq = false;
        for (int i = 0; i <= 3; i++)
        {
            ItemData currentItem = items[i];
            if (currentItem != null && currentItem.itemName == itemData.itemName)
            {
                Debug.LogError("Istnieje już taki item w ekwipunku");
                duplicateInEq = true;
                break;
            }
        }
        if (duplicateInEq) return;
        items[slotIndex] = itemData;
        playerInventory.SetImageAtSlot(itemData, slotIndex);
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

                bool duplicateInEq = false;
                for (int i = 0; i <= 3; i++)
                {
                    ItemData currentItem = items[i];

                    if (currentItem != null && currentItem.itemName == itemData.itemName)
                    {
                        Debug.LogError("Istnieje już taki item w ekwipunku");

                        duplicateInEq = true;
                        playerInventory.HideEquipment();
                        break;
                    }
                }

                if (duplicateInEq) break;

                items[playerInventory.selectedItemIndex] = null;
                items[playerInventory.selectedItemIndex] = itemData;
                playerInventory.SetImageAtSlot(itemData);

                playerInventory.EndPickingItem();
                Destroy(pickedObject.gameObject.transform.parent.gameObject);

                ItemObject pickedItemObject = pickedObject.TryGetComponent<ItemObject>(out var itemObject) ? itemObject : null;

                if (!WorldSaveGameManager.instance.currentCharacterData.itemsPickedUp.ContainsKey(itemObject.ItemId))
                {
                    WorldSaveGameManager.instance.currentCharacterData.itemsPickedUp.Add(itemObject.ItemId, true);
                }
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.itemsPickedUp[itemObject.ItemId] = true;
                }

                playerInventory.HideEquipment();
                playerInventory.isPlayerPickingItem = false;
            }

            yield return null;
        }

        playerInventory.EndPickingItem();
        playerInventory.HideEquipment();
    }

    public void UseItem(int itemPos)
    {
        ItemData usedItem = items[itemPos];
        if (usedItem != null && usedItem.currentCooldown <= 0)
        {
            usedItem.itemAbility.Use();
            usedItem.currentCooldown = usedItem.cooldown;
            StartCoroutine(CooldownTimer(usedItem, itemPos));
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
}
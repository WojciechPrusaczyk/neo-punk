using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class LootItem
{
    public int itemId;
    [Range(0, 100)] public float dropChance;
}

public class LootTable : MonoBehaviour
{
    [SerializeField] private List<LootItem> lootItems;

    private GameObject scriptableObjectManager;


    private void Start()
    {
        scriptableObjectManager = GameObject.Find("ScriptableObjectManager");
    }

    public void DropLoot()
    {
        if (lootItems == null || lootItems.Count == 0)
        {
            Debug.LogWarning("LootTable is empty or null!");
            return;
        }

        foreach (var lootItem in lootItems)
        {
            float roll = Random.Range(0f, 100f);

            if (roll <= lootItem.dropChance)
            {
                SpawnItem(lootItem.itemId);
                break;
            }
        }
    }

    private void SpawnItem(int itemId)
    {
        var itemData = scriptableObjectManager.GetComponent<ScriptableObjectManager>().GetItemData(itemId);

        if (itemData == null)
        {
            Debug.LogError($"Item with ID {itemId} not found in ScriptableObjectManager.");
            return;
        }

        GameObject itemPrefab = Resources.Load<GameObject>($"Items/Prefabs/ItemPrefab");

        if (itemPrefab == null)
        {
            Debug.LogError("ItemPrefab not found in Resources/Items/Prefabs.");
            return;
        }

        Vector3 position = transform.position;
        position.z = 5;
        GameObject spawnedItem = Instantiate(itemPrefab, position, Quaternion.identity);

        var itemComponent = spawnedItem.GetComponentInChildren<ItemObject>();
        itemComponent.ScriptableObjectManager = scriptableObjectManager;
        if (itemComponent != null)
        {
            itemComponent.ItemId = itemId;
        }
        else
        {
            Debug.LogError("No ItemComponent found on the spawned item's child.");
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    public static ScriptableObjectManager instance;

    public List<ItemData> itemDataList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public ItemData GetItemData(int id)
    {
        return itemDataList[id];
    }

    public ItemData GetItemData(string name)
    {
        foreach (var item in itemDataList)
        {
            if (item.itemName == name)
            {
                return item;
            }
        }
        Debug.LogError($"Item with name {name} not found.");
        return null;
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("CoroutineRunner");
                instance = obj.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(obj);
            }

            return instance;
        }
    }
}

public class ItemObject : MonoBehaviour
{
    public GameObject ScriptableObjectManager;
    public int ItemId;
    public ItemData itemData;
    public Color CommonColor;
    public Color RareColor;
    public Color QuantumColor;

    private SpriteRenderer spriteRenderer;
    private Light2D light2D;
    private ParticleSystem itemParticles;
    private GameObject tooltip;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        light2D = GetComponentInParent<Light2D>();
        itemParticles = GetComponentInChildren<ParticleSystem>();
        ScriptableObjectManager = GameObject.Find("ScriptableObjectManager");
        itemData = ScriptableObjectManager.GetComponent<ScriptableObjectManager>().GetItemData(ItemId);
        tooltip = gameObject.transform.parent.transform.Find("Tooltip").gameObject;

        if (itemData != null)
        {
            spriteRenderer.sprite = itemData.itemIcon;
            UpdateItemLightColor();
        }
        else
        {
            Debug.LogError("ItemData is not assigned!");
        }

        if (tooltip) tooltip.gameObject.SetActive(false);
    }

    private void UpdateItemLightColor()
    {
        var main = itemParticles.main;
        switch (itemData.rarity)
        {
            case Enums.ItemRarity.Common:
                light2D.color = CommonColor;
                main.startColor = CommonColor;
                break;
            case Enums.ItemRarity.Rare:
                light2D.color = RareColor;
                main.startColor = RareColor;
                break;
            case Enums.ItemRarity.Quantum:
                light2D.color = QuantumColor;
                main.startColor = QuantumColor;
                break;
            default:
                light2D.color = CommonColor;
                main.startColor = CommonColor;
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            tooltip.gameObject.SetActive(true);

            if (Input.GetKey(InputManager.InteractKey))
            {
                ItemsHandler itemsHandler = col.GetComponent<ItemsHandler>();
                if (itemsHandler != null && itemData != null)
                {
                    StartCoroutine(WaitForFrameThenAddItem(itemsHandler));
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitForFrameThenAddItem(ItemsHandler itemsHandler)
    {
        yield return new WaitForEndOfFrame();
        itemsHandler.AddItem(itemData, gameObject);
    }
}
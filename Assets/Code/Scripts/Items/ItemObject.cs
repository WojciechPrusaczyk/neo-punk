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
    private ParticleSystem dropAnimation;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        light2D = GetComponentInParent<Light2D>();
        itemParticles = gameObject.transform.parent.Find("Particles").GetComponentInChildren<ParticleSystem>();
        dropAnimation = gameObject.transform.parent.Find("DropAnimation").GetComponentInChildren<ParticleSystem>();
        itemData = ScriptableObjectManager.GetComponent<ScriptableObjectManager>().GetItemData(ItemId);

        if (itemData != null)
        {
            spriteRenderer.sprite = itemData.itemIcon;
            UpdateItemLightColor();
        }
        else
        {
            Debug.LogError("ItemData is not assigned!");
        }
    }

    private void UpdateItemLightColor()
    {
        var main = itemParticles.main;
        var dropAnimMain = dropAnimation.main;
        switch (itemData.rarity)
        {
            case "Common":
                light2D.color = CommonColor;
                main.startColor = CommonColor;
                dropAnimMain.startColor = CommonColor;
                break;
            case "Rare":
                light2D.color = RareColor;
                main.startColor = RareColor;
                dropAnimMain.startColor = RareColor;
                break;
            case "Quantum":
                light2D.color = QuantumColor;
                main.startColor = QuantumColor;
                dropAnimMain.startColor = QuantumColor;
                break;
            default:
                light2D.color = CommonColor;
                main.startColor = CommonColor;
                dropAnimMain.startColor = CommonColor;
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && Input.GetKey(InputManager.InteractKey))
        {
            ItemsHandler itemsHandler = col.GetComponent<ItemsHandler>();
            if (itemsHandler != null && itemData != null)
            {
                StartCoroutine(WaitForFrameThenAddItem(itemsHandler));
            }
        }
    }

    private IEnumerator WaitForFrameThenAddItem(ItemsHandler itemsHandler)
    {
        yield return new WaitForEndOfFrame();
        itemsHandler.AddItem(itemData, gameObject);
    }
}
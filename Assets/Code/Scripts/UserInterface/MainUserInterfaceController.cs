using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MainUserInterfaceController : MonoBehaviour
{

    private Label hpLabel;
    private VisualElement bossBarRoot;
    private Label bossBar;
    private Label bossName;

    private EntityStatus playerStatus;
    public EntityStatus BossStatus;
    private Boolean isBossBarShown = false;
    private GameObject bossHpBarObject;
    private UnityEngine.UI.Image bossHpBarImage;
    private List<VisualElement> _itemImages = new List<VisualElement>();
    private List<Label> _itemCooldowns = new List<Label>();
    [SerializeField] private VisualElement root;

    private void Awake()
    {
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerStatus = playerGameObject.GetComponent<EntityStatus>();
        bossHpBarObject = gameObject.transform.Find("BossHpBar").gameObject;

        if (bossHpBarObject)
            bossHpBarImage = bossHpBarObject.GetComponent<Image>();
    }

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("ERROR! NIE MA UIDOCUMENT");
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("ERROR! NIE MA ROOT VISUALELEMENT");
        }

        /*
         * Przyciski menu
         */
        hpLabel = root.Q<Label>("HpText");
        bossBarRoot = root.Q<VisualElement>("BossBarRoot");
        bossBar = root.Q<Label>("BossBarLabel");
        bossName = root.Q<Label>("BossBarName");


        _itemImages = new List<VisualElement>();
        _itemCooldowns = new List<Label>();
        for (int i = 1; i <= 4; i++)
        {
            var itemImage = root.Q<VisualElement>($"ItemImage{i}");
            var itemCooldown = root.Q<Label>($"ItemCooldown{i}");

            if (itemImage != null)
                _itemImages.Add(itemImage);

            if (itemCooldown != null)
                _itemCooldowns.Add(itemCooldown);
        }

        bossBarRoot.SetEnabled(false);
    }

    private void Update()
    {
        hpLabel.text = $"{playerStatus.entityMaxHelath} / {playerStatus.entityHealthPoints}";

        bossHpBarObject.SetActive(isBossBarShown);
        if (isBossBarShown && BossStatus)
        {
            bossHpBarObject.SetActive(true);
            bossBar.text = $"{BossStatus.entityMaxHelath} / {BossStatus.entityHealthPoints}";
            bossName.text = BossStatus.entityName;

            var fillPercentage = BossStatus.GetHp() / BossStatus.GetMaxHp();
            bossHpBarImage.fillAmount = fillPercentage;
        }

    }

    public void ShowBossBar(EntityStatus _BossStatus)
    {
        BossStatus = _BossStatus;
        bossBarRoot.SetEnabled(true);
        bossHpBarObject.SetActive(true);
        isBossBarShown = true;
    }

    public void HideBossBar()
    {
        isBossBarShown = false;
        bossName.text = "";
        bossBarRoot.SetEnabled(false);
        bossHpBarObject.SetActive(false);
        BossStatus = null;
    }

    public void SetItemCooldownAtSlot(int itemIndex, string itemCooldown = "")
    {
        _itemCooldowns[itemIndex].text = (itemCooldown == "") ? itemCooldown : "";
    }

    public void SetItemImageAtSlot(int itemIndex, Sprite itemImage = null)
    {
        var backgroundImage = ( null != itemImage ) ? new StyleBackground(itemImage) : null;

        _itemImages[itemIndex].style.backgroundImage = backgroundImage;
    }
}
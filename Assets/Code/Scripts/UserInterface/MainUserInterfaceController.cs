using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class MainUserInterfaceController : MonoBehaviour
{
    public float missionPopupTimeout;

    private Label hpLabel;
    private VisualElement bossBarRoot;
    private Label bossBar;
    private Label bossName;
    private VisualElement missionPopup;
    private Label missionTitle;
    private Label missionDescription;

    private VisualElement elementalIcon;
    private Label elementalLabel;

    private EntityStatus playerStatus;
    private ItemsHandler playerItemsHandler;
    private EntityStatus BossStatus;
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
        playerItemsHandler = playerGameObject.GetComponent<ItemsHandler>();
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

        /*
         * Elemental
         */
        elementalIcon = root.Q<VisualElement>("ElementalIcon");
        elementalLabel = root.Q<Label>("ElementalLabel");

        /*
         * Misje
         */
        missionPopup = root.Q<VisualElement>("MissionPopup");
        missionTitle = root.Q<Label>("MissionTitle");
        missionDescription = root.Q<Label>("MissionDescription");

        _itemImages = new List<VisualElement>();
        _itemCooldowns = new List<Label>();
        for (int i = 1; i <= 4; i++)
        {
            var itemImage = root.Q<VisualElement>($"ItemImage{i}");
            var itemCooldown = root.Q<Label>($"ItemCooldown{i}");

            if (itemImage != null)
            {
                _itemImages.Add(itemImage);

                if ( playerItemsHandler.items.Count > 0  && (null != playerItemsHandler.items[i-1]) )
                    itemImage.style.backgroundImage = new StyleBackground(playerItemsHandler.items[i-1].itemIcon);
            }

            if (itemCooldown != null)
                _itemCooldowns.Add(itemCooldown);
        }

        if (BossStatus == null || BossStatus.GetHp() <= 0)
            HideBossBar();
    }

    private void Update()
    {
        hpLabel.text = $"{playerStatus.entityHealthPoints.value:F1} / {playerStatus.entityMaxHealth.value:F1}";

        bossHpBarObject.SetActive(isBossBarShown);
        if (isBossBarShown && BossStatus)
        {
            bossHpBarObject.SetActive(true);
            bossBar.text = $"{BossStatus.entityHealthPoints.value:F1} / {BossStatus.entityMaxHealth.value:F1}";
            bossName.text = BossStatus.entityName;

            var fillPercentage = BossStatus.GetHp() / BossStatus.GetMaxHp();
            bossHpBarImage.fillAmount = fillPercentage;
        }
    }

    public void ShowBossBar(EntityStatus _BossStatus)
    {
        BossStatus = _BossStatus;
        bossBarRoot.style.display = DisplayStyle.Flex;
        bossHpBarObject.SetActive(true);
        isBossBarShown = true;
    }

    public void HideBossBar()
    {
        isBossBarShown = false;
        bossName.text = "";
        bossBarRoot.style.display = DisplayStyle.None;
        bossHpBarObject.SetActive(false);
        BossStatus = null;
    }

    public void SetItemCooldownAtSlot(int itemIndex)
    {
        var currentItem = playerItemsHandler.items[itemIndex];

        if ( null != currentItem && currentItem.currentCooldown <= currentItem.cooldown && currentItem.currentCooldown > 0 )
        {
            _itemCooldowns[itemIndex].text = currentItem.currentCooldown.ToString();
        }
        else if ( null != currentItem )
        {
            _itemCooldowns[itemIndex].text = "READY";
        }
        else
        {
            _itemCooldowns[itemIndex].text = "";
        }
    }

    public void SetItemImageAtSlot(int itemIndex, Sprite itemImage = null)
    {
        var backgroundImage = ( null != itemImage ) ? new StyleBackground(itemImage) : null;

        _itemImages[itemIndex].style.backgroundImage = backgroundImage;
    }

    public void ChangeElementalType(Sprite elementalSprite, string elementalName, Color elementalColor)
    {
        elementalIcon.style.backgroundImage = new StyleBackground(elementalSprite);

        elementalLabel.text = elementalName;
        elementalLabel.style.color = elementalColor;
    }

    public void SetCurrentMission(MissionInfo _mission)
    {
        if (_mission == null)
            return;

        StopAllCoroutines(); // na wypadek, gdyby wcześniejsze jeszcze trwały

        missionTitle.text = _mission.MissionName;
        missionDescription.text = _mission.MissionDescription;

        missionPopup.style.display = DisplayStyle.Flex;

        missionPopup.AddToClassList("showPopup");

        if (Time.timeScale == 0)
        {
            missionPopup.RemoveFromClassList("showPopup");
            missionPopup.style.display = DisplayStyle.None;
            return;
        }
        StartCoroutine(HideMissionPopupAfterDelay());
    }

    private IEnumerator HideMissionPopupAfterDelay()
    {
        yield return new WaitForSeconds(missionPopupTimeout);

        missionPopup.RemoveFromClassList("showPopup");
        missionPopup.style.display = DisplayStyle.None;
    }
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public bool isEquipmentShown = false;
    public bool isPlayerPickingItem = false;
    public bool isInspectingItems = false;
    public int selectedItemIndex = 0;
    public Sprite fieldImage;
    public Sprite selectedFieldImage;
    public Color secondaryEqColor;
    public Color secondaryItemsListColor;

    private GameObject MainUi;
    private GameObject InventoryUi;
    private GameObject fields;
    public GameObject selectedItemDesc;
    public GameObject incomingItemInfo;
    private GameObject arrowTooltip;
    private GameObject buttonTooltip;
    private ItemsHandler itemsHandler;
    private EntityStatus playerStatus;
    private float ItemsListInitialWidth;
    private Texture rawImage1;
    private Texture rawImage2;
    private Color primaryEqColor;
    private Color primaryItemsListColor;
    private bool isButtonDown = false;
    public bool isPickingItem = false;

    public void Start()
    {
        MainUi = GameObject.Find("Main User Interface");
        InventoryUi = GameObject.Find("Equipment Interface");
        selectedItemDesc = InventoryUi.transform.Find("SelectedItemInfo").gameObject;
        incomingItemInfo = InventoryUi.transform.Find("IncomingItemInfo").gameObject;
        arrowTooltip = InventoryUi.transform.Find("ArrowTooltip").gameObject;
        buttonTooltip = InventoryUi.transform.Find("ButtonTooltip").gameObject;
        fields = InventoryUi.transform.Find("ItemsFields").gameObject;
        itemsHandler = gameObject.GetComponent<ItemsHandler>();
        playerStatus = gameObject.GetComponent<EntityStatus>();
        ItemsListInitialWidth = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>().offsetMin.x;

        rawImage1 = InventoryUi.GetComponent<RawImage>().texture;
        rawImage2 = InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture;
        primaryEqColor = InventoryUi.GetComponent<RawImage>().color;
        primaryItemsListColor = InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color;

        HideEquipment();
    }

    void Update()
    {
        if (Input.GetKeyDown(InputManager.InventoryMenuKey) && !isEquipmentShown)
        {
            ShowEquipment();
        }
        else if ((Input.GetKeyDown(InputManager.InventoryMenuKey) || Input.GetKeyDown(KeyCode.Escape)) &&
                 isEquipmentShown)
        {
            HideEquipment();
        }

        if (isEquipmentShown)
        {
            if (isInspectingItems || isPlayerPickingItem)
            {
                float verticalInput = Input.GetAxisRaw("Vertical");

                if (verticalInput > 0 && !isButtonDown)
                {
                    isButtonDown = true;
                    selectedItemIndex = (selectedItemIndex == 0) ? 3 : selectedItemIndex - 1;
                    UpdateEquipmentFrames();
                }
                else if (verticalInput < 0 && !isButtonDown)
                {
                    isButtonDown = true;
                    selectedItemIndex = (selectedItemIndex == 3) ? 0 : selectedItemIndex + 1;
                    UpdateEquipmentFrames();
                }

                if (verticalInput == 0)
                {
                    isButtonDown = false;
                }
            }

            if (!isPlayerPickingItem)
            {
                // Zmiana rodzaju menu eq przeglądanie przedmiotów / wszystko
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                if (horizontalInput > 0)
                {
                    ShowItemInspector();
                }
                else if (horizontalInput < 0)
                {
                    HideItemInspector();
                }
            }
        }
    }

    /*
     * Metoda pokazująca ekwipunek
     */
    public void ShowEquipment()
    {
        InventoryUi.transform.Find("Gold").gameObject.SetActive(true);
        InventoryUi.transform.Find("Health").gameObject.SetActive(true);
        InventoryUi.transform.Find("Experience").gameObject.SetActive(true);
        InventoryUi.transform.Find("ArrowTooltip").gameObject.SetActive(false);
        InventoryUi.transform.Find("ButtonTooltip").gameObject.SetActive(true);

        isEquipmentShown = true;
        isInspectingItems = false;
        isPlayerPickingItem = false;
        Time.timeScale = 0;
        selectedItemIndex = 0;
        //UpdateEquipmentFrames();
        MainUi.SetActive(false);
        InventoryUi.SetActive(true);
        selectedItemDesc.SetActive(false);
        incomingItemInfo.SetActive(false);
        arrowTooltip.SetActive(false);
        buttonTooltip.SetActive(true);

        UpdateHp();
        UpdateGold();
        UpdateElemental();
        UpdateExperience();
    }

    /*
     * Metoda służąca do podnoszenia przedmiotów
     */
    public void PickupItem(ItemData itemData)
    {
        InventoryUi.transform.Find("Gold").gameObject.SetActive(false);
        InventoryUi.transform.Find("Health").gameObject.SetActive(false);
        InventoryUi.transform.Find("Experience").gameObject.SetActive(false);
        InventoryUi.transform.Find("ArrowTooltip").gameObject.SetActive(true);
        InventoryUi.transform.Find("ButtonTooltip").gameObject.SetActive(false);

        ShowItemInspector();
        SetItemInfo(itemData, incomingItemInfo);
        incomingItemInfo.SetActive(true);
        isPickingItem = true;
    }

    public void EndPickingItem()
    {
        InventoryUi.transform.Find("Experience").gameObject.SetActive(false);
        HideItemInspector();
        incomingItemInfo.SetActive(false);
        isPickingItem = false;
    }

    /*
     * Metoda pokazująca ekwipunek
     */
    public void HideEquipment()
    {
        ResetItemsFields();
        HideItemInspector();

        isEquipmentShown = false;
        Time.timeScale = 1;
        MainUi.SetActive(true);
        InventoryUi.SetActive(false);
    }

    private void ShowItemInspector()
    {
        isInspectingItems = true;
        buttonTooltip.SetActive(false);

        // zmiana stanu menu z Inventory na InspectingItem
        selectedItemDesc.SetActive(false);
        InventoryUi.transform.Find("MissionInfo").gameObject.SetActive(false);
        InventoryUi.transform.Find("Elemental").gameObject.SetActive(false);

        // wydłużenie width pasku itemów
        RectTransform rectTransform = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>();
        Vector2 offsetMin = rectTransform.offsetMin;
        offsetMin.x = ItemsListInitialWidth - 300.0f; // Ustawienie wartości 'left' na 50
        rectTransform.offsetMin = offsetMin;

        // zamiana tła menu z listą itemów
        InventoryUi.GetComponent<RawImage>().texture = rawImage2;
        InventoryUi.GetComponent<RawImage>().color = secondaryEqColor;

        InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture = rawImage1;
        InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color = secondaryItemsListColor;

        UpdateEquipmentFrames();
    }

    private void HideItemInspector()
    {
        isInspectingItems = false;
        buttonTooltip.SetActive(true);

        // zmiana stanu menu z InspectingItem na Inventory
        selectedItemDesc.SetActive(false);
        InventoryUi.transform.Find("MissionInfo").gameObject.SetActive(true);
        InventoryUi.transform.Find("Elemental").gameObject.SetActive(true);

        // skrócenie width pasku itemów
        RectTransform rectTransform = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>();
        Vector2 offsetMin = rectTransform.offsetMin;
        offsetMin.x = ItemsListInitialWidth; // Ustawienie wartości 'left' na 50
        rectTransform.offsetMin = offsetMin;

        // zamiana tła menu z listą itemów
        InventoryUi.GetComponent<RawImage>().texture = rawImage1;
        InventoryUi.GetComponent<RawImage>().color = primaryEqColor;

        InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture = rawImage2;
        InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color = primaryItemsListColor;

        // Reset pól wyświetlających itemy
        ResetItemsFields();
        selectedItemIndex = 0;
    }

    public void UpdateHp()
    {
        try
        {
            GameObject healthObject = InventoryUi.transform.Find("Health").gameObject;

            if (healthObject)
            {
                GameObject healthPoints = healthObject.transform.Find("HP").gameObject;
                GameObject maxHealth = healthObject.transform.Find("MaxHP").gameObject;

                if (healthPoints && maxHealth)
                {
                    healthPoints.GetComponent<TextMeshProUGUI>().text = playerStatus.GetHp().ToString();
                    maxHealth.GetComponent<TextMeshProUGUI>().text = " / " + playerStatus.GetMaxHp().ToString() + "HP";
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void UpdateGold()
    {
        try
        {
            TextMeshProUGUI goldCount = InventoryUi.transform.Find("Gold").transform.Find("Count").gameObject
                .GetComponent<TextMeshProUGUI>();

            if (goldCount)
            {
                goldCount.text = playerStatus.GetGold().ToString() + " g";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void UpdateExperience()
    {
        try
        {
            GameObject experienceObject = InventoryUi.transform.Find("Experience").gameObject;

            if (experienceObject)
            {
                GameObject level = experienceObject.transform.Find("Level").gameObject;
                GameObject currentXp = experienceObject.transform.Find("Xp").transform.Find("CurrentXp").gameObject;
                GameObject maxXp = experienceObject.transform.Find("Xp").transform.Find("MaxXp").gameObject;

                if (level && currentXp && maxXp)
                {
                    level.GetComponent<TextMeshProUGUI>().text = playerStatus.GetLevel().ToString() + " lvl";
                    currentXp.GetComponent<TextMeshProUGUI>().text = playerStatus.GetXp().ToString();
                    maxXp.GetComponent<TextMeshProUGUI>().text =
                        " / " + playerStatus.GetExpToNextLVl().ToString() + "xp";
                }
                else
                {
                    Debug.Log("Nie znaleziono elementu dziecka w Experience");
                }
            }
            else
            {
                Debug.Log("Nie znaleziono Experience");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void UpdateElemental()
    {
        try
        {
            GameObject elementalParent = InventoryUi.transform.Find("Elemental").gameObject;

            // szukanie elementu gracza
            List<Player.ElementalType> elementals = gameObject.GetComponent<Player>().ElementalTypes;
            int UsedElementalTypeId = gameObject.GetComponent<Player>().UsedElementalTypeId;

            Player.ElementalType usedElemental = elementals[UsedElementalTypeId];

            if (elementalParent && null != usedElemental)
            {
                GameObject ElementalImage = elementalParent.transform.Find("ElementalImage").gameObject;
                GameObject ElementalName = elementalParent.transform.Find("ElementalName").gameObject;

                if (ElementalImage && ElementalName)
                {
                    ElementalImage.GetComponent<Image>().sprite = usedElemental.icon;
                    ElementalName.GetComponent<TextMeshProUGUI>().text = usedElemental.name;
                    ElementalName.GetComponent<TextMeshProUGUI>().color = usedElemental.elementalColor;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /*
     * Metoda aktualizująca obrazek w UI, na podstawie podanych danych
     */
    public void SetImageAtSlot(ItemData itemData)
    {
        if (null != itemData.itemIcon)
        {
            GameObject selectedSlot = fields.transform.GetChild(selectedItemIndex).Find("ItemImage").gameObject;

            if (null != itemData.itemIcon && selectedSlot)
            {
                selectedSlot.GetComponent<Image>().sprite = itemData.itemIcon;

                MainUi.transform.Find("Items").GetChild(selectedItemIndex).GetComponent<Image>().sprite =
                    itemData.itemIcon;
                MainUi.transform.Find("Items").GetChild(selectedItemIndex).GetComponent<Image>().color = Color.white;
            }
            else
            {
                Debug.LogError("Wystąpił problem przy ładowaniu ikony przedmiotu");
            }
        }
    }

    /*
     * Metoda do dynamicznej aktualizacji obrazka
     */
    public void SetImageAtSlotByIndex(String imagePath, String itemName)
    {
        int itemIndex = itemsHandler.items.FindIndex(obj =>
            string.Equals(obj.itemName, itemName, StringComparison.OrdinalIgnoreCase));

        if (imagePath != "" || itemIndex == -1)
        {
            GameObject selectedSlot = fields.transform.GetChild(itemIndex).Find("ItemImage").gameObject;

            Texture2D texture2D = Resources.Load<Texture2D>(imagePath);
            if (texture2D != null)
            {
                selectedSlot.GetComponent<Image>().sprite = Sprite.Create(texture2D,
                    new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
                MainUi.transform.Find("Items").GetChild(itemIndex).GetComponent<Image>().sprite =
                    Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            }
        }
    }

    private void UpdateEquipmentFrames()
    {
        // zwaracnie wszystkich pól eq do podstawowego stanu
        ResetItemsFields();

        // odpowiednie ustawienie pola wybranego
        GameObject selectedField = fields.transform.GetChild(selectedItemIndex).gameObject;
        if (selectedFieldImage != null && selectedField != null)
        {
            // ustawienie obramowania dla wybranego pola
            selectedField.transform.Find("Frame").GetComponent<Image>().sprite = selectedFieldImage;

            // Pokazanie strzałki
            selectedField.transform.Find("Arrow").gameObject.SetActive(true);

            ItemData selectedItem = itemsHandler.items[selectedItemIndex];
            if (null != selectedItem)
            {
                SetItemInfo(itemsHandler.items[selectedItemIndex], selectedItemDesc);
                selectedItemDesc.SetActive(true);
            }
            else
            {
                selectedItemDesc.SetActive(false);
            }
        }
    }

    private void ResetItemsFields()
    {
        foreach (Transform child in fields.transform)
        {
            // resetowanie obramowania pola
            Image image = child.transform.Find("Frame").GetComponent<Image>();
            if (image != null)
            {
                image.sprite = fieldImage;
            }

            // ukrywanie strzałki przedmiotu
            GameObject arrow = child.transform.Find("Arrow").gameObject;
            if (arrow != null)
            {
                arrow.SetActive(false);
            }
        }
    }

    public void SetItemInfo(ItemData itemData, GameObject UiParent)
    {
        //Ustawianie opisu przedmiotu
        GameObject header = UiParent.transform.Find("Header").gameObject;
        GameObject passive = UiParent.transform.Find("Passive").gameObject;
        GameObject active = UiParent.transform.Find("Active").gameObject;

        if (header)
        {
            // Tytuł przedmiotu
            header.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = itemData.itemName;

            // Ikona przedmiotu
            header.transform.Find("ItemIcon").gameObject.GetComponent<Image>().sprite = itemData.itemIcon;
        }

        if (passive)
        {
            // Pasywka przedmiotu
            passive.transform.Find("PassiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text =
                itemData.passiveDescription;
        }

        if (active)
        {
            // Zdolność aktywna przedmiotu
            active.transform.Find("ActiveDescription").gameObject.GetComponent<TextMeshProUGUI>().text =
                itemData.activeDescription;
        }
    }
}
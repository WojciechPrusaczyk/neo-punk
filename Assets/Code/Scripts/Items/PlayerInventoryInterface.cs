using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlayerInventoryInterface : MonoBehaviour
{
    public bool isEquipmentShown = false;
    public bool isPlayerPickingItem = false;
    public bool isInspectingItems = false;
    public int selectedItemIndex = 0;

    [Header("Index of equipment inventory in mainUi")]
    public int interfaceIndex = 3;

    [Range(1, 4)] public int unlockedSlots = 2;
    public Sprite fieldImage;
    public Sprite selectedFieldImage;
    public Color secondaryEqColor;
    public Color secondaryItemsListColor;

    private GameObject MainUi;
    private UserInterfaceController userInterfaceController;
    private VisualElement rootVisualElement;
    private GameObject fields;
    public GameObject incomingItemInfo;
    private GameObject arrowTooltip;
    private ItemsHandler itemsHandler;
    private EntityStatus playerStatus;
    private float ItemsListInitialWidth;
    private Texture rawImage1;
    private Texture rawImage2;
    private Color primaryEqColor;
    private Color primaryItemsListColor;
    private bool isButtonDown = false;
    public bool isPickingItem = false;

    /*
     * Ui document
     */
    [SerializeField] private VisualElement root;
    private List<VisualElement> _itemSlots = new List<VisualElement>();

    public void Start()
    {
        MainUi = GameObject.Find("MainUserInterfaceRoot");
        userInterfaceController = MainUi.GetComponent<UserInterfaceController>();
        // selectedItemDesc = InventoryUi.transform.Find("SelectedItemInfo").gameObject;
        // incomingItemInfo = InventoryUi.transform.Find("IncomingItemInfo").gameObject;
        // arrowTooltip = InventoryUi.transform.Find("ArrowTooltip").gameObject;
        // fields = InventoryUi.transform.Find("ItemsFields").gameObject;
        // itemsHandler = gameObject.GetComponent<ItemsHandler>();
        // playerStatus = gameObject.GetComponent<EntityStatus>();
        // ItemsListInitialWidth = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>().offsetMin.x;
        //
        // rawImage1 = InventoryUi.GetComponent<RawImage>().texture;
        // rawImage2 = InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture;
        // primaryEqColor = InventoryUi.GetComponent<RawImage>().color;
        // primaryItemsListColor = InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color;

        // HideEquipment();
    }

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Player inventory interface error! NIE MA UIDOCUMENT");
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Player inventory interface error! NIE MA ROOT VISUALELEMENT");
        }

        // Zbieramy sloty itemów item1–item4
        _itemSlots = new List<VisualElement>();
        for (int i = 1; i <= 4; i++)
        {
            var itemSlot = root.Q<VisualElement>($"item{i}");
            if (itemSlot != null)
            {
                _itemSlots.Add(itemSlot);
            }
        }
        /*
         * Elementy UI
         */
        rootVisualElement = root.Q<VisualElement>("Root");

        SetUnlockedSlotsCount(unlockedSlots);

        if (isPlayerPickingItem) ShowItemInspector();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && isEquipmentShown)
        {
            HideEquipment();
        }

        //
        // if (isEquipmentShown)
        // {
        //     if (isInspectingItems || isPlayerPickingItem)
        //     {
        //         float verticalInput = Input.GetAxisRaw("Vertical");
        //
        //         if (verticalInput > 0 && !isButtonDown)
        //         {
        //             isButtonDown = true;
        //             selectedItemIndex = (selectedItemIndex == 0) ? 3 : selectedItemIndex - 1;
        //             UpdateEquipmentFrames();
        //         }
        //         else if (verticalInput < 0 && !isButtonDown)
        //         {
        //             isButtonDown = true;
        //             selectedItemIndex = (selectedItemIndex == 3) ? 0 : selectedItemIndex + 1;
        //             UpdateEquipmentFrames();
        //         }
        //
        //         if (verticalInput == 0)
        //         {
        //             isButtonDown = false;
        //         }
        //     }
        //
        //     if (!isPlayerPickingItem)
        //     {
        //         // Zmiana rodzaju menu eq przeglądanie przedmiotów / wszystko
        //         float horizontalInput = Input.GetAxisRaw("Horizontal");
        //         if (horizontalInput > 0)
        //         {
        //             ShowItemInspector();
        //         }
        //         else if (horizontalInput < 0)
        //         {
        //             HideItemInspector();
        //         }
        //     }
        // }
    }

    private void SetUnlockedSlotsCount(int unlockedSlotsCount)
    {
        foreach (var item in _itemSlots.Select((slot, i) => new { i, slot }))
        {
            if (item.i < unlockedSlotsCount)
                item.slot.RemoveFromClassList("lockedSlot");
            else
                item.slot.AddToClassList("lockedSlot");
        }
    }

    /*
     * Metoda pokazująca ekwipunek
     */
    public void ShowEquipment()
    {
        userInterfaceController.ActivateInterface(interfaceIndex);
        gameObject.SetActive(true);

        isEquipmentShown = true;
        isInspectingItems = false;
        isPlayerPickingItem = false;
        selectedItemIndex = 0;
    }

    /*
     * Metoda ukrywająca ekwipunek
     */
    public void HideEquipment()
    {
        ResetItemsFields();
        HideItemInspector();

        userInterfaceController.ActivateInterface(0);
        gameObject.active = false;
        rootVisualElement.RemoveFromClassList("inspectorShown");

        isEquipmentShown = false;
        isInspectingItems = false;
        isPlayerPickingItem = false;
        selectedItemIndex = 0;
    }

    /*
     * Metoda służąca do podnoszenia przedmiotów
     */
    public void PickupItem(ItemData itemData)
    {
        isPickingItem = true;
        SetItemInfo(itemData, incomingItemInfo);
        incomingItemInfo.SetActive(true);
    }

    public void EndPickingItem()
    {
        // InventoryUi.transform.Find("Experience").gameObject.SetActive(false);
        HideItemInspector();
        incomingItemInfo.SetActive(false);
        isPickingItem = false;
    }

    private void ShowItemInspector()
    {
        isInspectingItems = true;

        // Przełączanie trybu UI
        rootVisualElement.AddToClassList("inspectorShown");

        // InventoryUi.transform.Find("MissionInfo").gameObject.SetActive(false);
        // InventoryUi.transform.Find("Elemental").gameObject.SetActive(false);

        // wydłużenie width pasku itemów
        // RectTransform rectTransform = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>();
        // Vector2 offsetMin = rectTransform.offsetMin;
        // offsetMin.x = ItemsListInitialWidth - 300.0f; // Ustawienie wartości 'left' na 50
        // rectTransform.offsetMin = offsetMin;

        // zamiana tła menu z listą itemów
        // InventoryUi.GetComponent<RawImage>().texture = rawImage2;
        // InventoryUi.GetComponent<RawImage>().color = secondaryEqColor;

        // InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture = rawImage1;
        // InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color = secondaryItemsListColor;

        // UpdateEquipmentFrames();
    }

    private void HideItemInspector()
    {
        isInspectingItems = false;

        // Przełączanie trybu UI
        rootVisualElement.RemoveFromClassList("inspectorShown");

        // InventoryUi.transform.Find("MissionInfo").gameObject.SetActive(true);
        // InventoryUi.transform.Find("Elemental").gameObject.SetActive(true);

        // skrócenie width pasku itemów
        // RectTransform rectTransform = InventoryUi.transform.Find("ItemsFields").GetComponent<RectTransform>();
        // Vector2 offsetMin = rectTransform.offsetMin;
        // offsetMin.x = ItemsListInitialWidth; // Ustawienie wartości 'left' na 50
        // rectTransform.offsetMin = offsetMin;

        // zamiana tła menu z listą itemów
        // InventoryUi.GetComponent<RawImage>().texture = rawImage1;
        // InventoryUi.GetComponent<RawImage>().color = primaryEqColor;

        // InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().texture = rawImage2;
        // InventoryUi.transform.Find("ItemsFields").GetComponent<RawImage>().color = primaryItemsListColor;

        // Reset pól wyświetlających itemy
        ResetItemsFields();
        selectedItemIndex = 0;
    }

    public void UpdateElemental()
    {
        // try
        // {
        //     GameObject elementalParent = InventoryUi.transform.Find("Elemental").gameObject;
        //
        //     // szukanie elementu gracza
        //     List<Player.ElementalType> elementals = gameObject.GetComponent<Player>().ElementalTypes;
        //     int UsedElementalTypeId = gameObject.GetComponent<Player>().UsedElementalTypeId;
        //
        //     Player.ElementalType usedElemental = elementals[UsedElementalTypeId];
        //
        //     if (elementalParent && null != usedElemental)
        //     {
        //         GameObject ElementalImage = elementalParent.transform.Find("ElementalImage").gameObject;
        //         GameObject ElementalName = elementalParent.transform.Find("ElementalName").gameObject;
        //
        //         if (ElementalImage && ElementalName)
        //         {
        //             ElementalImage.GetComponent<Image>().sprite = usedElemental.icon;
        //             ElementalName.GetComponent<TextMeshProUGUI>().text = usedElemental.name;
        //             ElementalName.GetComponent<TextMeshProUGUI>().color = usedElemental.elementalColor;
        //         }
        //     }
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        // }
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

    /*
     * Metoda do dynamicznej aktualizacji obrazka
     */
    public void SetImageAtSlotByIndex(Sprite sprite, string itemName)
    {
        int itemIndex = itemsHandler.items.FindIndex(obj =>
            string.Equals(obj.itemName, itemName, StringComparison.OrdinalIgnoreCase));

        if (sprite != null && itemIndex != -1)
        {
            GameObject selectedSlot = fields.transform.GetChild(itemIndex).Find("ItemImage").gameObject;

            selectedSlot.GetComponent<Image>().sprite = sprite;
            MainUi.transform.Find("Items").GetChild(itemIndex).GetComponent<Image>().sprite = sprite;
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
                // SetItemInfo(itemsHandler.items[selectedItemIndex], selectedItemDesc);
                // selectedItemDesc.SetActive(true);
            }
            else
            {
                // selectedItemDesc.SetActive(false);
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
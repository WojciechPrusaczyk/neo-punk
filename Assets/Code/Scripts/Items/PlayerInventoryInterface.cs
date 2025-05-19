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
    private MainUserInterfaceController MainUiController;
    private UserInterfaceController userInterfaceController;
    private VisualElement rootVisualElement;
    private GameObject fields;
    private GameObject arrowTooltip;
    private ItemsHandler itemsHandler;
    private EntityStatus playerStatus;
    private float ItemsListInitialWidth;
    private Texture rawImage1;
    private Texture rawImage2;
    private Color primaryEqColor;
    private Color primaryItemsListColor;
    private bool isButtonDown = false;

    /*
     * Ui document
     */
    [SerializeField] private VisualElement root;
    private List<VisualElement> _itemSlots = new List<VisualElement>();
    private Label IncomingItemName;
    private VisualElement IncomingItemImage;
    private Label IncomingItemPassive;
    private Label IncomingItemActive;
    private Label SelectedItemName;
    private VisualElement SelectedItemImage;
    private Label SelectedItemPassive;
    private Label SelectedItemActive;

    public void Awake()
    {
        MainUi = GameObject.Find("MainUserInterfaceRoot");
        MainUiController = MainUi.GetComponentInChildren<MainUserInterfaceController>();
        userInterfaceController = MainUi.GetComponent<UserInterfaceController>();
        itemsHandler = GameObject.FindWithTag("Player").GetComponent<ItemsHandler>();
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
        IncomingItemName = root.Q<Label>("IncomingItemName");
        IncomingItemImage = root.Q<VisualElement>("IncomingItemImage");
        IncomingItemPassive = root.Q<Label>("IncomingItemPassive");
        IncomingItemActive = root.Q<Label>("IncomingItemActive");
        SelectedItemName = root.Q<Label>("SelectedItemName");
        SelectedItemImage = root.Q<VisualElement>("SelectedItemImage");
        SelectedItemPassive = root.Q<Label>("SelectedItemPassive");
        SelectedItemActive = root.Q<Label>("SelectedItemActive");

        SetUnlockedSlotsCount();

        if (isPlayerPickingItem) ShowItemInspector();

        isEquipmentShown = true;
        selectedItemIndex = 0;
    }

    private void OnDisable()
    {
        HideItemInspector();

        rootVisualElement.RemoveFromClassList("inspectorShown");

        isEquipmentShown = false;
        isInspectingItems = false;
        isPlayerPickingItem = false;
        selectedItemIndex = 0;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && isEquipmentShown)
        {
            HideEquipment();
        }

        if (isEquipmentShown)
        {

            if (!isPlayerPickingItem)
            {
                // Zmiana rodzaju menu eq przeglądanie przedmiotów / wszystko

                if (Input.GetKeyDown(InputManager.MoveRightKey))
                {
                    isInspectingItems = true;
                    rootVisualElement.AddToClassList("inspectorShown");
                }
                else if (Input.GetKeyDown(InputManager.MoveLeftKey))
                {
                    isInspectingItems = false;
                    rootVisualElement.RemoveFromClassList("inspectorShown");
                }
            }
            if (isInspectingItems || isPlayerPickingItem)
            {
                float verticalInput = Input.GetAxisRaw("Vertical");

                if (verticalInput > 0 && !isButtonDown)
                {
                    isButtonDown = true;
                    selectedItemIndex = (selectedItemIndex == 0) ? unlockedSlots - 1 : selectedItemIndex - 1;
                }
                else if (verticalInput < 0 && !isButtonDown)
                {
                    isButtonDown = true;
                    selectedItemIndex = (selectedItemIndex == unlockedSlots - 1) ? 0 : selectedItemIndex + 1;
                }

                if (verticalInput == 0)
                {
                    isButtonDown = false;
                }

                ShowSelectedItemFrame();
                UpdateSelectedItemInfo();
            }
        }
    }

    private void SetUnlockedSlotsCount()
    {
        foreach (var item in _itemSlots.Select((slot, i) => new { i, slot }))
        {
            if (item.i < unlockedSlots)
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
        isPlayerPickingItem = true;
        rootVisualElement.AddToClassList("pickingItem");
        SetItemInfo(itemData);
    }

    public void EndPickingItem()
    {
        // InventoryUi.transform.Find("Experience").gameObject.SetActive(false);
        HideItemInspector();
        rootVisualElement.RemoveFromClassList("pickingItem");
        isPlayerPickingItem = false;
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
            var selectedSlot = _itemSlots[selectedItemIndex];
            var selectedSlotImage = selectedSlot.Q<VisualElement>("Image");

            if (null != itemData.itemIcon && (null != selectedSlotImage ))
            {
                selectedSlotImage.style.backgroundImage = new StyleBackground(itemData.itemIcon);

                MainUiController.SetItemImageAtSlot(selectedItemIndex, itemData.itemIcon);
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
    public void SetImageAtSlotByIndex(Sprite sprite, string itemName)
    {
        int itemIndex = itemsHandler.items.FindIndex(obj =>
            string.Equals(obj.itemName, itemName, StringComparison.OrdinalIgnoreCase));

        if (sprite != null && itemIndex != -1)
        {
            var selectedSlot = _itemSlots[selectedItemIndex];
            var selectedSlotImage = selectedSlot.Q<VisualElement>("Image");

            selectedSlotImage.style.backgroundImage = new StyleBackground(sprite);

            MainUiController.SetItemImageAtSlot(selectedItemIndex, sprite);
        }
    }

    private void ShowSelectedItemFrame()
    {
        foreach (var item in _itemSlots.Select((slot, i) => new { i, slot }))
        {
            if (item.i < unlockedSlots)
                item.slot.RemoveFromClassList("choosenSlot");
        }

        _itemSlots[selectedItemIndex].AddToClassList("choosenSlot");
    }

    private void UpdateSelectedItemInfo()
    {
        // Czyszczenie itemu, jeśli nie wybrano żadnego
        var selectedItem = itemsHandler.items[selectedItemIndex];
        if (null == selectedItem || !selectedItem )
        {
            SelectedItemName.text = "";
            SelectedItemActive.style.backgroundImage = null;
            SelectedItemPassive.text = "";
            SelectedItemActive.text = "";

            rootVisualElement.RemoveFromClassList("selectedValidItem");

            return;
        }

        rootVisualElement.AddToClassList("selectedValidItem");

        // Tytuł przedmiotu
        if (null != SelectedItemName)
            SelectedItemName.text = selectedItem.itemName;

        // Zdolność aktywna przedmiotu
        if (null != SelectedItemImage)
            SelectedItemImage.style.backgroundImage = new StyleBackground(selectedItem.itemIcon);

        // Pasywka przedmiotu
        if (null != SelectedItemPassive)
            SelectedItemPassive.text = selectedItem.passiveDescription;

        // Zdolność aktywna przedmiotu
        if (null != SelectedItemActive)
            SelectedItemActive.text = selectedItem.activeDescription;
    }

    public void SetItemInfo(ItemData itemData)
    {
        // Tytuł przedmiotu
        if (null != IncomingItemName)
            IncomingItemName.text = itemData.itemName;

        // Zdolność aktywna przedmiotu
        if (null != IncomingItemImage)
            IncomingItemImage.style.backgroundImage = new StyleBackground(itemData.itemIcon);

        // Pasywka przedmiotu
        if (null != IncomingItemPassive)
            IncomingItemPassive.text = itemData.passiveDescription;

        // Zdolność aktywna przedmiotu
        if (null != IncomingItemActive)
            IncomingItemActive.text = itemData.activeDescription;
    }
}
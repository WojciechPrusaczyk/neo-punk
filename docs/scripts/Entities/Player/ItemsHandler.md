[Go back to contents](../../../contents.md)

### ItemsHandler

**Lokalizacja:** [`Assets/Code/Scripts/Entities/Player/ItemsHandler.cs`](../../../../Assets/Code/Scripts/Entities/Player/ItemsHandler.cs)

## Pola

- `public List<ItemData> items`  
  Lista przechowująca dane przedmiotów (`ItemData`) w poszczególnych slotach (do czterech przedmiotów).

- `private GameObject MainUi`  
  Referencja do głównego obiektu UI gry.

- `private List<TextMeshProUGUI> itemsCooldowns`  
  Lista komponentów `TextMeshProUGUI` do wyświetlania czasów cooldownów na UI dla każdego przedmiotu.

- `private PlayerInventory playerInventory`  
  Referencja do obiektu `PlayerInventory`, umożliwiającego interakcję z ekwipunkiem gracza.

## Metody

### `private void Start()`
Inicjalizuje początkowe wartości listy przedmiotów oraz odnajduje wymagane komponenty w drzewie obiektów Unity. Dodaje cztery puste sloty dla przedmiotów oraz ustawia referencje do tekstów cooldownów w UI.

### `private void Update()`
Sprawdza, czy gracz próbuje użyć danego przedmiotu za pomocą klawiszy (1-4). Wywołuje metody `UseItem`, `UsePassive` i `UpdateCooldownTimer` dla aktywnych przedmiotów, zapewniając ich funkcjonalność w czasie rzeczywistym.

### `public void AddItem(ItemData itemData, GameObject objectToDelete)`
Dodaje nowy przedmiot do ekwipunku. Wywołuje współbieżną metodę `WaitForAction`, która czeka na akcję gracza (przypisanie przedmiotu do slota). Jeśli gracz potwierdzi akcję, przedmiot zostaje przypisany do wybranego slota.

### `private IEnumerator WaitForAction(ItemData itemData, GameObject pickedObject)`
Metoda współbieżna, która umożliwia graczowi przypisanie nowego przedmiotu do wybranego slota w ekwipunku. Czeka na naciśnięcie klawisza interakcji, aby przypisać przedmiot, aktualizuje obraz przedmiotu w UI, kończy interakcję i niszczy podniesiony obiekt.

### `public void UseItem(int itemPos)`
Używa przedmiotu z danego slota (określonego przez `itemPos`) poprzez wywołanie jego zdolności (`itemAbility.Use()`). Ustawia czas cooldownu i uruchamia metodę `CooldownTimer`. Zmienia kolor ikony przedmiotu na ciemniejszy, aby wskazać aktywny cooldown.

### `private IEnumerator CooldownTimer(ItemData item, int itemPos)`
Metoda współbieżna, która zmniejsza wartość cooldownu przedmiotu co sekundę. Po zakończeniu cooldownu zmienia kolor ikony przedmiotu na biały, informując gracza o ponownej dostępności przedmiotu.

### `public void UsePassive(int itemPos)`
Aktywuje pasywną zdolność przedmiotu przypisanego do danego slota (jeśli istnieje), wywołując metodę `Apply()` na `itemAbility`.

### `public void OnItemDisband(int itemPos)`
Usuwa pasywną zdolność przedmiotu z danego slota, wywołując metodę `Remove()` na `itemAbility`. Metoda służy do deaktywacji efektów przedmiotu, gdy zostaje on usunięty z ekwipunku.

### `private void UpdateCooldownTimer(int itemPos)`
Aktualizuje wyświetlany czas cooldownu dla danego przedmiotu. Jeśli przedmiot jest w trakcie cooldownu, wartość ta jest wyświetlana w interfejsie gracza.

## Przykłady użycia
1. **Przypisanie przedmiotu do slota:** Gracz podnosi przedmiot, a następnie przypisuje go do pustego slota w ekwipunku za pomocą `AddItem()`.
2. **Użycie przedmiotu:** Gracz używa przedmiotu za pomocą przycisków (1-4). Metoda `UseItem()` aktywuje zdolność przedmiotu i uruchamia cooldown.
3. **Wyświetlanie czasu cooldownu:** W `UpdateCooldownTimer()` interfejs gracza wyświetla pozostały czas cooldownu dla aktywnych przedmiotów.

## Uwagi
- Każdy przedmiot może być używany tylko wtedy, gdy nie jest na cooldownie.
- UI przedmiotów jest zorganizowane w hierarchii obiektów UI gry, a dostęp do komponentów cooldownów jest uzyskiwany dynamicznie na początku gry.


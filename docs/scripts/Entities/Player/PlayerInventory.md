[Go back to contents](../../../contents.md)

### PlayerInventory

**Lokalizacja:** [`Assets/Code/Scripts/Entities/Player/PlayerInventory.cs`](../../../../Assets/Code/Scripts/Entities/Player/PlayerInventory.cs)

#### Opis
Klasa `PlayerInventory` odpowiada za zarządzanie ekwipunkiem gracza, w tym za wyświetlanie i ukrywanie ekwipunku, wybór przedmiotów oraz interakcje z przedmiotami. Umożliwia także podgląd szczegółowych informacji o przedmiotach i przeglądanie ekwipunku.

#### Publiczne pola

- `isEquipmentShown` *(bool)*: Określa, czy interfejs ekwipunku jest obecnie widoczny.
- `isPlayerPickingItem` *(bool)*: Określa, czy gracz obecnie podnosi przedmiot.
- `isInspectingItems` *(bool)*: Określa, czy gracz przegląda szczegóły przedmiotów.
- `selectedItemIndex` *(int)*: Indeks aktualnie wybranego przedmiotu w ekwipunku.
- `fieldImage` *(Sprite)*: Obrazek pola ekwipunku w trybie nieaktywnym.
- `selectedFieldImage` *(Sprite)*: Obrazek pola ekwipunku dla wybranego przedmiotu.
- `secondaryEqColor` *(Color)*: Kolor tła dla menu ekwipunku w trybie przeglądania.
- `secondaryItemsListColor` *(Color)*: Kolor tła dla listy przedmiotów w trybie przeglądania.

#### Metody

- **`Start()`**: Inicjalizuje elementy interfejsu ekwipunku i ustawia początkowe wartości pól.
- **`Update()`**: Odpowiada za aktualizację stanu ekwipunku oraz wykrywa zmiany wejścia od użytkownika (klawisze).
- **`ShowEquipment()`**: Wyświetla interfejs ekwipunku i zatrzymuje czas gry.
- **`HideEquipment()`**: Ukrywa interfejs ekwipunku i wznawia czas gry.
- **`PickupItem(ItemData itemData)`**: Rozpoczyna proces podnoszenia przedmiotu i wyświetla informacje o podnoszonym przedmiocie.
- **`EndPickingItem()`**: Kończy proces podnoszenia przedmiotu i ukrywa informacje o przedmiocie.
- **`ShowItemInspector()`**: Przełącza ekwipunek w tryb inspektora przedmiotów.
- **`HideItemInspector()`**: Wyłącza tryb inspektora przedmiotów i przywraca zwykły widok ekwipunku.
- **`UpdateHp()`**: Aktualizuje wyświetlaną ilość punktów zdrowia gracza.
- **`UpdateGold()`**: Aktualizuje ilość złota posiadanego przez gracza.
- **`UpdateExperience()`**: Aktualizuje poziom i doświadczenie gracza.
- **`UpdateElemental()`**: Aktualizuje elementarne zdolności gracza.

#### Powiązania

Klasa `PlayerInventory` jest powiązana z klasą `ItemsHandler`, która zarządza operacjami na przedmiotach i wspiera działanie ekwipunku.

#### Uwagi

- **Zmienne prywatne**: Klasa posiada liczne zmienne prywatne do zarządzania interfejsem użytkownika i stanu gry, takie jak `MainUi`, `InventoryUi`, `fields`, itp.
- **Interakcja z `EntityStatus`**: `PlayerInventory` używa klasy `EntityStatus` do uzyskiwania aktualnych informacji o graczy, takich jak punkty zdrowia i doświadczenie.

---
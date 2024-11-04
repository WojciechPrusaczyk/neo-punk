[Go back to contents](../../contents.md)

### ItemData

**Lokalizacja:** [`Assets/Code/Scripts/NeoPunk/ItemData.cs`](../../../Assets/Code/Scripts/NeoPunk/ItemData.cs)

Klasa `ItemData` jest typem `ScriptableObject` reprezentującym dane i właściwości przedmiotu w grze. Używana jest do definiowania i przechowywania informacji o przedmiocie, takich jak nazwa, opisy, ikona, rzadkość oraz wymagany poziom gracza. Dodatkowo zawiera interfejs `IItemAbility` definiujący działania związane z aktywacją i dezaktywacją zdolności przedmiotu.

#### Atrybuty klasy

- `[CreateAssetMenu(fileName = "NewItem", menuName = "NeoPunk/Item", order = 1)]`  
  Pozwala na tworzenie nowych instancji `ItemData` bezpośrednio z menu `Assets > Create > NeoPunk > Item` w Unity.

#### Pola Publiczne

- `string itemName`  
  Nazwa przedmiotu.

- `string passiveDescription`  
  Opis efektów pasywnych przedmiotu.

- `string activeDescription`  
  Opis efektów aktywnych przedmiotu, uruchamianych przez gracza.

- `float cooldown`  
  Czas odnowienia aktywnej zdolności przedmiotu.

- `float currentCooldown`  
  Aktualny czas pozostały do ponownego użycia zdolności aktywnej.

- `string rarity`  
  Rzadkość przedmiotu (np. "Common", "Rare").

- `float minPlayerLvl`  
  Minimalny poziom gracza wymagany do użycia przedmiotu.

- `Sprite itemIcon`  
  Ikona przedmiotu wyświetlana w interfejsie użytkownika.

- `IItemAbility itemAbility`  
  Zdolność przypisana do przedmiotu implementująca interfejs `IItemAbility`, umożliwiająca dodawanie specyficznych efektów i działań.

#### Metody

- `virtual void Initialize()`  
  Metoda do nadpisania, przeznaczona do inicjalizacji przedmiotu. Może być używana do przypisania początkowych stanów lub wartości przedmiotu.

#### Interfejs IItemAbility

Interfejs `IItemAbility` definiuje zestaw metod związanych z obsługą zdolności przedmiotu. Każda klasa implementująca ten interfejs powinna dostarczyć funkcjonalności dla:

- `void Use()`  
  Wywoływana, gdy zdolność przedmiotu jest aktywowana przez gracza.

- `void Apply()`  
  Wywoływana podczas stosowania efektów zdolności na gracza lub inne obiekty.

- `void Remove()`  
  Wywoływana przy dezaktywacji lub usunięciu efektów zdolności.

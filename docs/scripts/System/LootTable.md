[Go back to code contents](../../codeContents.md)

Klasa `LootTable` znajduje się w [`Assets/Code/Scripts/System/LootTable.cs`](../../../Assets/Code/Scripts/System/LootTable.cs)

Klasa ta zarządza logiką losowego generowania łupów dla obiektu, do którego jest przypisana. Przechowuje listę dostępnych przedmiotów do wylosowania i określa szansę na pojawienie się danego przedmiotu. Gdy wywołana jest metoda `DropLoot`, klasa losuje przedmiot na podstawie jego szansy dropu i wywołuje jego pojawienie się w grze.

### Klasy składowe
- `LootItem` - klasa wewnętrzna do przechowywania informacji o przedmiotach w `LootTable`. Zawiera dwa publiczne pola:
    - `int itemId` - identyfikator przedmiotu używany do odnalezienia go w `ScriptableObjectManager`.
    - `float dropChance` - szansa na wylosowanie przedmiotu (wartość z zakresu 0-100%).

### Skrypty powiązane i wymagane komponenty
Klasa `LootTable` wykorzystuje komponent `ScriptableObjectManager`, który powinien być zdefiniowany na scenie pod nazwą `"ScriptableObjectManager"`. Skrypt wymaga również dostępu do zasobów prefabów w katalogu `Resources/Items/Prefabs`, skąd ładuje obiekty do gry.

### Metody

#### `void DropLoot()`
Metoda odpowiedzialna za losowe generowanie łupów:
- Jeśli lista `lootItems` jest pusta lub `null`, metoda wyświetla ostrzeżenie i kończy działanie.
- Dla każdego przedmiotu w `lootItems` generuje losową wartość (0-100) i porównuje ją z `dropChance`.
- W przypadku sukcesu wywołuje `SpawnItem` z odpowiednim `itemId` i przerywa pętlę.

#### `void SpawnItem(int itemId)`
Tworzy obiekt łupu na scenie na podstawie jego identyfikatora:
- Pobiera dane przedmiotu (`itemData`) z `ScriptableObjectManager`.
- Ładuje prefab przedmiotu z `Resources/Items/Prefabs/ItemPrefab`.
- Instancjuje prefab w pozycji `LootTable` z lekką zmianą osi Z.
- Przypisuje `itemId` do komponentu `ItemObject` znajdującego się w prefabie.

### Uwagi
- `LootItem` posiada pole `dropChance`, które można konfigurować w inspektorze Unity za pomocą suwaka (0-100).
- Dla poprawnego działania, obiekt `ScriptableObjectManager` musi istnieć na scenie, a prefab przedmiotu powinien znajdować się w katalogu `Resources/Items/Prefabs`.

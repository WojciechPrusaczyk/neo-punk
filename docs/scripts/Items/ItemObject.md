[Go back to contents](../../contents.md)

### ItemObject

**Lokalizacja:** [`Assets/Code/Scripts/Items/ItemObject.cs`](../../../Assets/Code/Scripts/Items/ItemObject.cs)

Klasa `ItemObject` zarządza właściwościami i zachowaniem obiektów przedmiotów, w tym ustawieniami wizualnymi i interakcją z graczem.

#### Pola Publiczne

- `ScriptableObjectManager` (`GameObject`) - Odniesienie do menedżera obiektów `ScriptableObject`, który przechowuje dane przedmiotu.
- `ItemId` (`int`) - Unikalny identyfikator przedmiotu.
- `itemData` (`ItemData`) - Dane przedmiotu, takie jak ikona i rzadkość.
- `CommonColor`, `RareColor`, `QuantumColor` (`Color`) - Kolory reprezentujące różne poziomy rzadkości przedmiotu.

#### Pola Prywatne

- `spriteRenderer` (`SpriteRenderer`) - Komponent odpowiedzialny za wyświetlanie ikony przedmiotu.
- `light2D` (`Light2D`) - Komponent światła, który zmienia kolor zależnie od rzadkości przedmiotu.
- `itemParticles` (`ParticleSystem`) - System cząsteczek dla efektu przedmiotu.
- `dropAnimation` (`ParticleSystem`) - System cząsteczek dla efektu animacji upuszczania przedmiotu.

#### Metody

1. **Start()**
    - Inicjalizuje komponenty `SpriteRenderer`, `Light2D`, `itemParticles`, `dropAnimation`.
    - Pobiera dane przedmiotu na podstawie `ItemId` i aktualizuje kolor przedmiotu oraz efektów wizualnych na podstawie jego rzadkości.

2. **UpdateItemLightColor()**
    - Ustawia kolor światła i efektów cząsteczek (`itemParticles`, `dropAnimation`) zależnie od poziomu rzadkości przedmiotu (`Common`, `Rare`, `Quantum`).

3. **OnTriggerStay2D(Collider2D col)**
    - Sprawdza, czy gracz znajduje się w zasięgu interakcji i czy wciśnięto przycisk interakcji (`InputManager.InteractKey`).
    - Wywołuje korutynę `WaitForFrameThenAddItem`, aby dodać przedmiot do ekwipunku gracza.

4. **WaitForFrameThenAddItem(ItemsHandler itemsHandler)**
    - Korutyna, która czeka do końca klatki, zanim doda przedmiot do ekwipunku gracza za pomocą `itemsHandler.AddItem()`.


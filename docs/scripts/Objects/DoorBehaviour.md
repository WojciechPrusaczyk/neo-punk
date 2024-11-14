[Go back to contents](../../contents.md)

### DoorBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/Objects/DoorBehaviour.cs`](../../../Assets/Code/Scripts/Objects/DoorBehaviour.cs)

Klasa `DoorBehaviour` zarządza zachowaniem drzwi w grze, umożliwiając otwieranie i zamykanie drzwi, a także reagowanie na interakcję gracza w pobliżu drzwi, która może np. skutkować załadowaniem nowej sceny.

#### Pola Publiczne

- `bool IsOpen` - Flaga określająca, czy drzwi są otwarte. Domyślnie `false`.

#### Pola Prywatne

- `GameObject playerObject` - Referencja do obiektu gracza, wykorzystywana do sprawdzenia, czy gracz wchodzi w interakcję z drzwiami.
- `Animator animator` - Referencja do komponentu Animator przypisanego do obiektu drzwi, używana do animacji otwierania i zamykania drzwi.

#### Metody

- `void Awake()` - Wywoływana przy inicjalizacji obiektu:
    - Pobiera komponent `Animator` z obiektu drzwi i przypisuje go do `animator`.
    - Znajduje obiekt `Player` i przypisuje go do `playerObject`.

- `void OpenDoor()` - Ustawia drzwi w stanie otwartym:
    - Ustawia `IsOpen` na `true`.
    - Uaktualnia wartość parametru `IsOpen` w animatorze, aby uruchomić animację otwierania drzwi.

- `void CloseDoor()` - Ustawia drzwi w stanie zamkniętym:
    - Ustawia `IsOpen` na `false`.
    - Uaktualnia wartość parametru `IsOpen` w animatorze, aby uruchomić animację zamykania drzwi.

- `void OnTriggerStay2D(Collider2D collision)` - Wywoływana przy wykryciu obiektu w strefie kolizji 2D:
    - Otwiera drzwi, jeśli nie są otwarte.
    - Sprawdza, czy obiekt w strefie kolizji posiada tag `Player`.
    - Jeśli gracz jest w strefie kolizji i przycisk interakcji (`InputManager.InteractKey`) jest wciśnięty, może załadować odpowiednią scenę (implementacja w `TODO`).

- `void OnTriggerExit2D(Collider2D other)` - Wywoływana, gdy obiekt opuszcza strefę kolizji 2D:
    - Zamyka drzwi wywołując metodę `CloseDoor()`.

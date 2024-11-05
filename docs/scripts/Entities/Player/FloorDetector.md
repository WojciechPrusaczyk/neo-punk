[Go back to contents](../../../contents.md)

### FloorDetector

**Lokalizacja:** [`Assets/Code/Scripts/Entities/Player/FloorDetector.cs`](../../../../Assets/Code/Scripts/Entities/Player/FloorDetector.cs)

## Opis
Klasa `FloorDetector` odpowiada za wykrywanie, czy gracz znajduje się w pobliżu podłoża oraz określenie, czy podłoże jest przechodnie (passable) czy nie (impassable). Klasa ta wykorzystuje `Collider2D` do wykrywania kolizji gracza z różnymi typami podłóg, co jest przydatne do implementacji logiki ruchu i zachowania postaci względem terenu w grze 2D.

## Pola

- `public bool isPlayerNearGround`  
  Flaga wskazująca, czy gracz znajduje się w pobliżu jakiegokolwiek typu podłoża. Wartość `true` oznacza, że gracz jest blisko podłoża.

- `public bool isFloorPassable`  
  Flaga wskazująca, czy wykryte podłoże jest przechodnie. Wartość `true` oznacza, że gracz może przejść przez to podłoże.

- `public GameObject collidingObject`  
  Obiekt podłoża, z którym gracz aktualnie koliduje. Pozwala na identyfikację konkretnego podłoża.

## Metody

### `void OnTriggerStay2D(Collider2D collision)`
Metoda wywoływana w momencie, gdy gracz przebywa w kolizji z obiektem podłoża (`Collider2D`). Ustawia wartości pól na podstawie typu podłoża:
- Jeśli gracz jest w pobliżu obiektu z tagiem `impassableFloor`, ustawia `isPlayerNearGround` na `true`, `isFloorPassable` na `false`, oraz przypisuje `collidingObject` do tego obiektu.
- Jeśli gracz jest w pobliżu obiektu z tagiem `passableFloor`, ustawia `isPlayerNearGround` na `true`, `isFloorPassable` na `true`, oraz przypisuje `collidingObject` do tego obiektu.
- Jeśli gracz przebywa w kolizji z innym typem obiektu, ustawia `isPlayerNearGround` na `false`.

### `void OnTriggerExit2D(Collider2D collision)`
Metoda wywoływana, gdy gracz opuszcza obszar kolizji z obiektem podłoża (`Collider2D`). Sprawdza, czy gracz opuszcza obiekt z tagiem `passableFloor` lub `impassableFloor`, i jeśli tak, resetuje wartości pól:
- Ustawia `isPlayerNearGround` na `false`, `isFloorPassable` na `false`, a `collidingObject` na `null`.

## Przykłady użycia
1. **Sprawdzenie, czy gracz jest blisko podłoża:** Użycie `isPlayerNearGround` pozwala na implementację logiki, w której gracz może skakać lub upadać tylko wtedy, gdy nie znajduje się blisko podłoża.
2. **Obsługa różnych typów podłoży:** Na podstawie wartości `isFloorPassable`, gra może decydować, czy gracz może przejść przez dane podłoże, co jest przydatne w poziomach z platformami przechodnimi i nieprzechodnimi.

## Uwagi
- Obiekty podłoża muszą być oznaczone tagami `passableFloor` lub `impassableFloor`, aby klasa mogła odpowiednio przypisać właściwości kolizji.
- W przypadku rozbudowanych środowisk 2D, klasa `FloorDetector` może być częścią większego systemu fizyki gracza, kontrolując skoki i interakcję gracza z terenem.

[Go back to code contents](../../../codeContents.md)

### HitboxBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/Entities/Player/HitboxBehaviour.cs`](../../../../Assets/Code/Scripts/Entities/Player/HitboxBehaviour.cs)

## Opis
Klasa `HitboxBehaviour` odpowiada za detekcję kolizji z obiektami przeciwników (`Enemy`) w grze 2D stworzonej w Unity. Klasa ta jest przeznaczona do użycia z obiektami, które posiadają komponent `Collider2D` w trybie "Trigger". Pozwala na identyfikację przeciwnika, który wszedł w obszar kolizji, oraz na monitorowanie, kiedy przeciwnik opuszcza ten obszar.

## Pola

- `public GameObject collidingEnemyObject`  
  Przechowuje referencję do obiektu przeciwnika (`Enemy`), który aktualnie znajduje się wewnątrz obszaru kolizji hitboxa. W momencie wejścia przeciwnika do obszaru kolizji pole to jest aktualizowane referencją do tego przeciwnika. W chwili, gdy przeciwnik opuszcza obszar kolizji, pole to jest resetowane do `null`.

## Metody

### `private void OnTriggerEnter2D(Collider2D collision)`
Wywoływana automatycznie, gdy obiekt z przypisanym komponentem `HitboxBehaviour` wykryje wejście innego obiektu do swojego obszaru kolizji. Jeśli obiekt kolidujący posiada tag `Enemy`, metoda ustawia `collidingEnemyObject` na referencję do tego obiektu, umożliwiając dalsze interakcje z wykrytym przeciwnikiem.

### `private void OnTriggerExit2D(Collider2D collision)`
Wywoływana automatycznie, gdy obiekt przeciwnika opuszcza obszar kolizji hitboxa. Resetuje pole `collidingEnemyObject` do `null`, sygnalizując, że przeciwnik opuścił obszar hitboxa i interakcje z nim są zakończone.

## Przykłady użycia
1. **Monitorowanie kolizji z przeciwnikami:** Obiekt wyposażony w `HitboxBehaviour` może śledzić, czy w jego obszarze kolizji znajduje się przeciwnik (`Enemy`), np. aby aktywować odpowiednie zdolności lub efekty, gdy przeciwnik znajdzie się w zasięgu.
2. **Resetowanie po opuszczeniu obszaru:** Gdy przeciwnik opuszcza hitbox, `collidingEnemyObject` jest resetowane, co pozwala na ponowne wykrywanie kolizji tylko z przeciwnikami będącymi w obrębie hitboxa.

## Uwagi
- Aby klasa działała poprawnie, obiekt, do którego jest przypisana, musi posiadać komponent `Collider2D` z ustawieniem "Is Trigger".
- Sprawdzanie tagu `Enemy` w metodzie `OnTriggerEnter2D` zapewnia, że tylko obiekty oznaczone tym tagiem będą traktowane jako przeciwnicy, co zapobiega błędnemu rozpoznawaniu kolizji z innymi obiektami.


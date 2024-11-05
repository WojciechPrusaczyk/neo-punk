[Go back to contents](../../../contents.md)

### Player

**Lokalizacja:** [`Assets/Code/Scripts/Player/Player.cs`](../../../../Assets/Code/Scripts/Entities/Player/Player.cs)


Klasa `Player` reprezentuje postać gracza w grze i odpowiada za obsługę jego ruchu, ataków oraz interakcji z innymi obiektami. Zawiera liczne zmienne i metody, które pozwalają na kontrolowanie zachowania gracza, takich jak ruch, atak, blokowanie, parowanie oraz zarządzanie specjalnymi typami obrażeń.

## Zmienne publiczne

- `List<ElementalType> ElementalTypes` - Lista przechowująca dostępne typy obrażeń elementarnych, które mogą być użyte przez gracza.
- `int UsedElementalTypeId` - Identyfikator aktualnie używanego typu obrażeń.
- `String UsedElementalName` - Nazwa aktualnie używanego typu obrażeń.
- `float jumpForce` - Siła skoku postaci.
- `Animator animator` - Obiekt animatora służący do sterowania animacjami postaci.
- `int attackState` - Aktualny stan sekwencji ataku gracza.
- `bool isAttacking` - Flaga informująca, czy gracz wykonuje atak.
- `bool isGrounded` - Flaga informująca, czy postać znajduje się na ziemi.
- `bool isBlocking` - Flaga informująca, czy postać blokuje.
- `bool isParrying` - Flaga informująca, czy postać paruje atak.
- `float cooldownBetweenBlocks` - Czas pomiędzy kolejnymi blokami.
- `Vector3 lastSafePosition` - Ostatnia bezpieczna pozycja gracza.
- `float playerVoidLevel` - Poziom, poniżej którego postać jest resetowana do ostatniej bezpiecznej pozycji.

## Metody publiczne

### `void Start()`
Inicjalizuje komponenty niezbędne do działania klasy, takie jak `Rigidbody2D`, `PlayerInventory`, `Animator`, czy `PauseMenuBehaviour`. Pobiera również referencję do ikony elementu.

### `void Update()`
Metoda odpowiedzialna za aktualizację stanu gracza w każdej klatce, w tym obsługę ruchu, ataku, skoku, zmianę kierunku oraz interakcji z podłożem.

### `IEnumerator EnableBlockingAfterDuration(float duration)`
Umożliwia ponowne blokowanie po upływie określonego czasu.

### `void Jump()`
Obsługuje skok postaci. Dodaje siłę skoku do komponentu `Rigidbody2D`, jeśli gracz znajduje się na ziemi.

### `IEnumerator Parry()`
Aktywuje tryb parowania na czas określony przez `parryWindow`.

### `void PerformChargeAttack()`
Wykonuje specjalny, naładowany atak, który przesuwa gracza w kierunku przeciwnika i zadaje większe obrażenia.

### `void DisableCollisionForDuration(float duration)`
Tymczasowo wyłącza kolizję z podłożem, umożliwiając przejście przez obiekt kolidujący.

### `void EnableCollision()`
Włącza kolizję z wcześniej ignorowanym obiektem.

### `void DealDamage(float damageToDeal)`
Zadaje obrażenia przeciwnikowi, z którym gracz aktualnie koliduje przy pomocy `swordHitbox`.

### `void StartAttack()`
Rozpoczyna sekwencję ataku, ustawia `isAttacking` na `true`, odgrywa animację ataku oraz zadaje obrażenia.

### `void ContinueAttack()`
Kontynuuje sekwencję ataku, sprawdzając czas pomiędzy kolejnymi atakami oraz zadając obrażenia w zależności od aktualnego stanu ataku.

---

## Klasa `ElementalType`

### `string name`
Nazwa typu obrażeń elementarnych.

### `Sprite icon`
Ikona reprezentująca typ obrażeń elementarnych.

### `Color elementalColor`
Kolor powiązany z typem obrażeń elementarnych.

---
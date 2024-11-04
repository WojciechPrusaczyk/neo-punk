[Go back to contents](../../contents.md)

Klasa `InputManager` znajduje się w [`Assets/Code/Scripts/System/InputManager.cs`](../../../Assets/Code/Scripts/System/InputManager.cs)

`InputManager` to statyczna klasa, która zarządza i przechowuje przypisania klawiszy oraz przycisków dla klawiatury i padów (kontrolerów) używanych w grze. Klasa umożliwia graczowi konfigurowanie i dostosowywanie klawiszy.

### Przeznaczenie i zasady funkcjonowania klasy `InputManager`
1. **Przypisania klawiszy i przycisków** – klasa definiuje domyślne klawisze i przyciski zarówno dla klawiatury, jak i padów. Przykładowo, `JumpKey` jest domyślnie ustawiony na `Space`, a `PadButtonAttack` na `"Fire1"`.
2. **Metody zmiany przypisań** – `InputManager` udostępnia publiczne metody pozwalające na zmianę każdego przypisania. Funkcje te mogą być używane w grze do dynamicznego dostosowywania sterowania przez gracza.

### Kluczowe zasady użytkowania
- **Statyczne pola** – ponieważ klasa jest statyczna, wszystkie pola i metody są bezpośrednio dostępne bez konieczności tworzenia instancji `InputManager`.
- **Przyciski i klawisze przypisane jako `KeyCode` lub `string`** – klawiatura używa `KeyCode`, natomiast przyciski pada są definiowane jako `string` reprezentujące ich nazwy w systemie Input Manager w Unity.

Klasa `InputManager` upraszcza zarządzanie sterowaniem i zapewnia centralne miejsce konfiguracji dla ustawień wejściowych w grze.

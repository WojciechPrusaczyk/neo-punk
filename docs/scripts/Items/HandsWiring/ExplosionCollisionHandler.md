[Go back to contents](../../../contents.md)

### ExplosionCollisionHandler

**Lokalizacja:** [`Assets/Code/Scripts/Items/HandsWiring/ExplosionCollisionHandler.cs`](../../../../Assets/Code/Scripts/Items/HandsWiring/ExplosionCollisionHandler.cs)

Klasa `ExplosionCollisionHandler` zarządza wykrywaniem kolizji eksplozji z przeciwnikami, zapisując informacje o trafionych wrogach, aby uniknąć ich ponownego trafienia podczas jednej eksplozji.

#### Pola Publiczne

- `List<GameObject> hitEnemies` - Lista przeciwników, którzy zostali trafieni przez eksplozję. Pozwala na jednorazowe zaliczenie obrażeń, zapobiegając wielokrotnemu trafieniu tego samego wroga.

#### Pola Prywatne

- `CircleCollider2D circleCollider` - Referencja do komponentu `CircleCollider2D`, który definiuje zasięg eksplozji.

#### Metody

- `void Start()` - Inicjalizuje `CircleCollider2D`:
    - Ustawia referencję do `circleCollider`.
    - Zakomentowana linia `circleCollider.radius = range;` sugeruje, że można dynamicznie ustawiać zasięg eksplozji.

- `void OnCollisionStay2D(Collision2D collidingObject)` - Wykrywa, czy obiekt jest przeciwnikiem (`Enemy`) i dodaje go do `hitEnemies`:
    - Sprawdza, czy obiekt kolidujący ma tag `Enemy`.
    - Jeśli przeciwnik nie znajduje się na liście `hitEnemies`, dodaje go, aby odnotować, że został już trafiony.

- `void OnCollisionExit2D(Collision2D collidingObject)` - Usuwa obiekt z `hitEnemies` po zakończeniu kolizji:
    - Zakomentowana linia `hitEnemies.Remove(collidingObject.gameObject);` sugeruje, że lista może być czyszczona, gdy obiekt opuszcza zasięg kolizji.

- `void Update()` - Aktualizuje zasięg eksplozji, jeśli wartość `range` została zmieniona:
    - Zakomentowany fragment kodu porównuje wartość `range` z promieniem `circleCollider` i dostosowuje promień, jeśli wartości się różnią.
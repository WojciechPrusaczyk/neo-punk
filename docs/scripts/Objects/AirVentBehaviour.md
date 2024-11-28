[Go back to code contents](../../codeContents.md)

### AirVentBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/Objects/AirVentBehaviour.cs`](../../../Assets/Code/Scripts/Objects/AirVentBehaviour.cs)

Klasa `AirVentBehaviour` symuluje działanie strumienia powietrza, który odpycha gracza w górę, gdy ten znajduje się w jego zasięgu.

#### Pola Publiczne

- `float pushForce` - siła, z jaką strumień powietrza odpycha gracza. Domyślnie ustawiona na `10.0f`.

#### Pola Prywatne

- `GameObject player` - referencja do obiektu gracza, uzyskiwana przy uruchomieniu skryptu.

#### Metody

- `void Start()` - Inicjalizuje skrypt, znajdując obiekt gracza na scenie i przypisując go do pola `player`.
- `void OnCollisionStay2D(Collision2D collision)` - Wywoływana podczas trwania kolizji z innym obiektem. Jeśli kolidującym obiektem jest gracz (`collision.gameObject.CompareTag("Player")`):
    - Pobiera komponenty `Rigidbody2D` oraz `Player` z obiektu gracza.
    - Jeśli komponenty są dostępne, aplikuje siłę `pushForce` w górę (`Vector2.up * pushForce`) przy użyciu trybu `ForceMode2D.Impulse`, aby odpychać gracza.


[Go back to contents](../../contents.md)

### BulletBehaviour

**Lokalizacja:** [`Assets/Code/Scripts/Objects/BulletBehaviour.cs`](../../../Assets/Code/Scripts/Objects/BulletBehaviour.cs)

Klasa `BulletBehaviour` definiuje zachowanie pocisku, który może zadawać obrażenia graczowi po kolizji.

#### Pola Prywatne

- `GameObject shooter` - referencja do obiektu, który wystrzelił pocisk, ustawiana przy wywołaniu metody `SetShooter`.

#### Metody

- `void SetShooter(GameObject shooter)` - Ustawia obiekt `shooter` jako strzelca odpowiedzialnego za wystrzelenie pocisku. Dzięki temu można przypisać obrażenia do konkretnej jednostki.

- `void OnCollisionEnter2D(Collision2D collision)` - Wywoływana przy kolizji pocisku z innym obiektem:
    - Sprawdza, czy kolizja nastąpiła z graczem (`collision.gameObject.CompareTag("Player")`) oraz czy pocisk ma przypisanego strzelca (`null != shooter`).
    - Jeśli warunki są spełnione:
        - Pobiera komponent `EntityStatus` gracza i strzelca.
        - Wywołuje metodę `DealDamage` na obiekcie gracza, zadając obrażenia o wartości zależnej od ataku strzelca (`shooterStatus.GetAttackDamageCount()`).
        - Niszczy pocisk (`Destroy(gameObject)`) po zadaniu obrażeń.
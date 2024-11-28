[Go back to code contents](../../../codeContents.md)

### VoodooDollAbilities

**Lokalizacja:** [`Assets/Code/Scripts/Items/VoodooDoll/VoodooDollAbilities.cs`](../../../../Assets/Code/Scripts/Items/VoodooDoll/VoodooDollAbilities.cs)

`VoodooDollAbilities` to klasa implementująca interfejs `ItemData.IItemAbility`. Klasa zarządza specjalnymi zdolnościami przedmiotu "Voodoo Doll", który zwiększa obrażenia zadawane przez gracza oraz dodaje efekt stosu igieł, aktywowanego przy każdej utracie życia przez gracza. Po osiągnięciu określonej liczby stosów, gracz ginie.

#### Pola Prywatne

- `float damageIncreasePercentage` - Procentowy wzrost obrażeń podczas aktywacji zdolności.
- `float effectDuration` - Czas trwania efektu wzrostu obrażeń.
- `float needleStacks` - Liczba stosów igieł, które rosną przy każdej utracie życia.
- `EntityStatus playerStatus` - Komponent `EntityStatus` gracza, służący do modyfikacji zdrowia i obrażeń.
- `PlayerInventory playerInventory` - Komponent `PlayerInventory`, zarządzający ekwipunkiem gracza.
- `float lastNoticedPlayerHp` - Ostatnia wartość zdrowia gracza, służąca do sprawdzania utraty zdrowia.

#### Metody

- **`void Initialize(float damageIncreasePercentage, float effectDuration)`**
    - Inicjalizuje podstawowe parametry, takie jak procentowy wzrost obrażeń oraz czas trwania efektu. Ustawia również początkową wartość `needleStacks` na `0` i `lastNoticedPlayerHp` na `0`.

- **`void Use()`**
    - Aktywuje zdolność zwiększania obrażeń:
        - Pobiera i zapisuje komponent `EntityStatus` gracza, jeśli jeszcze nie jest ustawiony.
        - Zwiększa bieżące obrażenia gracza o określony procent (`damageIncreasePercentage`) i uruchamia coroutine `ResetDamageAfterDuration`, aby przywrócić oryginalne obrażenia po upływie `effectDuration`.

- **`IEnumerator ResetDamageAfterDuration(EntityStatus playerStatus, float baseDamage)`**
    - Coroutine resetująca obrażenia gracza po określonym czasie (`effectDuration`):
        - Czeka na czas trwania efektu, a następnie przywraca wartość obrażeń gracza do `baseDamage`.

- **`void Apply()`**
    - Zapewnia pasywny efekt przedmiotu:
        - Ustawia referencje do `EntityStatus` i `PlayerInventory`, jeśli jeszcze nie są przypisane.
        - Sprawdza, czy gracz stracił zdrowie od czasu ostatniej aktualizacji, a jeśli tak, zwiększa `needleStacks`.
        - Aktualizuje ikonę w ekwipunku gracza, aby pokazać odpowiedni poziom stosów igieł.
        - Jeśli `needleStacks` przekracza wartość `3`, wywołuje metodę `PlayerDeathEvent`, kończąc grę.

- **`void Remove()`**
    - Wyłącza efekt przedmiotu:
        - Przywraca obrażenia gracza do wartości bazowej po usunięciu przedmiotu lub zakończeniu efektu.
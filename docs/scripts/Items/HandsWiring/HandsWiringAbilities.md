[Go back to code contents](../../../codeContents.md)

### HandsWiringAbilities

**Lokalizacja:** [`Assets/Code/Scripts/Items/HandsWiring/HandsWiringAbilities.cs`](../../../../Assets/Code/Scripts/Items/HandsWiring/HandsWiringAbilities.cs)

`HandsWiringAbilities` to klasa dziedzicząca po `ScriptableObject`, implementująca interfejs `ItemData.IItemAbility`. Klasa ta zarządza zdolnościami związanymi z przedmiotem "Hands Wiring", który zmienia podstawowy typ obrażeń na elektryczny i wywołuje wybuchowy efekt obszarowy, zadający obrażenia i ogłuszający przeciwników.

#### Pola Prywatne

- `float explosionForce` - Siła wybuchu działająca na wrogów w zasięgu eksplozji.
- `float explosionRange` - Zasięg eksplozji w jednostkach.
- `float damageDealt` - Procent obrażeń zadawanych podczas wybuchu, w stosunku do obrażeń bazowych gracza.
- `GameObject explosionEffectPrefab` - Prefab efektu eksplozji.
- `Player player` - Referencja do gracza.
- `bool isEffectActive` - Flaga informująca, czy efekt wybuchu jest aktywny.
- `float effectEndTime` - Czas zakończenia aktywnego efektu.
- `int lastElementalType` - Typ elementarny przed zastosowaniem efektu, umożliwiający przywrócenie oryginalnego typu.
- `bool isPassiveGranted` - Flaga informująca, czy pasywny bonus został aktywowany.

#### Metody

- **`void Initialize(GameObject _explosionEffectPrefab, float _explosionForce, float _explosionRange, float _damageDealt)`**
    - Inicjalizuje efekt wybuchu oraz ustawia parametry takie jak prefab eksplozji, siłę, zasięg oraz procent obrażeń.

- **`void Use()`**
    - Uruchamia aktywną zdolność przedmiotu:
        - Sprawdza, czy `explosionEffectPrefab` jest ustawiony, a następnie tworzy instancję wybuchu przy pozycji gracza.
        - Dodaje siłę wybuchu do wrogów w zasięgu oraz zadaje im obrażenia bazujące na `damageDealt`.
        - Wyświetla błąd w konsoli, jeśli `explosionEffectPrefab` jest pusty.

- **`void Apply()`**
    - Zapewnia pasywny efekt przedmiotu:
        - Znajduje obiekt `Player`, jeśli jeszcze nie został zainicjowany.
        - Ustawia typ elementarny gracza na elektryczny, jeśli bonus pasywny nie jest jeszcze aktywny.
        - Sprawdza, czy efekt jest aktywny, a jeśli czas trwania upłynął, wyłącza efekt i przywraca oryginalny typ elementarny gracza.

- **`void Remove()`**
    - Wyłącza efekt przedmiotu:
        - Przywraca oryginalny typ elementarny gracza, jeśli efekt był aktywny.
        - Wyłącza pasywny bonus, ustawiając `isPassiveGranted` na `false`.
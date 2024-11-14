[Go back to contents](../../../contents.md)

### DemonBellAbilities

**Lokalizacja:** [`Assets/Code/Scripts/Items/DemonBell/DemonBellAbilities.cs`](../../../../Assets/Code/Scripts/Items/DemonBell/DemonBellAbilities.cs)

Klasa `DemonBellAbilities` implementuje zdolności specjalne przedmiotu "Demon Bell". Przedmiot ten tymczasowo zwiększa obrażenia zadawane przez gracza kosztem większej podatności na obrażenia. Główne zdolności obejmują pasywne bonusy oraz umiejętność aktywacji dodatkowego efektu, który zmienia typ obrażeń na czas określony.

#### Pola Prywatne

- `EntityStatus playerStatus` - Referencja do komponentu `EntityStatus` gracza, odpowiedzialnego za zarządzanie jego statystykami, takimi jak obrażenia i obrona.
- `Player player` - Referencja do komponentu `Player` gracza, który umożliwia zmianę typu obrażeń.
- `float effectDuration` - Czas trwania aktywnego efektu w sekundach.
- `bool isEffectActive` - Flaga określająca, czy aktywny efekt jest włączony.
- `float effectEndTime` - Czas, w którym efekt przestanie działać.
- `int lastElementalType` - Ostatnio używany typ obrażeń gracza, przed aktywacją efektu.
- `float additionalDamage` - Procentowy bonus do obrażeń zadawanych przez gracza.
- `float addedDamage` - Rzeczywista wartość dodanych obrażeń.
- `float defenceLoweringPercent` - Procentowa wartość obniżenia odporności gracza.
- `float loweredDefence` - Wartość dodana do podatności na obrażenia.
- `bool isDamageBonusGranted` - Flaga określająca, czy pasywne bonusy zostały zastosowane.

#### Metody

- `void Initialize(float _additionalDamage, float _defenceLoweringPercent, float _effectDuration)` - Inicjalizuje parametry efektu:
    - `_additionalDamage` - Procentowy bonus do obrażeń.
    - `_defenceLoweringPercent` - Procentowy spadek odporności na obrażenia.
    - `_effectDuration` - Czas trwania aktywnego efektu.

- `void Use()` - Aktywuje zdolność przedmiotu:
    - Jeśli efekt nie jest aktywny, ustawia flagę `isEffectActive` na `true` oraz oblicza czas zakończenia efektu.
    - Zapisuje aktualny typ obrażeń gracza i zmienia go na typ "Bloody" (ID `5`), co zwiększa obrażenia.

- `void Apply()` - Zastosowuje pasywne efekty zdolności:
    - Inicjalizuje referencje do komponentów `Player` i `EntityStatus`, jeśli są puste.
    - Dodaje do `AttackDamage` gracza obrażenia z `addedDamage`, zwiększa też podatność na obrażenia o `loweredDefence`.
    - Jeśli aktywny efekt dobiega końca, przywraca pierwotny typ obrażeń gracza i wyłącza efekt.

- `void Remove()` - Usuwa efekty zdolności:
    - Przywraca oryginalne wartości `AttackDamage` i podatności na obrażenia.
    - Resetuje typ obrażeń gracza do ostatniego elementarnego przed aktywacją przedmiotu.
    - Ustawia flagę `isDamageBonusGranted` na `false`, wskazując, że pasywne bonusy zostały wyłączone.
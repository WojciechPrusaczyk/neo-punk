[Go back to contents](../../contents.md)

### EntityStatus

**Lokalizacja:** [`Assets/Code/Scripts/Entities/EntityStatus.cs`](../../../Assets/Code/Scripts/Entities/EntityStatus.cs)

Klasa `EntityStatus` zarządza atrybutami, zdrowiem, doświadczeniem, interakcjami bojowymi oraz wizualnym sprzężeniem zwrotnym jednostki w grze.

#### Pola Prywatne

- `GameObject detectedTarget` - obiekt wykrywany jako cel jednostki.
- `float attackRange` - zakres, w którym jednostka może przeprowadzić atak.
- `Color lightDamageColor` - kolor używany do sygnalizacji lekkich obrażeń.
- `Color heavyDamageColor` - kolor używany do sygnalizacji ciężkich obrażeń.
- `Color deathColor` - kolor sygnalizujący śmierć jednostki.
- `LootTable lootTable` - tabela przedmiotów możliwych do upuszczenia po śmierci jednostki.
- `SpriteRenderer spriteRenderer` - komponent odpowiedzialny za renderowanie grafiki jednostki.

#### Pola Publiczne

- `string entityName` - nazwa jednostki.
- `int entityLevel` - poziom jednostki, określający jej siłę.
- `int entityExperiencePoints` - aktualna liczba punktów doświadczenia jednostki.
- `int entityExperienceToNextLvl` (readonly) - liczba punktów doświadczenia potrzebna do osiągnięcia kolejnego poziomu.
- `float entityHealthPoints` - aktualna liczba punktów zdrowia jednostki.
- `float entityMaxHealth` - maksymalna liczba punktów zdrowia jednostki.
- `int droppedXp` - liczba punktów doświadczenia, które jednostka upuszcza po śmierci.
- `int gold` - ilość złota posiadana przez jednostkę.
- `float AttackDamage` - wartość obrażeń zadawanych przez jednostkę.
- `float MovementSpeed` - szybkość poruszania się jednostki.

#### Metody

1. **SetName(string name)**
    - Ustawia nazwę jednostki.

2. **GetName()**
    - Zwraca nazwę jednostki.

3. **SetLevel(int level)**
    - Ustawia poziom jednostki.

4. **GetLevel()**
    - Zwraca aktualny poziom jednostki.

5. **SetXp(int xp)**
    - Ustawia liczbę punktów doświadczenia jednostki.

6. **GetXp()**
    - Zwraca aktualną liczbę punktów doświadczenia jednostki.

7. **AddXp(int xpAmount)**
    - Dodaje punkty doświadczenia oraz przeprowadza awans na wyższy poziom, jeśli osiągnięto wymaganą liczbę punktów.

8. **SetHp(float hp)**
    - Ustawia bieżącą liczbę punktów zdrowia jednostki.

9. **GetHp()**
    - Zwraca bieżącą liczbę punktów zdrowia jednostki.

10. **SetMaxHp(float maxHp)**
    - Ustawia maksymalną liczbę punktów zdrowia jednostki.

11. **GetMaxHp()**
    - Zwraca maksymalną liczbę punktów zdrowia jednostki.

12. **DealDamage(float damage, GameObject attackingEntity = null)**
    - Przetwarza obrażenia zadawane jednostce, uwzględniając blokowanie i kierunek ataku, jeśli jednostka jest graczem.

13. **DeathEvent()**
    - Wywołuje zdarzenie śmierci jednostki, upuszczając łup i przyznając punkty doświadczenia graczowi.

14. **PlayerDeathEvent()**
    - Obsługuje specyficzne czynności w przypadku śmierci gracza.

15. **GettingDamageEvent(float damage)**
    - Aktualizuje punkty zdrowia jednostki i zmienia jej kolor na krótki czas w zależności od poziomu obrażeń.

16. **SygnalizeGettingDamage()**
    - Animacja wizualna sygnalizująca otrzymanie obrażeń przez gracza.

17. **MoveHpBar(float xOffset, float yOffset)**
    - Przesuwa pozycję paska zdrowia jednostki, aby symulować reakcję na obrażenia.

18. **ChangeColorForInterval(Color color, float duration)**
    - Zmienia kolor grafiki jednostki na określony czas.

19. **DeathAnimation(Color color, float duration)**
    - Odtwarza animację wizualną podczas śmierci jednostki przed jej zniszczeniem.

20. **SetExpToNextLvl(int expToNextLvl)**
    - Ustawia liczbę punktów doświadczenia potrzebną do osiągnięcia kolejnego poziomu.

21. **GetExpToNextLvl()**
    - Zwraca liczbę punktów doświadczenia potrzebną do awansu na kolejny poziom.

22. **SetGold(int gold)**
    - Ustawia liczbę złota posiadaną przez jednostkę.

23. **GetGold()**
    - Zwraca aktualną liczbę złota jednostki.

24. **AddGold(int gold)**
    - Dodaje złoto do bilansu jednostki i aktualizuje interfejs użytkownika, jeśli jednostką jest gracz.

25. **SetAttackDamageCount(float AttackDamage)**
    - Ustawia wartość obrażeń zadawanych przez jednostkę.

26. **GetAttackDamageCount()**
    - Zwraca bieżącą wartość obrażeń zadawanych przez jednostkę.

27. **SetMovementSpeed(float MovementSpeed)**
    - Ustawia szybkość poruszania się jednostki.

28. **GetMovementSpeed()**
    - Zwraca bieżącą szybkość poruszania się jednostki.

---

**Uwaga**: Upewnij się, że komponenty `LootTable` i `SpriteRenderer` są podłączone do obiektu jednostki dla pełnej funkcjonalności.

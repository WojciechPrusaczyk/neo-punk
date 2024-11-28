[Go back to code contents](../../../codeContents.md)

### HandsWiring

**Lokalizacja:** [`Assets/Code/Scripts/Items/DemonBell/DemonBell.cs`](../../../../Assets/Code/Scripts/Items/DemonBell/DemonBell.cs)

Klasa `HandsWiring` rozszerza obiekt `ItemData` i przechowuje wszystkie informacje o przedmiocie, jego statystyki i przeliczniki obrażeń.

#### Atrybuty klasy

- `float additionalDamage`  
  Dodatkowe obrażenia oferowane przez przedmiot po użyciu.  
  **0.25f**
---
- `float defenceLoweringPercent`  
  Zmiejszona obrona po użyciu przedmiotu.  
  **0.35f**
---
- `float effectDuration`  
  Długość trwania efektu.
---
- `DemonBellAbilities itemAbility`  
  Pole obiektu typu `HandsWiringAbilities`, które przechowuje umiejętności przedmiotu.
---
- `string itemName`  
  Nazwa przedmiotu.
  **Demon Bell**
---
- `string passiveDescription`  
  Opis zdolności pasywnej.  
  **Player takes 35% more damage, but also deals 25% more.**
---
- `string activeDescription`  
  Opis zdolności aktywnej.  
  **Overrides damage type to Bloody for 10 seconds.**
---
- `string rarity`  
  Rzadkość efektu.  
  **Rare**
---
- `float cooldown`  
  Czas odnowienia umiejętności przedmiotu.  
  **40.0f**
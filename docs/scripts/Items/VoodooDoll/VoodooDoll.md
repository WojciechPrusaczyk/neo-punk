[Go back to code contents](../../../codeContents.md)

### VoodooDoll

**Lokalizacja:** [`Assets/Code/Scripts/Items/VoodooDoll/VoodooDoll.cs`](../../../../Assets/Code/Scripts/Items/VoodooDoll/VoodooDoll.cs)

Klasa `VoodooDoll` rozszerza obiekt `ItemData` i przechowuje wszystkie informacje o przedmiocie, jego statystyki i przeliczniki obrażeń.

#### Atrybuty klasy

- `float defenceIncreasePercentage`  
  Procent zwiększenia obrażeń przedmiotu.  
  **0.4f**
---
- `float effectDuration`  
  Czas trwania efektu przedmiotu.  
  **10.0f**
---
- `VoodooDollAbilities itemAbility`  
  Pole obiektu typu `VoodooDollAbilities`, które przechowuje umiejętności przedmiotu.
---
- `string itemName`  
  Nazwa przedmiotu.  
  **Voodoo Doll**
---
- `string passiveDescription`  
  Opis zdolności pasywnej.  
  **Increases player attack by 40% for 10 seconds.**
---
- `string activeDescription`  
  Opis zdolności aktywnej.  
  **Gives 40% more damage on use, but if you get hit, doll gets a needle stack. If you get hit when Doll have 3 stacks you will die.**
---
- `string rarity`  
  Rzadkość efektu.  
  **Quantum**
---
- `float cooldown`  
  Czas odnowienia umiejętności przedmiotu.  
  **30.0f**
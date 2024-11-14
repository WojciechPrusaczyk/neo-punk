[Go back to contents](../../../contents.md)

### DemonBell

**Lokalizacja:** [`Assets/Code/Scripts/Items/HandsWiring/HandsWiring.cs`](../../../../Assets/Code/Scripts/Items/HandsWiring/HandsWiring.cs)

Klasa `DemonBell` rozszerza obiekt `ItemData` i przechowuje wszystkie informacje o przedmiocie, jego statystyki i przeliczniki obrażeń.

#### Atrybuty klasy

- `GameObject explosionEffectPrefab`  
  Pole prefaba explozji. Obiekt jest tworzony w miejscu użycia.
---
- `float explosionForce`  
  Siła eksplozji.  
  **5.0f**
---
- `float explosionRange`  
  Zasieg eksplozji.  
  **1.0f**
---
- `float damageDealt`  
  Obrażenia zadawane z pomocą eksplozji.  
  **10.0f**
---
- `DemonBellAbilities itemAbility`  
  Pole obiektu typu `DemonBellAbilities`, które przechowuje umiejętności przedmiotu.
---
- `string itemName`  
  Nazwa przedmiotu.
  **Hands Wiring**
---
- `string passiveDescription`  
  Opis zdolności pasywnej.  
  **Changes basic type of damage to electric.**
---
- `string activeDescription`  
  Opis zdolności aktywnej.  
  **Produces electric discharge, which deals damage equal 20% of player AD and disables cybernetic enemies for 2 seconds.**
---
- `string rarity`  
  Rzadkość efektu.  
  **Common**
---
- `float cooldown`  
  Czas odnowienia umiejętności przedmiotu.  
  **11.0f**
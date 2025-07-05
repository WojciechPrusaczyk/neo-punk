using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HealingFlask", menuName = "NeoPunk/Items/HealingFlask", order = 2)]
public class Flask : ItemData
{
    public float healPercent = 0.45f;
    public float maxHealthBoostPercent = 0.1f;

    private void OnEnable()
    {
        currentCooldown = 0;
        Initialize();
    }

    public override void Initialize()
    {
        currentCooldown = 0;
        var abilities = new FlaskAbilities();
        abilities.Initialize(maxHealthBoostPercent, healPercent);
        itemAbility = abilities;

        itemName = "Flaszka lecząca";
        passiveDescription = "Zwiększa max HP o 10%.";
        activeDescription = "Leczy 45% HP w 2 sekundy. Leczenie ograniczone podczas działania.";
        rarity = Enums.ItemRarity.Rare;
        cooldown = 40.0f;
    }
}

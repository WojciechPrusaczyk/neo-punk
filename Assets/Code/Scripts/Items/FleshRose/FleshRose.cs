using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FleshRose", menuName = "NeoPunk/Items/FleshRose", order = 2)]
public class FleshRose : ItemData
{
    public float thornsPercent = 0.2f;
    public float bloodBoltDamagePercent = 1.2f;
    public float selfDamagePercent = 0.1f;
    
    public GameObject bloodBoltPrefab;

    private void OnEnable()
    {
        currentCooldown = 0;
        Initialize();
    }

    public override void Initialize()
    {
        currentCooldown = 0;
        var abilities = new FleshRoseAbilities();
        abilities.Initialize(thornsPercent, bloodBoltDamagePercent, selfDamagePercent, bloodBoltPrefab);
        itemAbility = abilities;

        itemName = "Flesh Rose";
        passiveDescription = "Enemies receive 20% of the damage they deal.";
        activeDescription = "For the next 3 attacks, you shoot blood projectiles (120% damage), but each one deals 10% damage to yourself.";
        rarity = Enums.ItemRarity.Rare;
        cooldown = 20;
    }
}

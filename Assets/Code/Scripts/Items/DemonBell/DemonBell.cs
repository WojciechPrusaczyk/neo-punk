using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DemonBell", menuName = "NeoPunk/Items/DemonBell", order = 2)]
public class DemonBell : ItemData
{
    public float additionalDamage = 0.25f;
    public float defenceLoweringPercent = 0.35f;
    public float effectDuration = 10.0f;

    private void OnEnable()
    {
        currentCooldown = 0;
        Initialize();
    }

    public override void Initialize()
    {
        currentCooldown = 0;
        //itemAbility = new DemonBellAbilities(0.25f, 0.35f, 10.0f);
        DemonBellAbilities abilities = ScriptableObject.CreateInstance<DemonBellAbilities>();
        abilities.Initialize(0.25f, 0.35f, 10.0f);
        itemAbility = abilities;
        itemName = "Demon Bell";
        passiveDescription = "Player takes 35% more damage, but also deals 25% more.";
        activeDescription = "Overrides damage type to Bloody for 10 seconds.";
        rarity = "Rare";
        cooldown = 40.0f;
    }
}
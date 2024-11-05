using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoodooDoll", menuName = "NeoPunk/Items/VoodooDoll", order = 2)]
public class VoodooDoll : ItemData
{
    private void OnEnable()
    {
        currentCooldown = 0;
        Initialize();
    }

    public override void Initialize()
    {
        currentCooldown = 0;
        VoodooDollAbilities abilities = new VoodooDollAbilities();
        abilities.Initialize(0.4f, 10f);
        itemAbility = abilities;

        itemName = "Voodoo Doll";
        passiveDescription = "Increases player attack by 40% for 10 seconds.";
        activeDescription = "Gives 40% more damage on use, but if you get hit, doll gets a needle stack. If you get hit when Doll have 3 stacks you will die.";
        rarity = "Quantum";
        cooldown = 30.0f;
    }
}
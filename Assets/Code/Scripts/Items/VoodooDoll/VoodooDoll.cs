using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VoodooDoll", menuName = "NeoPunk/Items/VoodooDoll", order = 2)]
public class VoodooDoll : ItemData
{
    public float defenceIncreasePercentage = 0.4f;
    public float effectDuration = 10.0f;
    public Sprite itemIconOneStack;
    public Sprite itemIconTwoStacks;
    public Sprite itemIconThreeStacks;
    private void OnEnable()
    {
        currentCooldown = 0;
        Initialize();
    }

    public override void Initialize()
    {
        currentCooldown = 0;
        VoodooDollAbilities abilities = new VoodooDollAbilities();
        abilities.Initialize(defenceIncreasePercentage, effectDuration, itemIconOneStack, itemIconTwoStacks, itemIconThreeStacks);
        itemAbility = abilities;

        itemName = "Voodoo Doll";
        passiveDescription = "Increases player attack by 40% for 10 seconds.";
        activeDescription = "Gives 40% more damage on use, but if you get hit, doll gets a needle stack. If you get hit when Doll have 3 stacks you will die.";
        rarity = Enums.ItemRarity.Quantum;
        cooldown = 30.0f;
    }
}
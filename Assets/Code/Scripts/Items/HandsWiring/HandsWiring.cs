using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandsWiring", menuName = "NeoPunk/Items/HandsWiring", order = 2)]
public class HandsWiring : ItemData
{
    
    public GameObject explosionEffectPrefab;
    public float explosionForce = 5.0f;
    public float explosionRange = 1.0f;
    [Tooltip("Percentage of player's attack dealt to enemies.")]
    public float damageDealt = 10.0f;

    private void OnEnable()
    {
        currentCooldown = 0;
        Initialize();  // Wywołanie funkcji inicjalizacyjnej
    }

    public override void Initialize()
    {
        currentCooldown = 0;

        explosionEffectPrefab = Resources.Load<GameObject>("Items/HandsWiring/HandsWiringExplosion");
        HandsWiringAbilities abilities = ScriptableObject.CreateInstance<HandsWiringAbilities>();
        abilities.Initialize(explosionEffectPrefab, explosionForce, explosionRange, damageDealt);
        itemAbility = abilities;
        
        itemName = "Hands Wiring";
        passiveDescription = "Changes basic type of damage to electric.";
        activeDescription = "Produces electric discharge, which deals damage equal 20% of player AD and disables cybernetic enemies for 2 seconds.";
        rarity = "Common";
        cooldown = 11.0f;
    }
}
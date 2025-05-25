using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "NeoPunk/Item", order = 1)]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string passiveDescription;
    public string activeDescription;
    public float cooldown;
    public float currentCooldown;
    public Enums.ItemRarity rarity;
    public float minPlayerLvl;
    public Sprite itemIcon;
    public IItemAbility itemAbility;

    public virtual void Initialize() {}

    public interface IItemAbility
    {
        void Use();
        void Apply();
        void Remove();
    }
}
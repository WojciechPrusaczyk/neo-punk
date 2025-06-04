using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackType", menuName = "NeoPunk/AttackTypes/AttackType")]
[System.Serializable]
public class AttackType : ScriptableObject
{
    public string AttackName;
    public int AttackDamage;
}

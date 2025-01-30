using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationEventShivernDog : MonoBehaviour
{
    public ShivernDog shivernDog;

    public void Attack()
    {
        shivernDog.DealDamage();
    }
}

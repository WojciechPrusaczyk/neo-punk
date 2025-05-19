using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    public AbominationMovement abominationMovement;



    public void ClawAttackBegin()
    {
        Debug.Log("startClaw");
        abominationMovement.isAttacking = true;
    }

    public void ClawAttackEnd()
    {
        Debug.Log("endClaw");
        abominationMovement.isAttacking = false;
    }
}

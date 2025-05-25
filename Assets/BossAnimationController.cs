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
    
    
    public void ActivateColliders(string part)
    {
        switch (part)
        {
            case "Head":
                abominationMovement.headCollider.enabled = true;
                break;
            case "Claw":
                abominationMovement.clawCollider.enabled = true;
                break;
            case "Tail":
                abominationMovement.tailCollider.enabled = true;
                break;
        }
    }

    public void DeactivateColliders(string part)
    {
        switch (part)
        {
            case "Head":
                abominationMovement.headCollider.enabled = false;
                break;
            case "Claw":
                abominationMovement.clawCollider.enabled = false;
                break;
            case "Tail":
                abominationMovement.tailCollider.enabled = false;
                break;
        }
    }
}

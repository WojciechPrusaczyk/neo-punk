using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDetector : MonoBehaviour
{
    public bool isPlayerNearGround;

    public bool isFloorPassable;

    public GameObject collidingObject;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("impassableFloor"))
        {
            isPlayerNearGround = true;
            isFloorPassable = false;
            collidingObject = collision.gameObject;
        }
        else if (collision.gameObject.CompareTag("passableFloor"))
        {
            isPlayerNearGround = true;
            isFloorPassable = true;
            collidingObject = collision.gameObject;
        }
        else
        {
            isPlayerNearGround = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("passableFloor") || collision.gameObject.CompareTag("impassableFloor"))
        {
            isPlayerNearGround = false;
            isFloorPassable = false;
            collidingObject = null;
        }
    }
}
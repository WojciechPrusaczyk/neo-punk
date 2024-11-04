using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private GameObject parentEntity;
    void Start()
    {
        parentEntity = gameObject.transform.parent.gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject collidingObject = collision.gameObject;
        if (collision.CompareTag("Player"))
        {
            parentEntity.GetComponent<EntityStatus>().detectedTarget = collidingObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        parentEntity.GetComponent<EntityStatus>().detectedTarget = null;
    }
}
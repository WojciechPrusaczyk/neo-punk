using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCollisionHandler : MonoBehaviour
{
    public List<GameObject> hitEnemies = new List<GameObject>();
    //public float range = 0.0f;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        circleCollider = gameObject.GetComponent<CircleCollider2D>();

        //circleCollider.radius = range;
    }

    private void OnCollisionStay2D(Collision2D collidingObject)
    {
        if (collidingObject.gameObject.CompareTag("Enemy"))
        {
            if (!hitEnemies.Contains(collidingObject.gameObject))
            {
                hitEnemies.Add(collidingObject.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collidingObject)
    {
        //hitEnemies.Remove(collidingObject.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*if (range != circleCollider.radius)
        {
            circleCollider.radius = range;
        }*/
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVentBehaviour : MonoBehaviour
{
    private GameObject player;
    public float pushForce = 10.0f;
    private void Start()
    {
        player = GameObject.Find("Player").gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();

            playerBody.AddForce(Vector2.up * pushForce , ForceMode2D.Impulse);
        }
    }
}
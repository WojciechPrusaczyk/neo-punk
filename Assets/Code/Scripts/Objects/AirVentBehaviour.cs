using System.Collections;
using UnityEngine;

public class AirVentBehaviour : MonoBehaviour
{
    public float pushForce = 10.0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D playerBody = collision.GetComponent<Rigidbody2D>();

            if (playerBody != null)
            {
                Vector2 newVelocity = playerBody.velocity;
                newVelocity.y = pushForce;
                playerBody.velocity = newVelocity;
            }
        }
    }
}
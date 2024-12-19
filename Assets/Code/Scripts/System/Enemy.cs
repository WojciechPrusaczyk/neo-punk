using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Wandering,
        Chasing,
    }
    [Header("Enemy variables")] 
    public float jumpForce = 6f;

    public float playerDetectionRange = 10f;
    public Transform targetA;
    public Transform targetB;
    public Transform targetPoint;
    public EnemyState state;

    public Transform eyes;
    /*
     * Zmienne lokalne
     */
    private Transform playerPosition;
    private Rigidbody2D rb;
    private CircleCollider2D circle;
    [SerializeField]
    private EntityStatus enemyStatus;
    [SerializeField]
    private FloorDetector floorDetector;

    [SerializeField] 
    private Vector2 direction;
    [SerializeField]
    private float distanceToPlayer;
    
    private float obstacleDetectionDistance = 1f; // Distance to detect obstacles
    private LayerMask obstacleLayer;
    private float jumpCooldown = 1f;
    private float lastJumpTime;
    
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        targetPoint = targetA;
    }

    private void Update()
    {
        distanceToPlayer = Vector2.Distance(playerPosition.position, transform.position);
        CheckForDirection();
        if (distanceToPlayer > playerDetectionRange)
        {
            Debug.Log("Wandering");
            Wander();
        }
        
        else if (distanceToPlayer > enemyStatus.attackRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    private void Wander()
    {
        direction = (targetPoint.position - transform.position).normalized; // Get direction
        rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, rb.velocity.y); // Apply velocity

        // Check if the enemy has reached the target point
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.5f)
        {
            // Switch target point
            targetPoint = targetPoint == targetA ? targetB : targetA;
        }
    }

    private void ChasePlayer()
    {
        if (IsObstacleAhead() && floorDetector.isPlayerNearGround)
        {
            Jump();
        }
        Vector2 difference = playerPosition.position - transform.position;
        Vector2 diffNormalized = difference.normalized;

        direction = new Vector2(Mathf.Round(diffNormalized.x), Mathf.Round(diffNormalized.y));

        float xMovement = direction.x * enemyStatus.MovementSpeed * Time.deltaTime;
            
        rb.velocity = new Vector2(xMovement, rb.velocity.y);
    }

    private void CheckForDirection()
    {
        if (direction.x < 0)
        {
            eyes.localPosition = new Vector2(-0.55f, eyes.localPosition.y);
        }
        else
        {
            eyes.localPosition = new Vector2(0.55f, eyes.localPosition.y);
        }
    }

    private bool IsObstacleAhead()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(eyes.position, rayDirection, obstacleDetectionDistance);

        if (hit.collider != null)
        {
            CustomTags tags = hit.collider.gameObject.GetComponent<CustomTags>();
            if (tags == null)
            {
                return false;
            }
            if (tags.HasTag("Wall"))
            {
                return true;
            }
        }

        return false;
    }
    
    private void Jump()
    {if (Time.time >= lastJumpTime + jumpCooldown)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            lastJumpTime = Time.time;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        Debug.DrawRay(eyes.position, rayDirection * obstacleDetectionDistance, Color.red);
    }
}

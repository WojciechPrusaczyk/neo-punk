using System;
using UnityEngine;

public class WanderingEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Wandering,
        Chasing,
    }
    [Header("Enemy Transforms")] 
    public Transform targetA;
    public Transform targetB;
    public Transform targetPoint;
    public Transform eyes;
    public Transform maxJumpHeight;
    public Transform floor;
    
    private Transform playerPosition;

    [Header("Enemy variables")] 
    public float obstacleDetectionDistance = 1f; // Distance to detect obstacles   

    public float chaseSpeedMultiplier = 2f;
    public float playerDetectionRange = 10f;
    public EnemyState state;
    
    [Header("Enemy Scripts")] 
    [SerializeField]
    private EntityStatus enemyStatus;
    [SerializeField]
    private FloorDetector floorDetector;
    
    [Header("Misc")] 
    [SerializeField] 
    private Vector2 direction;
    [SerializeField]
    private float distanceToPlayer;
    /*
     * Zmienne lokalne
     */
    private Rigidbody2D rb;
    private CircleCollider2D circle;

    
    private LayerMask obstacleLayer;
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
        state = EnemyState.Wandering;
        if (IsObstacleAhead() && canJumpOverWall() && floorDetector.isPlayerNearGround)
        {
            Jump();
        }

        if (IsObstacleAhead())
        {
            return;
        }
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
        state = EnemyState.Chasing;
        if (IsObstacleAhead() && canJumpOverWall() && floorDetector.isPlayerNearGround)
        {
            Jump();
        }

        if (IsObstacleAhead())
        {
            return;
        }
        Vector2 difference = playerPosition.position - transform.position;
        Vector2 diffNormalized = difference.normalized;

        direction = new Vector2(Mathf.Round(diffNormalized.x), Mathf.Round(diffNormalized.y));

        float xMovement = direction.x * enemyStatus.MovementSpeed * Time.deltaTime * chaseSpeedMultiplier;
            
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
            if (hit.collider.CompareTag("impassableFloor"))
            {
                return true;
            }
            return false;
        }
        return false;
    }
    
    private void Jump()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float yDistance = Mathf.Abs(maxJumpHeight.position.y - floor.position.y);
        float initialVelocity = Mathf.Sqrt(2 * gravity * yDistance * 1.2f);

        // Apply the force
        rb.velocity = new Vector2(rb.velocity.x, initialVelocity);
    }

    private bool canJumpOverWall()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(maxJumpHeight.position, rayDirection, obstacleDetectionDistance  * 2f);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("impassableFloor"))
            {
                return false;
            }
            return true;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        Debug.DrawRay(eyes.position, rayDirection * obstacleDetectionDistance, Color.red);
        
        Debug.DrawRay(maxJumpHeight.position, rayDirection * obstacleDetectionDistance * 2f, Color.green);
    }
}

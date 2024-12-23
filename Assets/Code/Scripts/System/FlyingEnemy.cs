using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Wandering,
        Chasing,
    }
    [Header("Enemy Transforms")] 
    public Transform offsetTransform;    
    public Transform targetA;
    public Transform targetB;
    public Transform targetPoint;
    public Transform eyes;
    
    private Transform playerPosition;

    [Header("Enemy variables")] 
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
    private float xOffset;
    private Rigidbody2D rb;
    private CircleCollider2D circle;

    
    private float obstacleDetectionDistance = 1f; // Distance to detect obstacles
    private LayerMask obstacleLayer;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        targetPoint = targetA;
        
        xOffset = offsetTransform.localPosition.x;
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
        direction = (targetPoint.position - transform.position).normalized; // Get direction
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, enemyStatus.MovementSpeed * Time.deltaTime);

        // Check if the enemy has reached the current target
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Switch to the other target
            targetPoint = (targetPoint == targetA) ? targetB : targetA;
        }
    }

    private void CheckForDirection()
    {
        if (direction.x < 0)
        {
            eyes.localPosition = new Vector2(-0.55f, eyes.localPosition.y);
            offsetTransform.localPosition = new Vector2(-xOffset, offsetTransform.localPosition.y);
        }
        else
        {
            eyes.localPosition = new Vector2(0.55f, eyes.localPosition.y);
            offsetTransform.localPosition = new Vector2(xOffset, offsetTransform.localPosition.y);

        }
    }
    
    private void ChasePlayer()
    {
        direction = (playerPosition.position - transform.position).normalized; // Get direction
        Vector3 targetPosition = playerPosition.position - offsetTransform.localPosition;

        Vector2 difference = targetPosition - transform.position;
        Vector2 diffNormalized = difference.normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, diffNormalized, obstacleDetectionDistance);

        if (IsObstacleAhead())
        {
            Vector2 avoidanceDirection = new Vector2(diffNormalized.x, -1).normalized;
            rb.velocity = avoidanceDirection * enemyStatus.MovementSpeed;
        }
        else
        {
            rb.velocity = diffNormalized * enemyStatus.MovementSpeed;
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
    
    private void OnDrawGizmos()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        Debug.DrawRay(eyes.position, rayDirection * obstacleDetectionDistance, Color.red);
    }
}

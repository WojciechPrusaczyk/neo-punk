using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        Wandering,
        Chasing,
    }
    [Header("Enemy variables")] 
    public Transform offsetTransform;    
    
    public float playerDetectionRange = 10f;
    public Transform targetA;
    public Transform targetB;
    public Transform targetPoint;
    public EnemyState state;
    public Transform eyes;
    
    private Transform playerPosition;
    private float xOffset;
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
        Debug.Log(rb.velocity.x);
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
        Vector2 difference = playerPosition.position - transform.position;
        Vector2 diffNormalized = difference.normalized;

        direction = new Vector2(Mathf.Round(diffNormalized.x), Mathf.Round(diffNormalized.y));
        
        Vector3 targetPosition = playerPosition.position - offsetTransform.localPosition;
        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyStatus.MovementSpeed * Time.deltaTime);

    }
    
    private void OnDrawGizmos()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        Debug.DrawRay(eyes.position, rayDirection * obstacleDetectionDistance, Color.red);
    }
}

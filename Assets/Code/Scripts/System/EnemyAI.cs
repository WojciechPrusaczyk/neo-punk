using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent((typeof(Rigidbody2D)))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(EntityStatus))]
[RequireComponent(typeof(CustomTags))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Wandering,
        Chasing
    }

    public enum EnemyType
    {
        FlyingEnemy,
        WalkingEnemy,
        JumpingEnemy
    }

    [Header("Enemy type")] 
    public EnemyType enemyType;
    
    [Header("Enemy Scripts")] 
    [SerializeField]
    private EntityStatus enemyStatus;
    [SerializeField]
    private FloorDetector floorDetector;

    
    [Header("Enemy Transforms")]
    public Transform pathParent;
    public List<Transform> pathPoints;
    public Transform eyes;
    public Transform offSet;
    public Transform maxJumpHeight;
    public Transform floor;

    private Transform playerPosition;
    
    [Header("Enemy colliders")]
    public BoxCollider2D playerAreaCollider;
    
    public Vector2 idleAreaOffset;
    public Vector2 idleAreaSize;
    
    public Vector2 alertedAreaOffset;
    public Vector2 alertedAreaSize;
    
    [Header("Enemy variables")] 
    public float obstacleDetectionDistance = 1f; // Distance to detect obstacles   

    public float chaseSpeedMultiplier = 1.2f;
    
    public EnemyState state;

    public bool canMove = true;
    public bool canAttack = false;
    
    private Vector2 moveDirection;
    public Vector2 randomMoveTime;
    public Vector2 randomMoveInvterval;
    private bool isMoving = false;
    
    
    
    [Header("Enemy components")]
    public  Rigidbody2D rb;
    public CircleCollider2D circle;
    
    [Header("Misc")] 
    [SerializeField] 
    private Vector2 direction;
    [SerializeField]
    private int currentTargetIndex = 0;
    [SerializeField]
    private GameObject player;

    private Vector2 initialEyesPosition;
    private Vector2 initialOffsetPosition;
    
    private bool isRandomWanderingJumping = false;
    private bool isRandomWanderingFlying = false;
    
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPosition = player.GetComponent<Transform>();
        
        InitializePath();
        
        playerAreaCollider.size = idleAreaSize;
        playerAreaCollider.offset = idleAreaOffset;
        
        switch (enemyType)
        {
            case EnemyType.FlyingEnemy:
                initialEyesPosition = eyes.localPosition;
                initialOffsetPosition = offSet.localPosition;
                break;
            case EnemyType.JumpingEnemy:
                initialEyesPosition = eyes.localPosition;
                break;
            case EnemyType.WalkingEnemy:
                initialEyesPosition = eyes.localPosition;
                break;
            default:
                break;
        }
    }
    
    private void Update()
    {
        canMove = !CanAttack();
        
        if(!canMove)
            return;
        switch (enemyType)
        {
            case EnemyType.FlyingEnemy:
                if(state == EnemyState.Wandering)
                    WanderFlying();
                if(state == EnemyState.Chasing)
                    ChasePlayerFlying();
                break;
            case EnemyType.JumpingEnemy:
                if(state == EnemyState.Wandering)
                    WanderJumping();
                if(state == EnemyState.Chasing)
                    ChasePlayerJumping();
                break;
            case EnemyType.WalkingEnemy:
                if(state == EnemyState.Wandering)
                    WanderWalking();
                if(state == EnemyState.Chasing)
                    ChasePlayerWalking();
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (HasLineOfSight())
            {
                state = EnemyState.Chasing;
                enemyStatus.SetIsAlerted(true);

                playerAreaCollider.size = alertedAreaSize;
                playerAreaCollider.offset = alertedAreaOffset;
                
                if(!enemyStatus.detectedTargets.Contains(other.gameObject))
                    enemyStatus.detectedTargets.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            state = EnemyState.Wandering;
            enemyStatus.SetIsAlerted(false);

            playerAreaCollider.size = idleAreaSize;
            playerAreaCollider.offset = idleAreaOffset;
            
            enemyStatus.detectedTargets.Remove(other.gameObject);

        }
    }

    private void InitializePath()
    {
        pathPoints.Clear();

        // Add all children of pathParent to pathPoints
        foreach (Transform child in pathParent)
        {
            pathPoints.Add(child);
        }
    }

    private void WanderJumping()
    {
        CheckForDirectionJumpingWalking();
        if (IsObstacleAhead() && CanJumpOverWall() && floorDetector.isPlayerNearGround)
        {
            Jump();
        }
        
        if(pathPoints.Count > 0)
        {
            Transform currentTargetPoint = pathPoints[currentTargetIndex];
            direction = (currentTargetPoint.position - transform.position).normalized; // Get direction
            rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, rb.velocity.y); // Apply velocity

            // Check if the enemy has reached the target point
            if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.5f)
            {
                // Move to the next target point in the list
                currentTargetIndex = (currentTargetIndex + 1) % pathPoints.Count;
            }
        }
        else
        {
            if (!isRandomWanderingJumping)
            {
                StartCoroutine(RandomWanderingJumpingWalking());
            }
        }
    }
    
    private void ChasePlayerJumping()
    {   
        CheckForDirectionJumpingWalking();
        if (IsObstacleAhead() && CanJumpOverWall() && floorDetector.isPlayerNearGround)
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

    private IEnumerator RandomWanderingJumpingWalking()
    {
        isRandomWanderingJumping = true;

        // Randomize a direction (-1 for left, 1 for right)
        float randomDirection = Random.value < 0.5f ? -1f : 1f;
        direction = new Vector2(randomDirection, 0).normalized;

        // Randomize move duration
        float moveDuration = Random.Range(randomMoveTime.x, randomMoveTime.y);

        // Move for the duration
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, rb.velocity.y);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Stop movement after the duration
        rb.velocity = new Vector2(0, rb.velocity.y);

        // Add a delay before the next movement
        yield return new WaitForSeconds(Random.Range(randomMoveInvterval.x, randomMoveInvterval.y));

        isRandomWanderingJumping = false; // Allow another random movement
    }
    
    private void CheckForDirectionJumpingWalking()
    {
        if (direction.x < 0)
        {
            eyes.localPosition = new Vector2(-initialEyesPosition.x, eyes.localPosition.y);
            playerAreaCollider.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            eyes.localPosition = new Vector2(initialEyesPosition.x, eyes.localPosition.y);
            playerAreaCollider.transform.localScale = new Vector3(1, 1, 1);

        }
    }
    
    private void CheckForDirectionFlying()
    {
        if (direction.x < 0)
        {
            eyes.localPosition = new Vector2(-initialEyesPosition.x, eyes.localPosition.y);
            offSet.localPosition = new Vector2(-initialOffsetPosition.x, offSet.localPosition.y);
            playerAreaCollider.transform.localScale = new Vector3(-1, 1, 1);
            
        }
        else
        {
            eyes.localPosition = new Vector2(initialEyesPosition.x, eyes.localPosition.y);
            offSet.localPosition = new Vector2(initialOffsetPosition.x, offSet.localPosition.y);
            playerAreaCollider.transform.localScale = new Vector3(1, 1, 1);

        }
    }
    
    private bool CanJumpOverWall()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(maxJumpHeight.position, rayDirection, obstacleDetectionDistance);
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
    
    private void Jump()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float yDistance = Mathf.Abs(maxJumpHeight.position.y - floor.position.y);
        float initialVelocity = Mathf.Sqrt(2 * gravity * yDistance * 1.2f);

        // Apply the force
        rb.velocity = new Vector2(rb.velocity.x, initialVelocity);
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

    private void WanderWalking()
    {
        CheckForDirectionJumpingWalking();
        if (IsObstacleAhead())
        {
            direction = new Vector2(-direction.x, direction.y);
        }
        if(pathPoints.Count > 0)
        {
            Transform currentTargetPoint = pathPoints[currentTargetIndex];
            direction = (currentTargetPoint.position - transform.position).normalized; // Get direction
            rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, rb.velocity.y); // Apply velocity

            // Check if the enemy has reached the target point
            if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.5f)
            {
                // Move to the next target point in the list
                currentTargetIndex = (currentTargetIndex + 1) % pathPoints.Count;
            }
        }
        else
        {
            if (!isRandomWanderingJumping)
            {
                StartCoroutine(RandomWanderingJumpingWalking());
            }
        }
    }
    
    private void ChasePlayerWalking()
    {   
        CheckForDirectionJumpingWalking();
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

    private void WanderFlying()
    {
        CheckForDirectionFlying();
        if(pathPoints.Count > 0)
        {
            Transform currentTargetPoint = pathPoints[currentTargetIndex];
            direction = (currentTargetPoint.position - transform.position).normalized; // Get direction
            rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, direction.y * enemyStatus.MovementSpeed * Time.deltaTime); // Apply velocity

            // Check if the enemy has reached the target point
            if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.5f)
            {
                // Move to the next target point in the list
                currentTargetIndex = (currentTargetIndex + 1) % pathPoints.Count;
            }
        }
        else
        {
            if (!isRandomWanderingFlying)
            {
                StartCoroutine(RandomWanderingFlying());
            }
        }
    }

    private void ChasePlayerFlying()
    {
        CheckForDirectionFlying();
        
        Vector2 difference = playerPosition.position - transform.position;
        Vector2 diffNormalized = (difference - (Vector2)offSet.localPosition).normalized;

        direction = new Vector2(Mathf.Round(diffNormalized.x), Mathf.Round(diffNormalized.y));

        float xMovement = direction.x * enemyStatus.MovementSpeed * Time.deltaTime * chaseSpeedMultiplier;
        float yMovement = direction.y * enemyStatus.MovementSpeed * Time.deltaTime * chaseSpeedMultiplier;
            
        rb.velocity = new Vector2(xMovement, yMovement);
    }
    private IEnumerator RandomWanderingFlying()
    {
        isRandomWanderingFlying = true;

        // Randomize a point within a circle around the enemy's current position
        Vector2 randomPoint = Random.insideUnitCircle  + (Vector2)transform.position;
        // Calculate direction to move towards the random point
        Vector2 newDir = (randomPoint - (Vector2)transform.position).normalized;
        direction = new Vector2(Mathf.Sign(newDir.x), direction.y);

        // Randomize move duration
        float moveDuration = Random.Range(randomMoveTime.x, randomMoveTime.y);

        // Move towards the random point for the duration
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            rb.velocity = new Vector2(newDir.x * enemyStatus.MovementSpeed * Time.deltaTime, newDir.y * enemyStatus.MovementSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Stop movement after the duration
        rb.velocity = Vector2.zero;

        // Add a delay before the next movement
        yield return new WaitForSeconds(Random.Range(randomMoveInvterval.x, randomMoveInvterval.y));

        isRandomWanderingFlying = false; // Allow another random movement
    }
    private void OnDrawGizmos()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        Debug.DrawRay(eyes.position, rayDirection * obstacleDetectionDistance, Color.red);
        if (maxJumpHeight)
        {
            Debug.DrawRay(maxJumpHeight.position, rayDirection * obstacleDetectionDistance * 2f, Color.green);
        }
    }

    public bool CanAttack()
    {
        if (Vector2.Distance(playerPosition.position, transform.position) < enemyStatus.attackRange)
        {
            FreezeMovement();
            return true;
        }
        return false;
    }

    public void FreezeMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
    }

    public void RestoreMovement()
    {
        canMove = true;
    }

    public bool HasLineOfSight()
    {
        Vector2 rayDirection = (playerPosition.position - eyes.position).normalized;
        float distance = Vector2.Distance(eyes.position, playerPosition.position);

        RaycastHit2D hit = Physics2D.Raycast(eyes.position, rayDirection, distance);
        Debug.DrawLine(playerPosition.position, eyes.position, Color.red);

        if (hit.collider.CompareTag("Player"))
        {   
            return true;
        }
        return false;
    }
}
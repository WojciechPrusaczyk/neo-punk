using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(EntityStatus))]
[RequireComponent(typeof(CustomTags))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Wandering, Chasing }
    public enum EnemyType { FlyingEnemy, WalkingEnemy, JumpingEnemy }

    [Header("Enemy Type")]
    public EnemyType enemyType;

    [Header("Enemy Scripts")]
    [SerializeField] public EntityStatus enemyStatus;
    [SerializeField] private FloorDetector floorDetector;

    [Header("Enemy Transforms")]
    public Transform pathParent;
    public List<Transform> pathPoints;
    public Transform eyes;
    public Transform offSet;
    public Transform maxJumpHeight;
    public Transform floor;

    [Header("Enemy Colliders")]
    public BoxCollider2D playerAreaCollider;
    public BoxCollider2D hitBox;

    [Header("Enemy Variables")]
    public float obstacleDetectionDistance = 1f;
    public float chaseSpeedMultiplier = 1.2f;
    public Vector2 idleAreaOffset;
    public Vector2 idleAreaSize;
    public Vector2 alertedAreaOffset;
    public Vector2 alertedAreaSize;
    public Vector2 randomMoveTime;
    public Vector2 randomMoveInterval;

    [Header("State Management")]
    public EnemyState state;
    public bool canMove = true;
    public bool canAttack = false;

    private Transform playerPosition;
    private Rigidbody2D rb;
    private Vector2 direction;
    private int currentTargetIndex = 0;
    private bool isRandomWandering = false;

    private void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerPosition == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        InitializePath();

        playerAreaCollider.size = idleAreaSize;
        playerAreaCollider.offset = idleAreaOffset;
    }

    private void Update()
    {
        UpdateAttackState();

        if (!canMove || playerPosition == null) return;
        HandleEnemyBehavior();
    }

    private void UpdateAttackState()
    {
        float disableAttackRange = enemyStatus.attackRange + 1.0f;
        float distanceToPlayer = Vector2.Distance(playerPosition.position, transform.position);
        if (enemyType == EnemyType.FlyingEnemy)
        {
            distanceToPlayer = Vector2.Distance(playerPosition.position, offSet.position);
        }

        canAttack = distanceToPlayer < enemyStatus.attackRange;
        if(canAttack) FreezeMovement();
        
        if (distanceToPlayer > disableAttackRange)
        {
            canAttack = false;
        }
    }

    private void HandleEnemyBehavior()
    {
        switch (enemyType)
        {
            case EnemyType.FlyingEnemy:
                if (state == EnemyState.Wandering) WanderFlying();
                else if (state == EnemyState.Chasing) ChasePlayerFlying();
                break;

            case EnemyType.JumpingEnemy:
                if (state == EnemyState.Wandering) WanderJumping();
                else if (state == EnemyState.Chasing) ChasePlayerJumping();
                break;

            case EnemyType.WalkingEnemy:
                if (state == EnemyState.Wandering) WanderWalking();
                else if (state == EnemyState.Chasing) ChasePlayerWalking();
                break;
        }
    }

    private void InitializePath()
    {
        pathPoints.Clear();
        if (pathParent != null)
        {
            foreach (Transform child in pathParent)
            {
                pathPoints.Add(child);
            }
        }
    }

    private void WanderJumping()
    {
        CheckForDirection();
        if (IsObstacleAhead() && CanJumpOverWall() && floorDetector.isPlayerNearGround)
        {
            Jump();
        }

        if (pathPoints.Count > 0)
        {
            FollowPath();
        }
        else if (!isRandomWandering)
        {
            StartCoroutine(RandomWandering());
        }
    }

    private void ChasePlayerJumping()
    {
        CheckForDirection();
        if (IsObstacleAhead() && CanJumpOverWall() && floorDetector.isPlayerNearGround)
        {
            Jump();
        }

        MoveTowardsPlayer();
    }

    private void WanderWalking()
    {
        CheckForDirection();
        if (IsObstacleAhead())
        {
            direction = new Vector2(-direction.x, direction.y);
        }

        if (pathPoints.Count > 0)
        {
            FollowPath();
        }
        else if (!isRandomWandering)
        {
            StartCoroutine(RandomWandering());
        }
    }

    private void ChasePlayerWalking()
    {
        CheckForDirection();
        MoveTowardsPlayer();
    }

    private void WanderFlying()
    {
        CheckForDirection();
        if (pathPoints.Count > 0)
        {
            FollowPath();
        }
        else if (!isRandomWandering)
        {
            StartCoroutine(RandomFlyingWandering());
        }
    }

    private void ChasePlayerFlying()
    {
        CheckForDirection();
        FlyTowardsPlayer();
    }

    private void FollowPath()
    {
        Transform currentTargetPoint = pathPoints[currentTargetIndex];
        direction = (currentTargetPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, rb.velocity.y);

        if (Vector2.Distance(transform.position, currentTargetPoint.position) < 0.5f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % pathPoints.Count;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 difference = playerPosition.position - transform.position;
        Vector2 diffNormalized = difference.normalized;

        direction = new Vector2(Mathf.Round(diffNormalized.x), Mathf.Round(diffNormalized.y));
        float xMovement = direction.x * enemyStatus.MovementSpeed * Time.deltaTime * chaseSpeedMultiplier;

        rb.velocity = new Vector2(xMovement, rb.velocity.y);
        
    }
    private void FlyTowardsPlayer()
    {
        Vector2 difference = playerPosition.position - offSet.position;
        Vector2 diffNormalized = difference.normalized;

        direction = new Vector2(Mathf.Round(diffNormalized.x), Mathf.Round(diffNormalized.y));
        float xMovement = direction.x * enemyStatus.MovementSpeed * Time.deltaTime * chaseSpeedMultiplier;

        float yMovement = direction.y * enemyStatus.MovementSpeed * Time.deltaTime * chaseSpeedMultiplier;
        rb.velocity = new Vector2(xMovement, yMovement);
        
    }

    private IEnumerator RandomWandering()
    {
        isRandomWandering = true;

        float randomDirection = Random.value < 0.5f ? -1f : 1f;
        direction = new Vector2(randomDirection, 0).normalized;

        float moveDuration = Random.Range(randomMoveTime.x, randomMoveTime.y);
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, rb.velocity.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(Random.Range(randomMoveInterval.x, randomMoveInterval.y));

        isRandomWandering = false;
    }
    
    private IEnumerator RandomFlyingWandering()
    {
        isRandomWandering = true;

        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        direction = randomDirection.normalized;
        direction = new Vector2(direction.x, direction.y / 3f);

        float moveDuration = Random.Range(randomMoveTime.x, randomMoveTime.y);
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            rb.velocity = new Vector2(direction.x * enemyStatus.MovementSpeed * Time.deltaTime, direction.y * enemyStatus.MovementSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(Random.Range(randomMoveInterval.x, randomMoveInterval.y));

        isRandomWandering = false;
    }

    private void CheckForDirection()
    {
        if (direction.x < 0)
        {
            eyes.localPosition = new Vector2(-Mathf.Abs(eyes.localPosition.x), eyes.localPosition.y);
            playerAreaCollider.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            eyes.localPosition = new Vector2(Mathf.Abs(eyes.localPosition.x), eyes.localPosition.y);
            playerAreaCollider.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private bool CanJumpOverWall()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(maxJumpHeight.position, rayDirection, obstacleDetectionDistance);

        return hit.collider == null || !hit.collider.CompareTag("impassableFloor");
    }

    private void Jump()
    {
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float yDistance = Mathf.Abs(maxJumpHeight.position.y - floor.position.y);
        float initialVelocity = Mathf.Sqrt(2 * gravity * yDistance * 1.2f);

        rb.velocity = new Vector2(rb.velocity.x, initialVelocity);
    }

    private bool IsObstacleAhead()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(eyes.position, rayDirection, obstacleDetectionDistance);
        
        return hit.collider != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("ImpassableWall");
    }

    private void OnDrawGizmos()
    {
        Vector2 rayDirection = new Vector2(Mathf.Sign(direction.x), 0);
        if (eyes != null)
        {
            Debug.DrawRay(eyes.position, rayDirection * obstacleDetectionDistance, Color.red);
        }

        if (maxJumpHeight)
        {
            Debug.DrawRay(maxJumpHeight.position, rayDirection * obstacleDetectionDistance * 2f, Color.green);
        }
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
        int layerMask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("ImpassableWall"));

        RaycastHit2D hit = Physics2D.Raycast(eyes.position, rayDirection, distance, layerMask);
        Debug.DrawLine(playerPosition.position, eyes.position, Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    public void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
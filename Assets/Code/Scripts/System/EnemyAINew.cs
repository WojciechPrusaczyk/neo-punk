using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// EnemyAI master component that orchestrates a simple finite‑state machine (FSM)
/// with two concrete states: Wandering and Chasing. 
/// Extend by adding new State classes and instantiating them in Awake().
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(EntityStatus))]
[RequireComponent(typeof(CustomTags))]
public class EnemyAINew : MonoBehaviour
{
    public enum EnemyTypeNew { FlyingEnemy, WalkingEnemy, JumpingEnemy }

    [Header("Enemy Type")]
    public EnemyTypeNew enemyType;

    [Header("Enemy Scripts & Transforms")]
    [SerializeField] public EntityStatus enemyStatus;
    [SerializeField] private FloorDetector floorDetector;

    [SerializeField] private Transform eyes;
    [SerializeField] private Transform maxJumpHeight;
    [SerializeField] private Transform floor;

    [Header("Enemy Colliders")]
    [SerializeField] private BoxCollider2D playerAreaCollider;
    [SerializeField] private BoxCollider2D hitBox;

    [Header("Enemy Variables")]
    [SerializeField] private float obstacleDetectionDistance = 1f;
    [SerializeField] private float chaseSpeedMultiplier = 1.2f;
    [SerializeField] private Vector2 randomMoveTime;
    [SerializeField] private Vector2 randomMoveInterval;

    [Header("Path Following")]
    public Transform pathParent;
    public readonly List<Transform> pathPoints = new List<Transform>();

    [HideInInspector] public Rigidbody2D Rb { get; private set; }
    [HideInInspector] public Transform Player { get; private set; }

    // FSM
    private EnemyState _currentState;
    private WanderingState _wanderingState;
    private ChasingState _chasingState;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (Player == null)
        {
            Debug.LogError("Player not found – ensure the player GameObject has the “Player” tag.");
            enabled = false;
            return;
        }

        // instantiate states
        _wanderingState = new WanderingState(this);
        _chasingState = new ChasingState(this);

        // start in wander
        ChangeState(_wanderingState);

        if (pathParent != null)
        {
            foreach (Transform child in pathParent)
            {
                pathPoints.Add(child);
            }
        }
    }

    private void Update()
    {
        _currentState?.LogicUpdate();
    }

    #region State Machine helpers
    public void ChangeState(EnemyState newState)
    {
        if (_currentState == newState) return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public bool HasLineOfSight()
    {
        Vector2 direction = (Player.position - eyes.position).normalized;
        float distance = Vector2.Distance(eyes.position, Player.position);
        int mask = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("ImpassableWall"));

        RaycastHit2D hit = Physics2D.Raycast(eyes.position, direction, distance, mask);
        return hit.collider != null && hit.collider.CompareTag("Player");
    }
    #endregion

    #region Movement helpers (call from states)
    public void MoveTowards(Vector2 direction, bool multiplyChaseSpeed)
    {
        float xSpeed = direction.x * enemyStatus.MovementSpeed * Time.deltaTime;
        float ySpeed = Rb.velocity.y;

        if (multiplyChaseSpeed) xSpeed *= chaseSpeedMultiplier;

        // flying adds y movement
        if (enemyType == EnemyTypeNew.FlyingEnemy)
        {
            ySpeed = direction.y * enemyStatus.MovementSpeed * Time.deltaTime;
            if (multiplyChaseSpeed) ySpeed *= chaseSpeedMultiplier;
        }

        Rb.velocity = new Vector2(xSpeed, ySpeed);
    }

    public void JumpIfPossible()
    {
        if (enemyType != EnemyTypeNew.JumpingEnemy) return;

        if (IsObstacleAhead() && CanJumpOverWall() && floorDetector.isPlayerNearGround)
        {
            float gravity = Mathf.Abs(Physics2D.gravity.y * Rb.gravityScale);
            float yDistance = Mathf.Abs(maxJumpHeight.position.y - floor.position.y);
            float initialVelocity = Mathf.Sqrt(2 * gravity * yDistance * 1.15f);

            Rb.velocity = new Vector2(Rb.velocity.x, initialVelocity);
        }
    }

    private bool CanJumpOverWall()
    {
        Vector2 rayDir = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(maxJumpHeight.position, rayDir, obstacleDetectionDistance);

        return hit.collider == null || !hit.collider.CompareTag("impassableFloor");
    }

    private bool IsObstacleAhead()
    {
        Vector2 rayDir = new Vector2(Mathf.Sign(transform.localScale.x), 0);
        RaycastHit2D hit = Physics2D.Raycast(eyes.position, rayDir, obstacleDetectionDistance);
        return hit && hit.transform.gameObject.layer == LayerMask.NameToLayer("ImpassableWall");
    }

    public IEnumerator RandomMoveCoroutine()
    {
        float dir = Random.value < .5f ? -1f : 1f;
        Vector2 moveDir = new Vector2(dir, 0).normalized;

        float duration = Random.Range(randomMoveTime.x, randomMoveTime.y);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            MoveTowards(moveDir, false);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Rb.velocity = new Vector2(0, Rb.velocity.y);
        yield return new WaitForSeconds(Random.Range(randomMoveInterval.x, randomMoveInterval.y));
    }

    public void StopMovement() => Rb.velocity = Vector2.zero;

    #endregion
}

/// <summary>
/// Base class for all enemy states.
/// </summary>
public abstract class EnemyState
{
    protected readonly EnemyAINew enemy;
    protected EnemyState(EnemyAINew enemy) => this.enemy = enemy;

    /// <summary>Called once when the state is entered.</summary>
    public virtual void Enter() { }

    /// <summary> Called every frame by EnemyAI.Update()</summary>
    public abstract void LogicUpdate();

    /// <summary>Called once when the state is exited.</summary>
    public virtual void Exit() { }
}

/// <summary>
/// Roams around until the player is spotted.
/// </summary>
public class WanderingState : EnemyState
{
    private IEnumerator wanderRoutine;

    public WanderingState(EnemyAINew enemy) : base(enemy) { }

    public override void Enter()
    {
        wanderRoutine = enemy.RandomMoveCoroutine();
        enemy.StartCoroutine(wanderRoutine);
    }

    public override void LogicUpdate()
    {
        // path following takes priority
        if (enemy.pathPoints.Count > 0)
        {
            Transform target = enemy.pathPoints[0]; // simple example
            Vector2 dir = (target.position - enemy.transform.position).normalized;
            enemy.MoveTowards(dir, false);
        }

        if (enemy.HasLineOfSight())
        {
            enemy.ChangeState(new ChasingState(enemy));
        }
    }

    public override void Exit()
    {
        if (wanderRoutine != null) enemy.StopCoroutine(wanderRoutine);
        enemy.StopMovement();
    }
}

/// <summary>
/// Follows and attacks the player while in sight.
/// </summary>
public class ChasingState : EnemyState
{
    public ChasingState(EnemyAINew enemy) : base(enemy) { }

    public override void LogicUpdate()
    {
        Vector2 dir = (enemy.Player.position - (enemy.enemyType == EnemyAINew.EnemyTypeNew.FlyingEnemy ?
                                                enemy.transform.position : enemy.transform.position)).normalized;

        enemy.MoveTowards(dir, true);
        enemy.JumpIfPossible();

        // transition back to wander if player lost
        if (!enemy.HasLineOfSight())
        {
            enemy.ChangeState(new WanderingState(enemy));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

public class AbominationMovement : MonoBehaviour
{
    public ArenaCollider arenaCollider;
    public bool active;
    
    public float movementSpeed;
    
    public GameObject player;

    public Rigidbody2D rb;
    
    public Animator animator;
    public IKManager2D ikManager;
    public Transform bodyTransform;
    
    public bool isAttacking;
    public List<string> triggers;
    public bool facingLeft;
    
    public float attackRange    = 2.5f;   // metres
    public float globalCooldown = 1.2f;   // seconds
    public float nextAllowedTime = 0f;
    

    [Header("Attack colliders")]
    public Collider2D headCollider;
    public Collider2D clawCollider;
    public Collider2D tailCollider;
    
    [Header("Bite retreat")]
    [SerializeField] private float biteRetreatDistance = 1.0f;

    [SerializeField] private float biteRetreatTime = 0.20f;
        
    [Header("Test smooth turn")]
    [SerializeField] private float turnDuration = 0.3f;
    private Coroutine turnRoutine;
    
    [Header("Instant flip")]
    [SerializeField] private float deadZone = 0.3f;
   
    [Header(" ")]
    private GameObject mainUi;
    private EntityStatus abominationStatus;
    
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator.enabled = false;
        mainUi = GameObject.Find("MainUserInterface");
        abominationStatus = gameObject.transform.parent.GetComponent<EntityStatus>();

        if (abominationStatus != null)
        {
            abominationStatus.OnEntityDeath += OnDeath;
        }
    }

    private void Update()
    {
        if (Time.time < nextAllowedTime)
            return;

        if (Vector2.Distance(player.transform.position, transform.position) > attackRange)
            return;

        float dist = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log(dist);
        int index;

        if (dist < 1.3f)
        {
            StingAttack();
        }
        else if (dist < 3f)
        {
            BiteAttack();
        }
        else
        {
            ClawAttack();
        }

        float r = Random.Range(-.5f, .5f);
        nextAllowedTime = Time.time + globalCooldown + r;
    }

    private void OnEnable()
    {
        if (arenaCollider == null)
            return;

        arenaCollider.onArenaEnter += ActivateBoss;
        arenaCollider.onArenaExit += DeactivateBoss;
    }

    private void OnDisable()
    {
        if (arenaCollider == null)
            return;

        arenaCollider.onArenaEnter -= ActivateBoss;
        arenaCollider.onArenaExit -= DeactivateBoss;
    }

    
    private void FixedUpdate()
    {
        if (active)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > attackRange ||
                Vector2.Distance(player.transform.position, transform.position) < 5)
            {
                if (!isAttacking)
                {
                    MoveHorizontallyToPlayer();
                    CheckForFlip();
                }
            }
        }
    }

    public void StingAttack()
    {
        StartCoroutine(PlayAnimation(triggers[0]));
    }

    private IEnumerator BiteSequence()
    {
        isAttacking = true;
        
        Vector2 startPos = rb.position;
        Vector2 awayDir = new Vector2(rb.position.x - player.transform.position.x, 0f).normalized;
        Vector2 endPos   = startPos + awayDir * biteRetreatDistance;

        float t = 0f;
        while (t < biteRetreatTime)
        {
            t += Time.deltaTime;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, t / biteRetreatTime));
            yield return null;
        }
        
        StartCoroutine(PlayAnimation(triggers[1]));

    }
    
    public void BiteAttack()
    {
        CheckForFlip();
        StartCoroutine(BiteSequence());
    }
    
    public void ClawAttack()
    {
        StartCoroutine(PlayAnimation(triggers[2]));
    }

    

    public void ActivateBoss(bool b)
    {
        active = b;
        
        var abominationStatus = gameObject.transform.parent.GetComponent<EntityStatus>();
        if (mainUi != null)
        {
            mainUi.GetComponent<MainUserInterfaceController>().ShowBossBar(abominationStatus);
        }

        MusicManager.instance.PlaySong(MusicManager.instance.Boss1Track, Enums.SoundType.Music);
    }
    
    public void DeactivateBoss(bool b)
    {
        active = b;
    }
    
    void MoveHorizontallyToPlayer()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);

        Vector2 pos = rb.position;
        pos.x = Mathf.MoveTowards(pos.x,
            player.transform.position.x,
            movementSpeed * Time.fixedDeltaTime);
        rb.MovePosition(pos);
    }

    void StartAnimation()
    {
        isAttacking = true;
        ikManager.enabled = false;
        animator.enabled = true;
    }
    
    void EndAnimation()
    {
        isAttacking = false;
        ikManager.enabled = true;
        animator.enabled = false;
    }

    IEnumerator PlayAnimation(string trigger)
    {
        StartAnimation();

        animator.SetTrigger(trigger);

        yield return null;

        yield return new WaitUntil(() =>
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            return info.IsTag("Attack") && !animator.IsInTransition(0);
        });

        var state = animator.GetCurrentAnimatorStateInfo(0);
        float clipLength = state.length / animator.speed;   // adjust for speed

        yield return new WaitForSeconds(clipLength);

        EndAnimation();
    }


    public void CheckForFlip()
    {
        if (!player) return;

        float dx = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(dx) <= deadZone)
            return;
        
        bool wantLeft = dx > 0f;
        if (wantLeft != facingLeft)
            Flip(wantLeft);
    }
    
    void Flip(bool faceLeft)
    {
        facingLeft = faceLeft;
        ikManager.enabled = false;

        Vector3 s = bodyTransform.localScale;
        s.x = Mathf.Abs(s.x) * (faceLeft ? -1 : 1);
        bodyTransform.localScale = s;
        ikManager.enabled = true;

    }
    
    IEnumerator SmoothFlip(bool faceLeft)
    {
        facingLeft = faceLeft;
        ikManager.enabled = false;

        Vector3 start = bodyTransform.localScale;
        Vector3 end   = new Vector3(Mathf.Abs(start.x) * (faceLeft ? -1 : 1),
            start.y, start.z);

        float t = 0f;
        while (t < turnDuration)
        {
            t += Time.deltaTime;
            bodyTransform.localScale = Vector3.Lerp(start, end, t / turnDuration);
            yield return null;
        }

        bodyTransform.localScale = end;
        ikManager.enabled = true;
        turnRoutine = null;
    }

    private void OnDeath()
    {
        if (mainUi != null)
        {
            mainUi.GetComponent<MainUserInterfaceController>().HideBossBar();
        }
    }
}

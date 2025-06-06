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

    public Collider2D headCollider;
    public Collider2D clawCollider;
    public Collider2D tailCollider;

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
        int index;
        if (dist < 1f)
        {
            index = 0;
        }
        else if (dist < 4f)
        {
            index = 1;
        }
        else
        {
            index = 2;
        }
        

        StartCoroutine(PlayAnimation(triggers[index]));

        // 4. reset global cooldown
        nextAllowedTime = Time.time + globalCooldown;
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
                MoveHorizontallyToPlayer();
                if (!isAttacking)
                {
                    CheckForFlip();
                }            
            }
        }
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
        if (isAttacking)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

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
        
        
        bool wantLeft = player.transform.position.x > transform.position.x;
        if (wantLeft != facingLeft)
            Flip(wantLeft);
        
    }
    
    void Flip(bool faceLeft)
    {
        facingLeft = faceLeft;
        ikManager.enabled = false;

        // 1️⃣  Odbicie sprite’ów i kości
        Vector3 s = bodyTransform.localScale;
        s.x = Mathf.Abs(s.x) * (faceLeft ? -1 : 1);
        bodyTransform.localScale = s;
        ikManager.enabled = true;

    }

    private void OnDeath()
    {
        if (mainUi != null)
        {
            mainUi.GetComponent<MainUserInterfaceController>().HideBossBar();
        }
        MusicManager.instance.PlaySong(MusicManager.instance.Level1Track, Enums.SoundType.Music);
        EventFlagsSystem _EventsFlagsSystem = EventFlagsSystem.instance;
        
        if (!_EventsFlagsSystem.IsEventDone("boss1Defeat"))
        {
            _EventsFlagsSystem.FinishEvent("boss1Defeat");
        }

    }
}

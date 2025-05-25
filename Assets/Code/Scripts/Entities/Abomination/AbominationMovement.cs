using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using UnityEngine.U2D.IK;
using Random = UnityEngine.Random;

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
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator.enabled = false;
        /*
         * For debugging only
         */
    }

    private void Update()
    {
        if (Time.time < nextAllowedTime)
            return;

        // 2. is player close enough?
        if (Vector2.Distance(player.transform.position, transform.position) > attackRange)
            return;

        // 3. pick a random attack and fire it
        float dist = Vector2.Distance(transform.position, player.transform.position);

        int index;
        if (dist < 2f)
        {
            // choose either 1 or 2 (inclusive)
            index = Random.Range(1, 3);   // Random.Range(int,int) upper bound is exclusive
        }
        else if (dist < 10f)
        {
            index = 0;
        }
        else
        {
            index = 1;
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
                Vector2.Distance(player.transform.position, transform.position) < 2)
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
        var mainUi = GameObject.Find("MainUserInterface");
        if (mainUi != null)
        {
            mainUi.GetComponent<MainUserInterfaceController>().ShowBossBar(abominationStatus);
        }
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

}

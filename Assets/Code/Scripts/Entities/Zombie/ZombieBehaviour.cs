using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieBehaviour : MonoBehaviour
{
    [SerializeField] public Transform[] Positions;
    private float EntitySpeed;

    private int NextPositionIndex;
    private Transform NextPosition;
    private EntityStatus entityStatus;
    private Vector3 playerVector3;
    private bool isChasingPlayer;
    private Vector2 previousPlayerDetectorRange;
    private BoxCollider2D playerDetector;
    private Vector3 previousPosition;
    public float currentSpeed;
    public bool isPlayerInAttackRange;
    public float distanceToPlayer;
    private Animator animator;
    public LayerMask warstwaPrzeszkod;
    private bool didRaycastFoundPlayer = false;
    public bool isAttacking = false;
    public bool canAttack = false; // Zmienna, która będzie przechowywać informację o dotknięciu gracza
    public GameObject collidingPlayer;

    void Start()
    {
        if (Positions.Length > 0) NextPosition = Positions[0];
        entityStatus = gameObject.GetComponent<EntityStatus>();
        EntitySpeed = entityStatus.GetMovementSpeed();

        playerDetector = gameObject.transform.Find("PlayerDetector").gameObject.GetComponent<BoxCollider2D>();
        previousPlayerDetectorRange = playerDetector.size;
        animator = gameObject.GetComponent<Animator>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canAttack = false;
            EntityStatus playerStatus = collision.gameObject.GetComponent<EntityStatus>();
            playerStatus.DealDamage(entityStatus.GetAttackDamageCount(), gameObject);
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canAttack )
        {
            canAttack = false;
            EntityStatus playerStatus = collision.gameObject.GetComponent<EntityStatus>();
            playerStatus.DealDamage(entityStatus.GetAttackDamageCount(), gameObject);
        }
    }

    void  Update()
    {
        if (!isAttacking) MoveZombie();
        
        /*
         * Obliczanie prędkości aktualnej, oraz kierunku ruchu zombie
         */
        var position = transform.position;
        Vector3 currentPosition = position;
        Vector3 displacement = position - previousPosition;
        Vector3 speedVector = displacement / Time.deltaTime;
        currentSpeed = speedVector.x;
        previousPosition = currentPosition;
        
        animator.SetFloat("CurrentSpeed", currentSpeed );
        animator.SetBool("IsPlayerInAttackRange", isPlayerInAttackRange );
        
        if (currentSpeed > 0) entityStatus.isFacedRight = true; 
        else if (currentSpeed < 0) entityStatus.isFacedRight = false; 
        
        //if (currentSpeed != 0) animator.Play("walkingAnimation");
        
        /*
         * Obracanie 
         */
        if (!isAttacking)
        {
            if (!entityStatus.isFacedRight)
            {
                Vector3 newRotation = transform.eulerAngles;
                newRotation.y = 180;
                transform.eulerAngles = newRotation;
            }
            else
            {
                Vector3 newRotation = transform.eulerAngles;
                newRotation.y = 0;
                transform.eulerAngles = newRotation;
            }
        }

        /*
         * Atak podstawowy na gracza
         */
        if (isChasingPlayer && entityStatus.detectedTarget )
        {
            distanceToPlayer = Vector3.Distance(transform.position, entityStatus.detectedTarget.transform.position);
            isPlayerInAttackRange = ( distanceToPlayer <= entityStatus.attackRange );
            
            // Atak na gracza
            if (isPlayerInAttackRange && !isAttacking)
            {
                StartCoroutine(PerformAttack());
            }
        }
        else isPlayerInAttackRange = false; 
        
        /*
         * Raycast
         */
        if (entityStatus.detectedTarget)
        {
            Vector2 raycastOrigin = transform.position;

            // Pozycja gracza
            Vector2 playerPosition = entityStatus.detectedTarget.transform.position;

            // Kierunek raycasta od obiektu do gracza
            Vector2 raycastDirection =  playerPosition - raycastOrigin;

            // Długość raycasta
            float raycastDistance = raycastDirection.magnitude;

            // Normalizacja kierunku raycasta
            raycastDirection.Normalize();

            // Wykonaj raycast w kierunku gracza
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, raycastDistance, warstwaPrzeszkod);

            // Jeśli raycast trafiał w przeszkodę
            if (hit.collider != null)
            {
                // Sprawdź, czy trafiony obiekt ma tag "Player"
                didRaycastFoundPlayer = hit.collider.CompareTag("Player");
            }
            else
            {
                didRaycastFoundPlayer = false;
            }
        }
        
    }

    void Attack()
    {
        
        Rigidbody2D zombieRigidbody = gameObject.GetComponent<Rigidbody2D>();
        
        canAttack = true;
        
        float attackDirection = (entityStatus.detectedTarget.transform.position.x < gameObject.transform.position.x)
            ? -1
            : 1;
        
        Vector3 movement = new Vector3(attackDirection * entityStatus.attackRange * 180, 900, 0);
        zombieRigidbody.AddForce(movement);
    }
    
    private IEnumerator PerformAttack()
    {
        while (isPlayerInAttackRange)
        {
            animator.Play("attackAnimation");
            isAttacking = true;
            Attack(); // Wywołaj atak
            
            yield return new WaitForSeconds(2f); // Poczekaj na zakończenie ataku
            isAttacking = false;
        }
    }

    void MoveZombie()
    {
        // gdy wykryto gracza w polu playerDetector, lub otrzymano jakiekolwiek obrażenia
        if ( (isChasingPlayer && playerDetector && didRaycastFoundPlayer) || entityStatus.GetMaxHp() > entityStatus.GetHp() )
        {
            // zwiększ hitbox playerDetector
            playerDetector.size = new Vector2(previousPlayerDetectorRange.x * 1.4f, previousPlayerDetectorRange.y * 1.2f) ;
        }
        
        // Niezakłócony ruch dopóki nie wykryto gracza
        if ( ( !didRaycastFoundPlayer && entityStatus.detectedTarget) || ( !entityStatus.detectedTarget && !entityStatus.detectedTarget ) )
        {
            isChasingPlayer = false;
            distanceToPlayer = 0;
            if (Math.Abs(transform.position.x - NextPosition.position.x) < 0.1 )
            {
                NextPositionIndex++;
                if (NextPositionIndex >= Positions.Length)
                {
                    NextPositionIndex = 0;
                }
                NextPosition = Positions[NextPositionIndex];
                entityStatus.isFacedRight = !entityStatus.isFacedRight;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, NextPosition.position, EntitySpeed * Time.deltaTime);
            }
        }
        else
        {
            // Ruch po wykryciu gracza
            isChasingPlayer = true;
            playerVector3 = entityStatus.detectedTarget.transform.position;
            var position = transform.position;
            position = Vector3.MoveTowards(position, new Vector3(playerVector3.x, position.y, position.z), EntitySpeed * Time.deltaTime);
            transform.position = position;
        }
    }
}

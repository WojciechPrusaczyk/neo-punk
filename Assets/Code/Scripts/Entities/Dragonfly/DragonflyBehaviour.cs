using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonflyBehavior : MonoBehaviour
{
    [SerializeField] public Transform[] Positions;
    private float EntitySpeed;
    private int NextPositionIndex;
    private Transform NextPosition;
    private EntityStatus entityStatus;
    private Vector3 playerVector3;
    private bool isChasingPlayer;
    private Vector2 previousPlayerDetectorRange;
    private CapsuleCollider2D playerDetector;
    private Vector3 previousPosition;
    private bool didRaycastFoundPlayer = false;
    private bool isAttacking = false;
    private bool canAttack = false; // Zmienna, która będzie przechowywać informację o dotknięciu gracza
    private bool isPlayerInAttackRange;
    private GameObject collidingPlayer;

    public float currentSpeed;
    public float distanceToPlayer;
    public LayerMask warstwaPrzeszkod;
    public Transform shootingPoint;
    public GameObject projectilePrefab;
    public float bulletSpeed = 20.0f;
    
    void Start()
    {
        if (Positions.Length > 0) NextPosition = Positions[0];
        entityStatus = gameObject.GetComponent<EntityStatus>();
        EntitySpeed = entityStatus.GetMovementSpeed();

        playerDetector = gameObject.transform.Find("PlayerDetector").gameObject.GetComponent<CapsuleCollider2D>();
        previousPlayerDetectorRange = playerDetector.size;
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
        if (!isAttacking) Move();
        
        /*
         * Obliczanie prędkości aktualnej, oraz kierunku ruchu
         */
        var position = transform.position;
        Vector3 currentPosition = position;
        Vector3 displacement = position - previousPosition;
        Vector3 speedVector = displacement / Time.deltaTime;
        currentSpeed = speedVector.x;
        previousPosition = currentPosition;

        if (!isPlayerInAttackRange)
        {
            if (currentSpeed > 0) entityStatus.isFacedRight = true; 
            else if (currentSpeed < 0) entityStatus.isFacedRight = false;
        }
        else if (isPlayerInAttackRange && null != entityStatus.detectedTarget)
        {
            entityStatus.isFacedRight = !(entityStatus.detectedTarget.transform.position.x < gameObject.transform.position.x);
        }

        /*
         * Obracanie 
         */
        if (!entityStatus.isFacedRight)
        {
            Vector3 newRotation = transform.eulerAngles;
            newRotation.y = 0;
            transform.eulerAngles = newRotation;
        }
        else
        {
            Vector3 newRotation = transform.eulerAngles;
            newRotation.y = 180;
            transform.eulerAngles = newRotation;
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

    void PerformShoot()
    {
        // Oblicz wektor kierunku od punktu strzału do pozycji gracza
        Vector3 shootDirection = (entityStatus.detectedTarget.transform.position - shootingPoint.position).normalized;

        // Utwórz nowy pocisk z prefabrykatu, oraz wprowadź do niego dane o strzelcu
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        if (bulletBehaviour != null)
        {
            bulletBehaviour.SetShooter(gameObject); // Przekazanie referencji na obiekt, który wystrzelił kulę
        }

        // Ustaw prędkość pocisku w kierunku gracza
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = shootDirection * bulletSpeed;

        // Obróć pocisk, aby wskazywał w kierunku ruchu
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, shootDirection) * Quaternion.Euler(0, 0, 90);
        projectile.transform.rotation = rotation;

        // Zniszcz pocisk po określonym czasie (jeśli nie trafi w cel)
        Destroy(projectile, 1f);
    }
    
    private IEnumerator PerformAttack()
    {
        while (isPlayerInAttackRange && entityStatus.detectedTarget)
        {
            isAttacking = true;
            canAttack = true;

            PerformShoot(); // Wywołaj atak
            
            yield return new WaitForSeconds(1.5f); // Tutaj można zdefiniować attack speed
            isAttacking = false;
        }
        
    }

    void Move()
    {
        // gdy wykryto gracza w polu playerDetector, lub otrzymano jakiekolwiek obrażenia
        if ( (isChasingPlayer && playerDetector && didRaycastFoundPlayer) || entityStatus.GetMaxHp() > entityStatus.GetHp() )
        {
            // zwiększ hitbox playerDetector
            playerDetector.size = new Vector2(previousPlayerDetectorRange.x * 2.2f, previousPlayerDetectorRange.y * 1.8f) ;
        }
        
        // Niezakłócony ruch dopóki nie wykryto gracza
        if ( ( !didRaycastFoundPlayer && entityStatus.detectedTarget) || ( !entityStatus.detectedTarget && !entityStatus.detectedTarget ) )
        {
            isChasingPlayer = false;
            distanceToPlayer = 0;
            
            // poruszanie się po wyznaconej trasie
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
            position = Vector3.MoveTowards(position, new Vector3(playerVector3.x, playerVector3.y, position.z), EntitySpeed * Time.deltaTime);
            transform.position = position;
        }
    }
}

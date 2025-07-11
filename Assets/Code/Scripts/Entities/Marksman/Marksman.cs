using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marksman : MonoBehaviour
{
    
    public bool isAttacking = false;

    public GameObject grenadePrefab;
    public GameObject bulletPrefab; 
    public Transform bulletSpawn;
    
    private Animator _animator;
    private EntityStatus _entityStatus;
    private GameObject _entityBody;
    private EnemyAI _enemyAI;
    private GameObject _player;
    
    [SerializeField] private string[] attackTriggers = { "Shoot", "Push", "Grenade"};
    [SerializeField] private float minExtraDelay = 1;
    [SerializeField] private float maxExtraDelay = 2;
    
    
    public void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _entityStatus = GetComponent<EntityStatus>();
        _enemyAI = GetComponent<EnemyAI>();
        _entityBody = gameObject.transform.Find("Graphics").transform.Find("Body").gameObject;
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    
    private void Update()
    {
        Attack();
        
        float entityVelocity = GetComponent<Rigidbody2D>().velocity.x;
        _animator.SetFloat("Velocity", Mathf.Abs(entityVelocity));
        
        if (entityVelocity > 0 && !_entityStatus.isFacedRight && (Time.timeScale != 0))
        {
            _entityStatus.isFacedRight = true;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
        else if (entityVelocity < 0 && _entityStatus.isFacedRight && (Time.timeScale != 0))
        {
            _entityStatus.isFacedRight = false;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
    }
    
    public void Attack()
    {
        float distanceToPlayer = Vector2.Distance(_player.transform.position, transform.position);

        if (distanceToPlayer <= 2f && !isAttacking)
        {
            isAttacking = true;
            LookAtPlayer();
            _animator.SetTrigger("Push");
            StartCoroutine(AttackCooldownRoutine());
            return;
        }

        if (_enemyAI.canAttack && !isAttacking)
        {
            string trig = "";

            if (distanceToPlayer > 7f)
            {
                trig = "Grenade";
            }
            else
            {
                trig = "Shoot";
            }

            isAttacking = true;
            LookAtPlayer();
            _animator.SetTrigger(trig);
            StartCoroutine(AttackCooldownRoutine());
        }
    }
    
    private IEnumerator AttackCooldownRoutine()
    {
        while (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            yield return null;

        yield return new WaitForSeconds(Random.Range(minExtraDelay, maxExtraDelay));
        isAttacking = false;
    }
    
    public void LookAtPlayer()
    {
        if (_player.transform.position.x > transform.position.x && !_entityStatus.isFacedRight)
        {
            _entityStatus.isFacedRight = true;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
        else if (_player.transform.position.x < transform.position.x && _entityStatus.isFacedRight)
        {
            _entityStatus.isFacedRight = false;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
    }
}

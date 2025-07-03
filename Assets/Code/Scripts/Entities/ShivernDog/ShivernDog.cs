using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShivernDog : MonoBehaviour
{
    public bool isEating = false;
    public bool isAttacking = false;
    private Animator _animator;
    private EntityStatus _entityStatus;
    private GameObject _entityBody;
    private EnemyAI _enemyAI;
    private GameObject _player;

    public List<string> attackAnimations;
    [SerializeField] private string[] attackTriggers = { "Attack1", "Attack2" };
    [SerializeField] private float minExtraDelay = 0.3f;
    [SerializeField] private float maxExtraDelay = 1.2f;
    
    public void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _entityStatus = GetComponent<EntityStatus>();
        _enemyAI = GetComponent<EnemyAI>();
        _entityBody = gameObject.transform.Find("Graphics").transform.Find("Body").gameObject;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAttacking && Vector2.Distance(_player.transform.position, transform.position) > 0.1f)
            _enemyAI.RestoreMovement();

        float entityVelocity = GetComponent<Rigidbody2D>().velocity.x;

        Attack();

        _animator.SetBool("IsEating", isEating);
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
    
    public void Attack()
    {
        if (_enemyAI.canAttack && !isAttacking)
        {
            isAttacking = true;
            LookAtPlayer();

            // Fire a random trigger instead of Play()
            string trig = attackTriggers[Random.Range(0, attackTriggers.Length)];
            _animator.SetTrigger(trig);

            StartCoroutine(AttackCooldownRoutine());
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        // Wait until the animator is NOT in any 'Attack' state
        // (avoids hard-coding clip lengths)
        while (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            yield return null;

        // Then wait extra random time
        yield return new WaitForSeconds(Random.Range(minExtraDelay, maxExtraDelay));

        isAttacking = false;
    }
    
    public void DealDamage()
    {
        if (_enemyAI.canAttack)
        {
            _player.GetComponent<EntityStatus>().DealDamage(_entityStatus.AttackDamage, transform.gameObject);
        }
        
    }
}
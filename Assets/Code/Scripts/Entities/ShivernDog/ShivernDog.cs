using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShivernDog : MonoBehaviour
{
    [FormerlySerializedAs("IsEating")] public bool isEating = false;
    public bool isAttacking = false;
    private Animator _animator;
	private EntityStatus _entityStatus;
    private GameObject _entityBody;
    private EnemyAI _enemyAI;
    private GameObject _player;

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
        float entityVelocity = GetComponent<Rigidbody2D>().velocity.x;

        Attack();

        _animator.SetBool("IsEating", isEating);
        _animator.SetFloat("Velocity", entityVelocity);

        /*
         * Obrót srpite'a jednostki
         */
        if (entityVelocity > 0 && _entityStatus.isFacedRight && (Time.timeScale != 0))
        {
            _entityStatus.isFacedRight = false;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        if (entityVelocity < 0 && !_entityStatus.isFacedRight && (Time.timeScale != 0))
        {
            _entityStatus.isFacedRight = true;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
    }

    public void LookAtPlayer()
    {
        if (_player.transform.position.x > transform.position.x && _entityStatus.isFacedRight)
        {
            _entityStatus.isFacedRight = false;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        if (_player.transform.position.x < transform.position.x && !_entityStatus.isFacedRight)
        {
            _entityStatus.isFacedRight = true;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
        
    }
    
    public void Attack()
    {
        if (_enemyAI.canAttack && !isAttacking)
        {        
            LookAtPlayer();

            StartCoroutine(AttackCoroutine());
        }
    }
    private IEnumerator AttackCoroutine()
    {
        _enemyAI.FreezeMovement();
        isAttacking = true;

        // Trigger the attack animation
        _animator.SetBool("isAttacking", true);
        _animator.SetTrigger("Attack1");

        yield return new WaitForSeconds(1f);
        
        // Reset attack state
        _animator.SetBool("isAttacking", false);
        isAttacking = false;
        _enemyAI.RestoreMovement();

    }
    
    public void DealDamage()
    {        
        if(_enemyAI.canAttack)
            _player.GetComponent<EntityStatus>().DealDamage(_entityStatus.AttackDamage, transform.gameObject);
    }
    
}

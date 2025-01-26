using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Dragonfly : MonoBehaviour
{
    public bool isAttacking = false;
    private Animator _animator;
    private EntityStatus _entityStatus;
    private GameObject _entityBody;
    private EnemyAI _enemyAI;

    public void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _entityStatus = GetComponent<EntityStatus>();
        _enemyAI = GetComponent<EnemyAI>();
        _entityBody = gameObject.transform.Find("Graphics").transform.Find("Body").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float entityVelocity = GetComponent<Rigidbody2D>().velocity.x;

        //Attack();


        /*
         * Obrót srpite'a jednostki
         */
        if (entityVelocity > 0 && !_entityStatus.isFacedRight && (Time.timeScale != 0))
        {
            _entityStatus.isFacedRight = true;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }

        if (entityVelocity < 0 && _entityStatus.isFacedRight && (Time.timeScale != 0))
        {
            _entityStatus.isFacedRight = false;
            _entityBody.transform.Rotate(new Vector3(0f, 180f, 0f));
        }
        
    }

    public void Attack()
    {
        if (_enemyAI.canAttack && !isAttacking)
        {
            _animator.SetBool("isAttacking", true);
            _animator.SetTrigger("Attack1");
            isAttacking = true;
        }
        else if (!_enemyAI.canAttack)
        {
            _animator.SetBool("isAttacking", false);
            isAttacking = false;
        }
    }
    
}
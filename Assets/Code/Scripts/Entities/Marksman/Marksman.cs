using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marksman : MonoBehaviour
{
    
    public bool isAttacking = false;

    public GameObject grenadePrefab;
    
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
    
    
    public void Attack()
    {
        if (_enemyAI.canAttack && !isAttacking)
        {
            isAttacking = true;
            LookAtPlayer();

            string trig = attackTriggers[Random.Range(0, attackTriggers.Length)];
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

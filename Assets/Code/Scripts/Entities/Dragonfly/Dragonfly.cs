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
    private GameObject _player;
    
    public string animToPlay;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletSpeed;

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

    public void PlayDragonflySFXSingle(AudioClip audioClip, Enums.SoundType soundType, float pitchMultiplier = 1f)
    {
        if (WorldSoundFXManager.instance == null)
            return;

        if (WorldSoundFXManager.instance.gameState == Enums.GameState.Paused)
            return;

        float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
        WorldSoundFXManager.instance.PlaySoundFX(audioClip, soundType, randomPitch * pitchMultiplier);
    }
    public void PlayDragonflySFXArray(AudioClip[] audioArray, Enums.SoundType soundType, float pitchMultiplier = 1f)
    {
        if (WorldSoundFXManager.instance == null)
            return;

        if (WorldSoundFXManager.instance.gameState == Enums.GameState.Paused)
            return;

        float randomPitch = UnityEngine.Random.Range(0.85f, 1.14f);
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(audioArray, soundType, randomPitch * pitchMultiplier);
    }

    public void Attack()
    {
        if (_enemyAI.canAttack && !isAttacking)
        {
            isAttacking = true;

            _animator.Play(animToPlay);
        }
    }

    public void PerformShoot()
    {
        // Oblicz wektor kierunku od punktu strzału do pozycji gracza
        if (_entityStatus.detectedTargets.Count == 0)
            return;

        Vector3 shootDirection = (_entityStatus.detectedTargets[0].transform.position - bulletSpawn.position).normalized;

        // Utwórz nowy pocisk z prefabrykatu, oraz wprowadź do niego dane o strzelcu
        GameObject projectile = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        if (bulletBehaviour != null)
        {
            bulletBehaviour.SetShooter(gameObject); // Przekazanie referencji na obiekt, który wystrzelił kulę
        }

        // Odtwórz dźwięk strzału
        PlayDragonflySFXArray(WorldSoundFXManager.instance.dragonflyAttackSFX, Enums.SoundType.SFX);

        // Ustaw prędkość pocisku w kierunku gracza
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        projectileRigidbody.velocity = shootDirection * bulletSpeed;

        // Obróć pocisk, aby wskazywał w kierunku ruchu
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, shootDirection) * Quaternion.Euler(0, 0, 90);
        projectile.transform.rotation = rotation;

        // Zniszcz pocisk po określonym czasie (jeśli nie trafi w cel)
        Destroy(projectile, 1f);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShivernDog : MonoBehaviour
{
    [FormerlySerializedAs("IsEating")] public bool isEating = false;
    private Animator _animator;
	private EntityStatus _entityStatus;
    private GameObject _entityBody;

    public void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _entityStatus = GetComponent<EntityStatus>();
        _entityBody = gameObject.transform.Find("Graphics").transform.Find("Body").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float entityVelocity = GetComponent<Rigidbody2D>().velocity.x;

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
}

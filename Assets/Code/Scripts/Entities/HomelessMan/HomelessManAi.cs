using System;
using UnityEngine;

public class HomelessManAi : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float interactionRange = 4f;

    private GameObject player;
    private Animator animator;
    private Transform appearanceTransform;
    private bool isWaving = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player GameObject has the tag 'Player'.");
        }

        appearanceTransform = transform.Find("Appearance");
        if (appearanceTransform == null)
        {
            Debug.LogError("Appearance child not found!");
        }
        else
        {
            animator = appearanceTransform.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component missing on Appearance.");
            }
        }
    }

    private void Update()
    {
        if (player == null || appearanceTransform == null || animator == null) return;

        // Obracanie sprite w stronę gracza
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Vector3 scale = appearanceTransform.localScale;
        scale.x = directionToPlayer.x >= 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x); // Flip X jeśli gracz po prawej
        appearanceTransform.localScale = scale;

        // Sprawdzanie dystansu do gracza
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectionRange)
        {
            if (!isWaving)
            {
                animator.SetTrigger("wave");
                isWaving = true;
            }
        }
        else
        {
            if (isWaving)
            {
                animator.SetTrigger("stopWaving");
                isWaving = false;
            }
        }
    }
}
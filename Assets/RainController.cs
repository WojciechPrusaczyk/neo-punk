using System;
using System.Collections;
using UnityEngine;

public class RainController : MonoBehaviour
{
    [Header("Czas w sekundach przed aktywacją/dezaktywacją deszczu")]
    public float enterDelay = 2.0f;
    public float exitDelay = 0.0f;

    [Header("Warstwa przeszkód raycast'a")]
    public LayerMask roofLayer;

    private GameObject player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool rainActive = false;
    public bool isUnderRoof = true;

    private Coroutine currentCoroutine = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (WorldGameManager.instance != null)
            player = WorldGameManager.instance.player?.gameObject;
        else
            player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        spriteRenderer.enabled = rainActive;
        CheckRainCondition();
    }

    private void CheckRainCondition()
    {
        if (player == null) return;

        Vector2 origin = player.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, Mathf.Infinity, roofLayer);

        bool nowUnderRoof = hit.collider != null;

        if (nowUnderRoof != isUnderRoof)
        {
            isUnderRoof = nowUnderRoof;

            if (!isUnderRoof)
            {
                RestartCoroutine(() => SetRain(true), enterDelay);
            }
            else
            {
                RestartCoroutine(() => SetRain(false), exitDelay);
            }
        }

        // debug wizualny
        Debug.DrawRay(origin, Vector2.up * 5f, hit.collider ? Color.red : Color.green);
    }

    private void RestartCoroutine(Action action, float delay)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(DelayedAction(action, delay));
    }

    private IEnumerator DelayedAction(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }

    private void SetRain(bool value)
    {
        rainActive = value;
    }
}
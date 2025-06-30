using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal; // example component

[RequireComponent(typeof(CircleCollider2D))]
public class ProximityActivator : MonoBehaviour
{
    [Header("Activation range")]
    [SerializeField] private float radius = 20f;

    [Header("Things to toggle")]
    [SerializeField] private MonoBehaviour[] scripts;   // AI, Animator, etc.
    [SerializeField] private Animator[] animators;
    [SerializeField] private Collider2D[] colliders;
    

    private bool isActive;

    private void Reset()
    {
        var trigger = GetComponent<CircleCollider2D>();
        trigger.isTrigger = true;
        trigger.radius    = radius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        SetActiv(true);
    }

    private void SetActiv(bool value)
    {
        if (value == isActive) return;
        isActive = value;

        foreach (var s in scripts)   if (s) s.enabled = value;
        foreach (var s in animators)   if (s) s.enabled = value;
        foreach (var s in colliders)   if (s) s.enabled = value;
        Debug.Log("activating Enemy");
    }
}
using UnityEngine;
using UnityEngine.AI;       // example component

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
        var trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius    = radius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Activated");
        SetActive(true);
    }

    private void SetActive(bool value)
    {
        if (value == isActive) return;
        isActive = value;

        foreach (var s in scripts)   if (s) s.enabled = value;
        foreach (var s in animators)   if (s) s.enabled = value;
        foreach (var s in colliders)   if (s) s.enabled = value;
    }
}
using System.Collections;
using UnityEngine;

public class MarksmanGrenade : MonoBehaviour
{
    public Animator animator;
    public GameObject entityInArea;
    public GameObject particles;
    
    public float damage;
    public float lifeTime;

    private void Start()
    {
        StartCoroutine(AutoExplode());
    }

    private IEnumerator AutoExplode()
    {
        yield return new WaitForSeconds(lifeTime);
        yield return StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        animator.enabled = true;

        float dur = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        if (entityInArea != null)
        {
            EntityStatus entityStatus = entityInArea.GetComponentInChildren<EntityStatus>();
            entityStatus.DealDamage(damage);
        }
        yield return new WaitForSeconds(.2f);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        
        particles.SetActive(true);
        
        yield return new WaitForSeconds(dur);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            entityInArea = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            entityInArea = null;
        }
    }
}
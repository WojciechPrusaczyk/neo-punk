using System.Collections;
using UnityEngine;

public class HpBarController : MonoBehaviour
{
    public EntityStatus entityStatus;
    public Sprite normalTypeIcon;
    public Sprite cyberTypeIcon;
    public Sprite demonTypeIcon;

    private Material material;
    private float lastHpRatio = -1f;
    private SpriteRenderer enemyIcon;

    void Start()
    {
        // Pasek HP to ten obiekt
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            enabled = false;
            return;
        }

        material = renderer.material;

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr != renderer)
            {
                enemyIcon = sr;
                break;
            }
        }

        if (enemyIcon == null)
        {
            Debug.LogWarning("HpBarController: Nie znaleziono ikony w dzieciach.");
        }
    }

    void Update()
    {
        if (entityStatus == null || material == null)
            return;

        float currentRatio = entityStatus.GetHp() / entityStatus.GetMaxHp();
        if (!Mathf.Approximately(currentRatio, lastHpRatio))
        {
            SetFill(currentRatio);
            lastHpRatio = currentRatio;
        }

        switch (entityStatus.entityType)
        {
            case Enums.EntityType.Human:
                enemyIcon.sprite = cyberTypeIcon;
                break;
            case Enums.EntityType.Cyber:
                enemyIcon.sprite = cyberTypeIcon;
                break;
            case Enums.EntityType.Demon:
                enemyIcon.sprite = cyberTypeIcon;
                break;
        }
    }

    private void SetFill(float percent)
    {
        var fillPercent = Mathf.Clamp01(percent);
        material.SetFloat("_Cutoff", fillPercent);
    }
}
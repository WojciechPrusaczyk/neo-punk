using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HpBarController : MonoBehaviour
{
    public EntityStatus entityStatus;

    [Header("Normal type")]
    public Sprite normalTypeIcon;
    public Color normalTypeColor;

    [Header("Cyber type")]
    public Sprite cyberTypeIcon;
    public Color cyberTypeColor;

    [Header("Cemon type")]
    public Sprite demonTypeIcon;
    public Color demonTypeColor;

    private Material material;
    private float lastHpRatio = -1f;
    private SpriteRenderer enemyIcon;
    private Light2D enemyLight;

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

        enemyIcon = gameObject.transform.Find("TypeIcon").GetComponent<SpriteRenderer>();
        enemyLight = gameObject.transform.Find("TypeIcon").GetComponent<Light2D>();

        if (enemyIcon == null)
        {
            Debug.LogWarning("HpBarController: Nie znaleziono ikony w dzieciach.");
        }
        if (enemyLight == null)
        {
            Debug.LogWarning("HpBarController: Nie znaleziono światła w dzieciach.");
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
                enemyIcon.sprite = normalTypeIcon;
                enemyLight.color = normalTypeColor;
                break;
            case Enums.EntityType.Cyber:
                enemyIcon.sprite = cyberTypeIcon;
                enemyLight.color = cyberTypeColor;
                break;
            case Enums.EntityType.Demon:
                enemyIcon.sprite = demonTypeIcon;
                enemyLight.color = demonTypeColor;
                break;
        }
    }

    private void SetFill(float percent)
    {
        var fillPercent = Mathf.Clamp01(percent);
        material.SetFloat("_Cutoff", fillPercent);
    }
}
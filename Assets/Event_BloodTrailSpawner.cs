using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_BloodTrailSpawner : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> bloodTrails;

    public void EnableAllBloodTrails()
    {
        if (bloodTrails == null || bloodTrails.Count == 0)
            return;

        foreach (var item in bloodTrails)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void DisableAllBloodTrails()
    {
        if (bloodTrails == null || bloodTrails.Count == 0)
            return;

        foreach (var item in bloodTrails)
        {
            item.gameObject.SetActive(false);
        }
    }
}

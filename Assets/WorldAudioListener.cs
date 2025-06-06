using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
public class WorldAudioListener : MonoBehaviour
{
    Player Player => FindFirstObjectByType<Player>();
    private void Update()
    {
        if (Player != null)
        {
            transform.position = Player.transform.position;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData : MonoBehaviour
{
    public int bossID;
    public string bossFlag;



    public void OnDeath()
    {
        MusicManager.instance.PlaySong(MusicManager.instance.Level1Track, Enums.SoundType.Music);
        EventFlagsSystem eventsFlagsSystem = EventFlagsSystem.instance;
        
        if (!eventsFlagsSystem.IsEventDone(bossFlag))
        {
            eventsFlagsSystem.FinishEvent(bossFlag);
        }
    }
}

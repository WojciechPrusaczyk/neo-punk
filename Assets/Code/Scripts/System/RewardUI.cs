using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    public TimeTrial timetrial;
    public List<TMPro.TextMeshProUGUI> texts;


    private void OnEnable()
    {
        foreach (var text in texts)
        {
            int index = texts.IndexOf(text);
            text.text = timetrial.FormatTime(timetrial.medalTimes[index]);
        }
    }
}

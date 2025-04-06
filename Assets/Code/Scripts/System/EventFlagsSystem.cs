using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFlagsSystem : MonoBehaviour
{
    /*
     * Zmienna listowa przechowująca aktualne typy obrażeń
     */
    [Serializable]
    public class EventFlag
    {
        [SerializeField] public string name;
        public bool isDone;
    }

    public List<EventFlag> eventFlags = new List<EventFlag>();

    public void FinishEvent(int eventIndex)
    {
        if (eventIndex < 0 || eventIndex >= eventFlags.Count)
        {
            Debug.LogWarning("EventFlag Index out of range.");
            return;
        }

        eventFlags[eventIndex].isDone = true;
    }

    public void FinishEvent(string eventName)
    {
        if (eventName == null || eventName.Length == 0)
        {
            Debug.LogWarning("EventFlag name in invalid.");
            return;
        }

        var eventFlag = eventFlags.Find(x => x.name == eventName);

        if (eventFlag == null)
        {
            Debug.LogWarning("EventFlag not found with provided name.");
            return;
        }

        eventFlag.isDone = true;
    }

    public bool IsEventDone(int eventIndex)
    {
        if (eventIndex < 0 || eventIndex >= eventFlags.Count)
        {
            Debug.LogWarning("EventFlag Index out of range.");
            return false;
        }

        return eventFlags[eventIndex].isDone;
    }

    public bool IsEventDone(string eventName)
    {
        if (eventName == null || eventName.Length == 0)
        {
            Debug.LogWarning("EventFlag name in invalid.");
            return false;
        }

        var eventFlag = eventFlags.Find(x => x.name == eventName);

        if (eventFlag == null)
        {
            Debug.LogWarning("EventFlag not found with provided name.");
            return false;
        }

        return eventFlag.isDone;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomTags : MonoBehaviour
{
    [SerializeField]
    [Tooltip("List of tags associated with this GameObject.")]
    private List<string> tags = new List<string>();

    // UnityEvents for tag changes (optional)
    public UnityEvent<string> OnTagAdded;
    public UnityEvent<string> OnTagRemoved;

    
    public void AddTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            Debug.LogWarning("Cannot add a null or empty tag.");
            return;
        }

        if (!tags.Contains(tag))
        {
            tags.Add(tag);
            OnTagAdded?.Invoke(tag);
        }
    }

    public void RemoveTag(string tag)
    {
        if (tags.Contains(tag))
        {
            tags.Remove(tag);
            OnTagRemoved?.Invoke(tag);
        }
    }

    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }

    public List<string> GetTags()
    {
        return new List<string>(tags);
    }

    public void ClearTags()
    {
        tags.Clear();
    }

    private void OnValidate()
    {
        HashSet<string> uniqueTags = new HashSet<string>(tags);
        tags = new List<string>(uniqueTags);
    }
}
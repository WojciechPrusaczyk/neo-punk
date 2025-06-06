using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnityMainThread
{
    private static readonly Queue<Action> _actions = new Queue<Action>();
    private static MainThreadRunner _runner;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_runner == null)
        {
            GameObject go = new GameObject("MainThreadRunner_Singleton");
            _runner = go.AddComponent<MainThreadRunner>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
    }

    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            Debug.LogWarning("UnityMainThread: Null action queued.");
            return;
        }

        lock (_actions)
        {
            _actions.Enqueue(action);
        }
    }

    private class MainThreadRunner : MonoBehaviour
    {
        void Update()
        {
            lock (_actions)
            {
                while (_actions.Count > 0)
                {
                    Action action = _actions.Dequeue();
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"UnityMainThread: Error executing action: {ex}");
                    }
                }
            }
        }
    }
}
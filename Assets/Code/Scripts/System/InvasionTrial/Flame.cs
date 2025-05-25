using System;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public Action Finished;      // subscribers get a ping

    // ← Animation Event calls this
    private void FxFinished()          // must match event name exactly
    {
        Finished?.Invoke();
        Destroy(gameObject);           // auto-clean
    }
}

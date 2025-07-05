using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableBrokenFly : Interactable
{
    private DialogScript dialogInterface;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        base.OnSceneLoaded(scene, mode);

        dialogInterface = FindFirstObjectByType<DialogScript>();
    }

    protected override void Interact()
    {
        if (EventFlagsSystem.instance == null)
            return;

        if (EventFlagsSystem.instance.IsEventDone("FoundBrokenDragonfly"))
            return;

        base.Interact();
        dialogInterface.StartDialog(6);
    }
    protected override void PrepareTriggerEnterPlayer(Collider2D collision)
    {
        if (EventFlagsSystem.instance.IsEventDone("FoundBrokenDragonfly"))
            return;

        base.PrepareTriggerEnterPlayer(collision);
    }
}

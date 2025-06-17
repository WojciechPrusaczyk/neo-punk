using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbominationArenaCollider : ArenaCollider
{
    public override void CloseArena()
    {
        if (EventFlagsSystem.instance.IsEventDone("AbominationDefeat"))
            return;

        base.CloseArena();
    }

    public override void OpenArena()
    {
        if (EventFlagsSystem.instance.IsEventDone("AbominationDefeat"))
            return;

        base.OpenArena();
    }
}

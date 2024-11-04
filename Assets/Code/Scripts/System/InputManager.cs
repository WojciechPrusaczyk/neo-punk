using UnityEngine;

public static class InputManager
{
    /*
     * kontrolki dla klawiatur
     */
    public static KeyCode JumpKey { get; private set; } = KeyCode.Space; // Klawisz skoku (domyślnie Spacja)
    public static KeyCode AltJumpKey { get; private set; } = KeyCode.Space; // Klawisz skoku (domyślnie Spacja)
    public static KeyCode BlockKey { get; private set; } = KeyCode.Mouse1; // Klawisz blokowania (domyślnie prawy przycisk myszy)
    public static KeyCode InteractKey { get; private set; } = KeyCode.F; // Klawisz interakcji z otoczeniem (domyślnie F)
    public static KeyCode InventoryMenuKey { get; private set; } = KeyCode.I; // Klawisz otwarcia menu przedmiotów (domyślnie I)
    public static KeyCode PauseMenuKey { get; private set; } = KeyCode.Escape; // Klawisz otwarcia menu pauzy (domyślnie Esc)
    public static KeyCode Item1Key { get; private set; } = KeyCode.Alpha1; // Klawisz używania przedmiotu 1 (domyślnie klawisz 1)
    public static KeyCode Item2Key { get; private set; } = KeyCode.Alpha2; // Klawisz używania przedmiotu 2 (domyślnie klawisz 2)
    public static KeyCode Item3Key { get; private set; } = KeyCode.Alpha3; // Klawisz używania przedmiotu 3 (domyślnie klawisz 3)
    public static KeyCode Item4Key { get; private set; } = KeyCode.Alpha4; // Klawisz używania przedmiotu 4 (domyślnie klawisz 4)
    public static KeyCode AttackKey { get; private set; } = KeyCode.Mouse0; // Domyślny klawisz ataku (lewy przycisk myszy)
    public static KeyCode MoveLeftKey { get; private set; } = KeyCode.A; // Klawisz poruszania się w lewo (domyślnie A)
    public static KeyCode MoveRightKey { get; private set; } = KeyCode.D; // Klawisz poruszania się w prawo (domyślnie D)
    public static KeyCode MoveDownKey { get; private set; } = KeyCode.S; // Klawisz poruszania się w dół (domyślnie S)
    public static KeyCode DodgeKey { get; private set; } = KeyCode.LeftShift; // Klawisz uniku (domyślnie Shift)

    /*
     * kontrolki dla padów
     */
    public static string PadButtonAttack { get; private set; } = "Fire1"; // Przycisk ataku na padzie (domyślnie "Fire1")
    public static string PadButtonJump { get; private set; } = "Jump"; // Przycisk skoku na padzie (domyślnie "Jump")
    public static string AltPadButtonJump { get; private set; } = "Jump"; // Przycisk skoku na padzie (domyślnie "Jump")
    public static string PadButtonBlock { get; private set; } = "Block"; // Przycisk blokowania na padzie (domyślnie "Block")
    public static string PadButtonInteract { get; private set; } = "Interact"; // Przycisk interakcji na padzie (domyślnie "Interact")
    public static string PadButtonInventoryMenu { get; private set; } = "InventoryMenu"; // Przycisk otwarcia menu przedmiotów na padzie (domyślnie "InventoryMenu")
    public static string PadButtonPauseMenu { get; private set; } = "PauseMenu"; // Przycisk otwarcia menu pauzy na padzie (domyślnie "PauseMenu")
    public static string PadButtonItem1 { get; private set; } = "Item1"; // Przycisk używania przedmiotu 1 na padzie (domyślnie "Item1")
    public static string PadButtonItem2 { get; private set; } = "Item2"; // Przycisk używania przedmiotu 2 na padzie (domyślnie "Item2")
    public static string PadButtonItem3 { get; private set; } = "Item3"; // Przycisk używania przedmiotu 3 na padzie (domyślnie "Item3")
    public static string PadButtonItem4 { get; private set; } = "Item4"; // Przycisk używania przedmiotu 4 na padzie (domyślnie "Item4")
    public static string PadButtonDodge { get; private set; } = "Dodge"; // Przycisk uniku na padzie (domyślnie "Dodge")
    public static string PadMoveDownKey { get; private set; } = "Down"; // Klawisz poruszania się w dół (domyślnie S)

    
    /*
     * Metody do zmiany przycisków
     */
    public static void ChangeAttackKey(KeyCode newKey)
    {
        AttackKey = newKey;
    }
    
    public static void ChangeJumpKey(KeyCode newKey)
    {
        JumpKey = newKey;
    }
    
    public static void ChangeAltJumpKey(KeyCode newKey)
    {
        AltJumpKey = newKey;
    }

    public static void ChangeBlockKey(KeyCode newKey)
    {
        BlockKey = newKey;
    }

    public static void ChangeInteractKey(KeyCode newKey)
    {
        InteractKey = newKey;
    }

    public static void ChangeInventoryMenuKey(KeyCode newKey)
    {
        InventoryMenuKey = newKey;
    }

    public static void ChangePauseMenuKey(KeyCode newKey)
    {
        PauseMenuKey = newKey;
    }

    public static void ChangeItem1Key(KeyCode newKey)
    {
        Item1Key = newKey;
    }

    public static void ChangeItem2Key(KeyCode newKey)
    {
        Item2Key = newKey;
    }

    public static void ChangeItem3Key(KeyCode newKey)
    {
        Item3Key = newKey;
    }

    public static void ChangeItem4Key(KeyCode newKey)
    {
        Item4Key = newKey;
    }
    
    public static void ChangeMoveLeftKey(KeyCode newKey)
    {
        MoveLeftKey = newKey;
    }

    public static void ChangeMoveRightKey(KeyCode newKey)
    {
        MoveRightKey = newKey;
    }
    
    public static void ChangeDodgeKey(KeyCode newKey)
    {
        DodgeKey = newKey;
    }
    
    public static void ChangeMoveDownKey(KeyCode newKey)
    {
        MoveDownKey = newKey;
    }
    
    /*
     * Zmiana kontrolek dla padów:
     */

    public static void ChangePadButtonJump(string newButton)
    {
        PadButtonJump = newButton;
    }

    public static void ChangePadAltButtonJump(string newButton)
    {
        AltPadButtonJump = newButton;
    }

    public static void ChangePadButtonBlock(string newButton)
    {
        PadButtonBlock = newButton;
    }

    public static void ChangePadButtonInteract(string newButton)
    {
        PadButtonInteract = newButton;
    }

    public static void ChangePadButtonInventoryMenu(string newButton)
    {
        PadButtonInventoryMenu = newButton;
    }

    public static void ChangePadButtonAttack(string newButton)
    {
        PadButtonAttack = newButton;
    }
    
    public static void ChangePadButtonPauseMenu(string newButton)
    {
        PadButtonPauseMenu = newButton;
    }

    public static void ChangePadButtonItem1(string newButton)
    {
        PadButtonItem1 = newButton;
    }

    public static void ChangePadButtonItem2(string newButton)
    {
        PadButtonItem2 = newButton;
    }

    public static void ChangePadButtonItem3(string newButton)
    {
        PadButtonItem3 = newButton;
    }

    public static void ChangePadButtonItem4(string newButton)
    {
        PadButtonItem4 = newButton;
    }
    
    public static void ChangePadButtonDodge(string newButton)
    {
        PadButtonDodge = newButton;
    }
    
    public static void ChangePadButtonMoveDown(string newKey)
    {
        PadMoveDownKey = newKey;
    }
}

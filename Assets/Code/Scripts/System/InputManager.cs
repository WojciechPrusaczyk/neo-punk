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
    // Przycisk ataku na padzie (domyślnie Fire1 -> JoystickButton0 - A)
    public static KeyCode PadButtonAttack { get; private set; } = KeyCode.JoystickButton0;

    // Przycisk skoku na padzie (domyślnie JoystickButton1 - B)
    public static KeyCode PadButtonJump { get; private set; } = KeyCode.JoystickButton1;

    // Alternatywny przycisk skoku na padzie (np. dodatkowy mapping)
    public static KeyCode AltPadButtonJump { get; private set; } = KeyCode.JoystickButton1;

    // Przycisk blokowania na padzie (domyślnie JoystickButton2 - X)
    public static KeyCode PadButtonBlock { get; private set; } = KeyCode.JoystickButton2;

    // Przycisk interakcji na padzie (domyślnie JoystickButton3 - Y)
    public static KeyCode PadButtonInteract { get; private set; } = KeyCode.JoystickButton3;

    // Przycisk otwarcia menu przedmiotów na padzie (domyślnie JoystickButton7 - Start)
    public static KeyCode PadButtonInventoryMenu { get; private set; } = KeyCode.JoystickButton7;

    // Przycisk otwarcia menu pauzy na padzie (domyślnie JoystickButton6 - Back)
    public static KeyCode PadButtonPauseMenu { get; private set; } = KeyCode.JoystickButton6;

    // Przycisk używania przedmiotu 1 na padzie (domyślnie JoystickButton4 - LB)
    public static KeyCode PadButtonItem1 { get; private set; } = KeyCode.JoystickButton4;

    // Przycisk używania przedmiotu 2 na padzie (domyślnie JoystickButton5 - RB)
    public static KeyCode PadButtonItem2 { get; private set; } = KeyCode.JoystickButton5;

    // Przycisk używania przedmiotu 3 na padzie (domyślnie JoystickButton8 - L3)
    public static KeyCode PadButtonItem3 { get; private set; } = KeyCode.JoystickButton8;

    // Przycisk używania przedmiotu 4 na padzie (domyślnie JoystickButton9 - R3)
    public static KeyCode PadButtonItem4 { get; private set; } = KeyCode.JoystickButton9;

    // Przycisk uniku na padzie (domyślnie JoystickButton4 - LB)
    public static KeyCode PadButtonDodge { get; private set; } = KeyCode.JoystickButton4;

    // Klawisz poruszania się w dół (Joystick Axis lub dodatkowy mapping, domyślnie Down na padzie)
    public static KeyCode PadMoveDownKey { get; private set; } = KeyCode.JoystickButton10; // Placeholder
    
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

    public static void ChangePadButtonJump(KeyCode newButton)
    {
        PadButtonJump = newButton;
    }

    public static void ChangePadAltButtonJump(KeyCode newButton)
    {
        AltPadButtonJump = newButton;
    }

    public static void ChangePadButtonBlock(KeyCode newButton)
    {
        PadButtonBlock = newButton;
    }

    public static void ChangePadButtonInteract(KeyCode newButton)
    {
        PadButtonInteract = newButton;
    }

    public static void ChangePadButtonInventoryMenu(KeyCode newButton)
    {
        PadButtonInventoryMenu = newButton;
    }

    public static void ChangePadButtonAttack(KeyCode newButton)
    {
        PadButtonAttack = newButton;
    }
    
    public static void ChangePadButtonPauseMenu(KeyCode newButton)
    {
        PadButtonPauseMenu = newButton;
    }

    public static void ChangePadButtonItem1(KeyCode newButton)
    {
        PadButtonItem1 = newButton;
    }

    public static void ChangePadButtonItem2(KeyCode newButton)
    {
        PadButtonItem2 = newButton;
    }

    public static void ChangePadButtonItem3(KeyCode newButton)
    {
        PadButtonItem3 = newButton;
    }

    public static void ChangePadButtonItem4(KeyCode newButton)
    {
        PadButtonItem4 = newButton;
    }
    
    public static void ChangePadButtonDodge(KeyCode newButton)
    {
        PadButtonDodge = newButton;
    }
    
    public static void ChangePadButtonMoveDown(KeyCode newKey)
    {
        PadMoveDownKey = newKey;
    }
}

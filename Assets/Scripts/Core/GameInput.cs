using UnityEngine.InputSystem;

public static class GameInput
{
    public static bool SpacePressedThisFrame =>
        Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

    public static bool EnterPressedThisFrame =>
        Keyboard.current != null &&
        (Keyboard.current.enterKey.wasPressedThisFrame ||
         Keyboard.current.numpadEnterKey.wasPressedThisFrame);
}

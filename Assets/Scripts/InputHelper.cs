using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public static class InputHelper
{
#if ENABLE_INPUT_SYSTEM
    private static Keyboard Keyboard => Keyboard.current;
#endif

    public static bool IsLeftHeld()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed);
#else
        return false;
#endif
    }

    public static bool IsRightHeld()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed);
#else
        return false;
#endif
    }

    public static bool IsJumpPressedDown()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && (keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame);
#else
        return false;
#endif
    }

    public static bool IsInteractPressedDown()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && keyboard.eKey.wasPressedThisFrame;
#else
        return false;
#endif
    }

    public static bool IsDiaryPressedDown()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && keyboard.qKey.wasPressedThisFrame;
#else
        return false;
#endif
    }

    public static bool IsScratchPressedDown()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && keyboard.rKey.wasPressedThisFrame;
#else
        return false;
#endif
    }

    public static bool IsDebugTogglePressedDown()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && keyboard.f1Key.wasPressedThisFrame;
#else
        return false;
#endif
    }

    public static bool IsConfirmPressedDown()
    {
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard;
        return keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame);
#else
        return false;
#endif
    }
}

using System.Collections.Generic;
using UnityEngine.InputSystem;

public static class CharacterSelectionData
{
    public static List<int> selectedCharacterIndices = new List<int>();
    public static List<InputDevice> playerInputDevices = new List<InputDevice>();

    public static void Clear()
    {
        selectedCharacterIndices.Clear();
        playerInputDevices.Clear();
    }
}

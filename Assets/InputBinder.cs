using UnityEngine;
using UnityEngine.InputSystem;

public class InputBinder : MonoBehaviour
{
    public PlayerInput player1Input;
    public PlayerInput player2Input;

    private void Start()
    {
        var gamepads = Gamepad.all;
        if (gamepads.Count >= 2)
        {
            player1Input.SwitchCurrentControlScheme(gamepads[0]);
            player2Input.SwitchCurrentControlScheme(gamepads[1]);

            player1Input.ActivateInput();
            player2Input.ActivateInput();
        }
        else
        {
            Debug.LogWarning("Not enough gamepads connected!");
        }
    }
}

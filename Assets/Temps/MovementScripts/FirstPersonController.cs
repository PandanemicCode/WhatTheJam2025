using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public MovementSettings movementSettings;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float lookClamp = 85f;

    private Player_Input inputActions;
    private CharacterController controller;

    private Vector2 inputMove;
    private Vector2 inputLook;
    private float verticalSpeed;
    private float currentCameraPitch = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new Player_Input();

        // Bind movement input
        inputActions.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => inputMove = Vector2.zero;

        // Bind mouse look
        inputActions.Player.Look.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => inputLook = Vector2.zero;

        // Bind jump
        inputActions.Player.Jump.performed += ctx => Jump();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        float mouseX = inputLook.x * mouseSensitivity;
        float mouseY = inputLook.y * mouseSensitivity;

        currentCameraPitch -= mouseY;
        currentCameraPitch = Mathf.Clamp(currentCameraPitch, -lookClamp, lookClamp);

        cameraTransform.localRotation = Quaternion.Euler(currentCameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        Vector3 move = transform.right * inputMove.x + transform.forward * inputMove.y;

        // Gravity
        if (controller.isGrounded && verticalSpeed < 0)
            verticalSpeed = -2f;

        verticalSpeed += movementSettings.gravity * Time.deltaTime;

        move.y = verticalSpeed;

        controller.Move(move * movementSettings.moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            verticalSpeed = Mathf.Sqrt(2f * -movementSettings.gravity * movementSettings.jumpHeight);
        }
    }
}



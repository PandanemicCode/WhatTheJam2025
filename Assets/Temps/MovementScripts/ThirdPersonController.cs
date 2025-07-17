using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public MovementSettings movementSettings;

    private Player_Input input;
    private CharacterController controller;

    private Vector2 inputMove;
    private bool isSprinting = false;
    private float verticalVelocity;
    private float groundedTimer = 0.2f; // Grace time

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new Player_Input();

        input.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => inputMove = Vector2.zero;

        input.Player.Jump.performed += ctx => TryJump();
        input.Player.Sprint.started += ctx => isSprinting = true;
        input.Player.Sprint.canceled += ctx => isSprinting = false;
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
{
    bool isGrounded = controller.isGrounded;
    if (isGrounded && verticalVelocity < 0)
        verticalVelocity = -2f;

    // Convert input into camera-relative movement
    Vector3 camForward = cameraTransform.forward;
    Vector3 camRight = cameraTransform.right;
    camForward.y = camRight.y = 0f;
    camForward.Normalize();
    camRight.Normalize();

    Vector3 move = camForward * inputMove.y + camRight * inputMove.x;

    // ✨ Rotate player to face move direction (if moving)
    if (move.magnitude > 0.1f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movementSettings.rotationSpeed * Time.deltaTime);
    }

    float speed = isSprinting ? movementSettings.moveSpeed * 1.5f : movementSettings.moveSpeed;
    controller.Move(move * speed * Time.deltaTime);

    verticalVelocity += movementSettings.gravity * Time.deltaTime;
    controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
}

    void TryJump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(2f * -movementSettings.gravity * movementSettings.jumpHeight);
        }
    }
}

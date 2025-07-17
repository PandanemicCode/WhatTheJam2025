using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;                  // Main camera for camera-relative movement
    public MovementSettings movementSettings;          // Custom movement configuration

    private Player_Input input;                        // Input system reference
    private CharacterController controller;            // Unity built-in character controller

    private Vector2 inputMove;                         // Stores movement input from gamepad
    private bool isSprinting = false;                  // Is the character sprinting?
    private float verticalVelocity;                    // Tracks falling and jumping vertical movement
    private float groundedTimer = 0.2f;                // Small grace time for jump detection

    // Animation
    private Animator animator;                         // Animator controller reference

    [Header("ParticleTimer")]
    public ParticleSystem wave;                        // Particle system reference (used while idle)
    private Coroutine emissionCoroutine;               // Reference to delayed coroutine

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new Player_Input();

        // Subscribe to input events
        input.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => inputMove = Vector2.zero;

        input.Player.Jump.performed += ctx => TryJump();
        input.Player.Sprint.started += ctx => isSprinting = true;
        input.Player.Sprint.canceled += ctx => isSprinting = false;

        input.Player.Attack.performed += ctx => animator.SetTrigger("Attack");
        input.Player.Interact.performed += ctx => animator.SetTrigger("Interact");

        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// Handles movement input, gravity, rotation, animations, and idle particle effects.
    /// </summary>
    void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        // Reset vertical velocity when grounded
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        // Convert input into camera-relative movement
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * inputMove.y + camRight * inputMove.x;

        // Smoothly rotate player in movement direction
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movementSettings.rotationSpeed * Time.deltaTime);
        }

        // Determine speed (normal or sprinting)
        float speed = isSprinting ? movementSettings.moveSpeed * 1.5f : movementSettings.moveSpeed;

        // Apply movement and gravity
        controller.Move(move * speed * Time.deltaTime);
        verticalVelocity += movementSettings.gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        // Update animation parameters
        animator.SetFloat("Move", move.magnitude);                  // Walk/run blend
        animator.SetBool("IsJump", !controller.isGrounded);        // Jumping state
        animator.SetBool("IsSprint", isSprinting);                 // Sprinting state

        // Particle Wave - delay activation when idle
        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            // If already waiting to emit, cancel that wait
            if (emissionCoroutine != null)
            {
                StopCoroutine(emissionCoroutine);
                emissionCoroutine = null;
            }

            // Stop emitting particles while moving
            var emission = wave.emission;
            emission.enabled = false;
        }
        else
        {
            // Start emitting after delay only if coroutine is not running
            if (emissionCoroutine == null)
            {
                emissionCoroutine = StartCoroutine(EnableEmissionWithDelay(4f));
            }
        }
    }

    /// <summary>
    /// Delays particle emission by X seconds (only when idle)
    /// </summary>
    private IEnumerator EnableEmissionWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var emission = wave.emission;
        emission.enabled = true;
        emissionCoroutine = null;
    }

    /// <summary>
    /// Called when jump button is pressed. Applies jump force if grounded.
    /// </summary>
    void TryJump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(2f * -movementSettings.gravity * movementSettings.jumpHeight);
        }
    }
}

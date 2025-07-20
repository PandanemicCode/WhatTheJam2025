using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;               // Main camera for camera-relative movement
    public MovementSettings movementSettings;       // Movement config (speed, gravity, etc)

    private Player_Input input;                      // Input system reference
    private CharacterController controller;         // Unity's built-in character controller

    private Vector2 inputMove;                       // Stores movement input vector
    private bool isSprinting = false;                // Sprinting flag
    private float verticalVelocity;                  // Y-axis velocity for gravity/jumping

    // Animation
    private Animator animator;                       // Reference to child Animator component

    [Header("ParticleTimer")]
    public ParticleSystem wave;                      // Particle system to activate when idle
    private Coroutine emissionCoroutine;            // Coroutine reference for delayed emission

    // Combat
    private PlayerCombat combat;                      // Reference to PlayerCombat script

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new Player_Input();
        animator = GetComponentInChildren<Animator>();

        combat = GetComponent<PlayerCombat>();
        if (combat == null)
            Debug.LogWarning("PlayerCombat component missing on " + gameObject.name);

        // Movement input subscriptions
        input.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => inputMove = Vector2.zero;

        input.Player.Jump.performed += ctx => TryJump();

        input.Player.Sprint.started += ctx => isSprinting = true;
        input.Player.Sprint.canceled += ctx => isSprinting = false;

        // Animation triggers for attack and interact
        input.Player.Attack.performed += ctx =>
        {
            animator?.SetTrigger("Attack");
            combat?.Attack();
        };

        input.Player.Interact.performed += ctx => animator?.SetTrigger("Interact");
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        HandleMovement();
    }

    /// <summary>
    /// Handles player movement, rotation, animation, gravity, and idle particle emission.
    /// </summary>
    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        // Reset vertical velocity when grounded to small negative to stick to ground
        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        // Get camera-relative movement vector
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * inputMove.y + camRight * inputMove.x;

        // Rotate player toward movement direction smoothly
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movementSettings.rotationSpeed * Time.deltaTime);
        }

        float speed = isSprinting ? movementSettings.moveSpeed * 1.5f : movementSettings.moveSpeed;

        // Move player and apply gravity
        controller.Move(move * speed * Time.deltaTime);
        verticalVelocity += movementSettings.gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        // Update animator parameters
        animator.SetFloat("Move", move.magnitude);
        animator.SetBool("IsJump", !controller.isGrounded);
        animator.SetBool("IsSprint", isSprinting);

        // Handle idle particle emission with delay
        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            if (emissionCoroutine != null)
            {
                StopCoroutine(emissionCoroutine);
                emissionCoroutine = null;
            }

            var emission = wave.emission;
            emission.enabled = false;
        }
        else
        {
            if (emissionCoroutine == null)
            {
                emissionCoroutine = StartCoroutine(EnableEmissionWithDelay(4f));
            }
        }
    }
    
    private IEnumerator EnableEmissionWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var emission = wave.emission;
        emission.enabled = true;
        emissionCoroutine = null;
    }

    private void TryJump()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(2f * -movementSettings.gravity * movementSettings.jumpHeight);
        }
    }
}

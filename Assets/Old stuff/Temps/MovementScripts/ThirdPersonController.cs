using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;               // Main camera for camera-relative movement
    public MovementSettings movementSettings;       // Movement config (speed, gravity, etc)

    private PlayerInput playerInput;                 // PlayerInput component reference
    private CharacterController controller;         

    private Vector2 inputMove;                       
    private bool isSprinting = false;                
    private float verticalVelocity;                  

    // Animation
    private Animator animator;                       

    [Header("ParticleTimer")]
    public ParticleSystem wave;                      
    private Coroutine emissionCoroutine;            

    // Combat
    private PlayerCombat combat;                      

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        combat = GetComponent<PlayerCombat>();
        playerInput = GetComponent<PlayerInput>();

        if (combat == null)
            Debug.LogWarning("PlayerCombat component missing on " + gameObject.name);

        // Subscribe to input actions
        playerInput.actions["Move"].performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        playerInput.actions["Move"].canceled += ctx => inputMove = Vector2.zero;

        playerInput.actions["Jump"].performed += ctx => TryJump();

        playerInput.actions["Sprint"].started += ctx => isSprinting = true;
        playerInput.actions["Sprint"].canceled += ctx => isSprinting = false;

        playerInput.actions["Attack"].performed += ctx =>
        {
            animator?.SetTrigger("Attack");
            combat?.Attack();
        };

        playerInput.actions["Interact"].performed += ctx => animator?.SetTrigger("Interact");
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * inputMove.y + camRight * inputMove.x;

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, movementSettings.rotationSpeed * Time.deltaTime);
        }

        float speed = isSprinting ? movementSettings.moveSpeed * 1.5f : movementSettings.moveSpeed;

        controller.Move(move * speed * Time.deltaTime);
        verticalVelocity += movementSettings.gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        animator.SetFloat("Move", move.magnitude);
        animator.SetBool("IsJump", !controller.isGrounded);
        animator.SetBool("IsSprint", isSprinting);

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
                emissionCoroutine = StartCoroutine(EnableEmissionWithDelay(2f));
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

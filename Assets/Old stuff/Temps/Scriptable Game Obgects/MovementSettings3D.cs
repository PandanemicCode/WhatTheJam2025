using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementSettings", menuName = "GameMovementToolkit/Movement Settings")]
public class MovementSettings : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Jumping")]
    public float jumpHeight = 2f;
    public float jumpCooldown = 0.2f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    public float terminalVelocity = -20f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;
}


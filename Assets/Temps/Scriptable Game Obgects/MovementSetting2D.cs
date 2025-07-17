using UnityEngine;

[CreateAssetMenu(fileName = "MovementSetting2D", menuName = "GameMovementToolkit/MovementSetting2D")]
public class MovementSetting2D : ScriptableObject
{
    // 2d Movemnt
    #region 2d
    [Header("Movement 2D")]
    public float moveSpeed_2d = 5f;
    public float acceleration_2d = 10f;
    public float deceleration_2D = 10f;

    [Header("Jumping 2D")]
    public float jumpHeight_2D = 2f;
    public float jumpCooldown_2D = 0.2f;

    [Header("Gravity 2D")]
    public float gravity_2d = -9.81f;
    public float terminalVelocity_2d = -20f;

    [Header("Rotation 2D")]
    public float rotationSpeed_2d = 10f;
    #endregion
}

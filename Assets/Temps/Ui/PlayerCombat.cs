using UnityEngine;

public enum PlayerRole { Hider, Seeker }

[RequireComponent(typeof(Collider))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1.0f;
    public LayerMask playerLayer;
    public Transform attackPoint;

    [Header("Role")]
    public PlayerRole currentRole = PlayerRole.Hider;

    private Animator animator;

    private void Awake()
    {
        // Look for Animator in child GameObjects
        animator = GetComponentInChildren<Animator>();

       // if (animator == null)
       //     Debug.LogError("Animator not found in children. Please assign an Animator component.");
    }

    /// <summary>
    /// Called when player presses attack button
    /// </summary>
    public void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    /// <summary>
    /// Called via animation event to actually apply the attack
    /// </summary>
    public void PerformAttack()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            playerLayer
        );

        foreach (var hit in hitPlayers)
        {
            if (hit.gameObject != gameObject)
            {
                var otherCombat = hit.GetComponent<PlayerCombat>();
                if (otherCombat != null)
                {
                    otherCombat.OnHit();
                }
            }
        }

        Debug.Log($"{gameObject.name} performed an attack.");
    }

    public void OnHit()
    {
        bool wasHider = currentRole == PlayerRole.Hider;
        SwitchRole();

        if (animator != null)
        {
            animator.SetTrigger("Hit");
            Debug.Log($"{gameObject.name} was hit and is now a {currentRole}");

            if (wasHider && currentRole == PlayerRole.Seeker)
            {
                GameManager.Instance.RespawnAsSeeker(this);
            }
        }
    }   


    private void SwitchRole()
    {
        currentRole = (currentRole == PlayerRole.Hider) ? PlayerRole.Seeker : PlayerRole.Hider;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}

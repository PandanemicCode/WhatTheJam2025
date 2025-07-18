using UnityEngine;

public enum PlayerRole { Hider, Seeker }

[RequireComponent(typeof(Collider))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1f;               // Attack range radius
    public LayerMask playerLayer;                 // Layer for detecting other players
    public Transform attackPoint;                 // Point from which attacks originate

    [Header("Role")]
    public PlayerRole currentRole = PlayerRole.Hider;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("Animator not found in children on " + gameObject.name);
    }

    /// <summary>
    /// Perform attack: plays animation and detects players hit within range.
    /// </summary>
    public void Attack()
{
    if (animator != null)
        animator.SetTrigger("Attack");

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
                // Pass this (attacker) to the one being hit
                otherCombat.OnHit(this);
            }
        }
    }
}


    /// <summary>
    /// Called when this player is hit; switches role and plays hit animation.
    /// </summary>
    public void OnHit(PlayerCombat attacker)
{
    // Swap both roles
    if (attacker != null)
    {
        PlayerRole tempRole = attacker.currentRole;
        attacker.currentRole = this.currentRole;
        this.currentRole = tempRole;

        // Optional: Trigger Hit animation
        if (attacker.animator != null)
            attacker.animator.SetTrigger("Hit");
    }

    if (animator != null)
        animator.SetTrigger("Hit");

    Debug.Log($"{gameObject.name} was hit and is now a {currentRole}");
    Debug.Log($"{attacker.gameObject.name} is now a {attacker.currentRole}");
}


    /// <summary>
    /// Switches player role between Hider and Seeker.
    /// </summary>
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

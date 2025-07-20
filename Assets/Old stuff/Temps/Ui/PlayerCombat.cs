using System.Collections;
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

        // Set role-based bool at start
        if (animator != null)
        {
            animator.SetBool("IsHunter", currentRole == PlayerRole.Seeker);
        }
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
                // Switch role of the other player
                otherCombat.OnHit();

                // Switch role of this attacker
                bool wasHider = currentRole == PlayerRole.Hider;
                SwitchRole();

                if (animator != null)
                {
                    animator.SetTrigger("Hit");
                    Debug.Log($"{gameObject.name} attacked and is now a {currentRole}");

                    if (wasHider && currentRole == PlayerRole.Seeker)
                    {
                        GameManager.Instance.RespawnAsSeeker(this);
                    }
                }
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
        animator.SetBool("IsHit", true);
        Debug.Log($"{gameObject.name} was hit and is now a {currentRole}");
        
        if (wasHider && currentRole == PlayerRole.Seeker)
        {
            GameManager.Instance.RespawnAsSeeker(this);
        }
        
        // Optionally, reset IsHit after some time
        StartCoroutine(ResetHitBool());
    }
}

private IEnumerator ResetHitBool()
{
    // Length of the hit animation, adjust as needed
    yield return new WaitForSeconds(0.5f);
    animator.SetBool("IsHit", false);
}


private void SwitchRole(float delay = 2f)
{
    StartCoroutine(DelayedRoleSwitch(delay));
}

private IEnumerator DelayedRoleSwitch(float delay)
{
    yield return new WaitForSeconds(delay);

    currentRole = (currentRole == PlayerRole.Hider) ? PlayerRole.Seeker : PlayerRole.Hider;

    if (animator != null)
        animator.SetBool("IsHunter", currentRole == PlayerRole.Seeker);
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

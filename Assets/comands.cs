using UnityEngine;

public class comands : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerCombat playerCombat;
    void Start()
    {
        playerCombat = GetComponentInParent<PlayerCombat>();
    }

    void Attack()
    {
        playerCombat.PerformAttack();
    }
}

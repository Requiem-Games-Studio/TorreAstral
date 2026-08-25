using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public EnemyBehavior enemy;
    public EnemyStats enemyStats;

    public void StopAttack()
    {
        enemy.attacking = false;
    }

    public void Impact()
    {
        enemyStats.CriticalImpact();
    }
    
}

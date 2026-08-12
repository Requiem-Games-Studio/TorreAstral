using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public EnemyBehavior enemy;

    public void StopAttack()
    {
        enemy.Attacking = false;
    }
}

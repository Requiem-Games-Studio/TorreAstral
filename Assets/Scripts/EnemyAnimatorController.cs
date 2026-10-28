using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    public EnemyBehavior enemy;
    public EnemyStats enemyStats;

    public CapsuleCollider2D capCollider;
    public Rigidbody2D body2D;

    public void StopAttack()
    {
        enemy.attacking = false;
    }

    public void Impact()
    {
        enemyStats.CriticalImpact();
    }

    public void DisableCollision()
    {
        capCollider.enabled = false;
        body2D.bodyType = RigidbodyType2D.Static;
    }

    public void EnableCollision()
    {
        capCollider.enabled = true;
        body2D.bodyType = RigidbodyType2D.Dynamic;
    }

}

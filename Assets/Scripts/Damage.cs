using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage, postureDamage;  
    public bool damageToPlayer, damageToEnemy;
    public bool heavyAttack;
    public GameObject enemyObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(damageToPlayer && collision.gameObject.CompareTag("Player"))
        {            
            PlayerStats stats = collision.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Damage(damage, postureDamage, enemyObject);
            
            }           
        }

        if (damageToEnemy && collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats stats = collision.GetComponent<EnemyStats>();
            if (stats != null)
            {               
                stats.Damage(damage, postureDamage, heavyAttack);
            }
        }
    }
}

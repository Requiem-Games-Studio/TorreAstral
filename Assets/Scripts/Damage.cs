using Fusion;
using UnityEngine;
using Photon.Realtime;
using static DamageSystem;

public class Damage : MonoBehaviour
{
    public float damage, postureDamage;  
    public bool damageToPlayer, damageToEnemy;
    public bool heavyAttack;
    public GameObject enemyObject;

    public NetworkObject attacker;
    [HideInInspector]
    public EnemyBehavior behavior;

    public DamageData damageData;

    private void Awake()
    {
        attacker = GetComponentInParent<NetworkObject>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(damageToPlayer && collision.gameObject.CompareTag("Player"))
        {            
            PlayerStats stats = collision.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.Damage(damageData, postureDamage, enemyObject, heavyAttack);
            
            }           
        }

        if (damageToEnemy && collision.gameObject.CompareTag("Enemy"))
        {

            EnemyStats stats = collision.GetComponent<EnemyStats>();
            behavior = collision.GetComponent<EnemyBehavior>();

            if (stats != null)
            {               
                stats.Damage(damage, postureDamage, heavyAttack);
            }

            if (behavior != null && attacker != null)
            {
                behavior.SetTarget(attacker);
                Debug.Log("Damage Set Targer");
            }
        }

        if (collision.gameObject.CompareTag("Object"))
        {
            ObjectStats stats = collision.GetComponent<ObjectStats>();
            if(stats != null)
            {
                collision.SendMessage("Damage", damage);
            }
        }
    }
}

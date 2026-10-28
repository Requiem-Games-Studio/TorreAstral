using Fusion;
using UnityEngine;
using static DamageSystem;

public class ObjectStats : NetworkBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [Networked] public float currentHealth { get; set; }
    
    private bool isAlive = true;
    public GameObject particle;
    public FlashSprite flashSprite;

    public ArmorResistance resistance;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(DamageData damageData)
    {
        if (!isAlive) return;

        float defense = resistance.GetResistance(damageData.type);
        float finalDamage = DamageCalculator.CalculateDamage(damageData.damage, defense);


        currentHealth -= finalDamage;

        flashSprite.Flash();
        Instantiate(particle, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
            GameFeelManager.Instance.DoImpactToKill();
            Destroy();
            return;
        }
        GameFeelManager.Instance.DoImpact();
    }

    void Destroy()
    {
        isAlive = false;
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(GetComponent<NetworkObject>());
        }
    }
}

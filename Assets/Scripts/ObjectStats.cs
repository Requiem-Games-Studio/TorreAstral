using Fusion;
using UnityEngine;

public class ObjectStats : NetworkBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [Networked] public float currentHealth { get; set; }
    
    private bool isAlive = true;
    public GameObject particle;
    public FlashSprite flashSprite;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damage)
    {
        if (!isAlive) return;


        currentHealth -= damage;

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

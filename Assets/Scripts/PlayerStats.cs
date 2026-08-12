using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : NetworkBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    [Networked] public float currentHealth { get; set; }

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    [Networked] public float currentPosture { get; set; }
    public float postureRecoveryRate = 10f; // Por segundo
    public float postureBreakTime = 2f; // Tiempo que dura tambaleado
    private bool isStaggered = false;

    [Header("Parry Settings")]
    public bool isBlocking = false;
    public bool isPerfectBlock = false;

    public Animator animator;

    public Slider healthBar;
    public Slider postureBar;

    public GameObject blockParticle,parryParticle,bloodParticle;

    public FlashSprite flashSprite;

    void Start()
    {
        currentHealth = maxHealth;
        currentPosture = maxPosture;
    }

    void Update()
    {
        RecoverPosture();
    }

    // Recibir Daño
    public void Damage(float damage, float postureDamage, GameObject enemyTransform)
    {
        if (isStaggered) return;

        if (isBlocking)
        {
            if (isPerfectBlock)
            {
                Debug.Log("Parry perfecto: No se recibe daño y se rompe la postura del enemigo");
                animator.Play("Parry");
                GameFeelManager.Instance.DoParryImpact();
                Instantiate(parryParticle,gameObject.transform);
                // Aquí deberías llamar algo como enemy.ReducePosture()
                EnemyStats enemy = enemyTransform.GetComponent<EnemyStats>();
                if (enemy != null)
                {
                    enemy.ReducePosture(30f); // Por ejemplo
                    Debug.Log("Reduce postura Enemy");
                }
                return;
            }
            else
            {
                currentPosture -= postureDamage;
                UpdatePostureBar();
                CheckPostureBreak();
                Debug.Log("Bloqueo normal: daño a la resistencia");
                Instantiate(blockParticle,gameObject.transform);
                return;
            }
        }

        // Daño normal
        Debug.Log("Daño al juagdor");
        currentHealth -= damage;
        currentPosture -= postureDamage;
        UpdatePostureBar();
        CheckPostureBreak();
        UpdateHealthBar();
        flashSprite.Flash();
        Instantiate(bloodParticle, transform.position, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Die();
            GameFeelManager.Instance.DoImpactToKill();
            return;
        }
        GameFeelManager.Instance.DoImpactPlayer();
    }

    // Recuperación de resistencia si no está siendo golpeado
    void RecoverPosture()
    {
        if (!isBlocking && !isStaggered && currentPosture < maxPosture)
        {
            currentPosture += postureRecoveryRate * Time.deltaTime;
            UpdatePostureBar();
        }
    }

    // Revisar si se rompe la resistencia
    void CheckPostureBreak()
    {
        if (currentPosture <= 0 && !isStaggered)
        {
            StartCoroutine(Stagger());
        }
    }

    // Tambalear al jugador
    System.Collections.IEnumerator Stagger()
    {
        isStaggered = true;
        animator.Play("Stagger"); // Asegúrate de tener esta animación
        Debug.Log("¡Jugador tambaleado!");
        yield return new WaitForSeconds(postureBreakTime);
        currentPosture = maxPosture * 0.5f; // Empieza a la mitad
        isStaggered = false;
    }

    // Muerte
    void Die()
    {
        Debug.Log("Jugador ha muerto.");
        //animator.SetTrigger("die");
        // Aquí puedes desactivar controles, mostrar UI de muerte, etc.
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
    private void UpdatePostureBar()
    {
        currentPosture = Mathf.Min(currentPosture, maxPosture);
        postureBar.value = currentPosture / maxPosture;
    }

    // Llamado desde la aniamcion de bloqueo
    public void SetBlock(bool isPerfect)
    {
        isBlocking = true;
        isPerfectBlock = isPerfect;
    }

    public void StopBlock()
    {
        isBlocking = false;
        isPerfectBlock = false;
    }
}

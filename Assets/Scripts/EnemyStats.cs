using UnityEngine;
using UnityEngine.UI;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    public float currentPosture;
    public float postureRecoveryRate = 8f; // Recuperación por segundo
    public float postureBreakTime = 2f;
    public bool isStaggered = false;

    public Animator animator;
    private bool isAlive = true;
    public EnemyBehavior enemyBehavior;

    public Slider healthBar;
    public Slider postureBar;
    public GameObject canvas;

    void Start()
    {
        currentHealth = maxHealth;
        currentPosture = maxPosture;
    }

    void Update()
    {
        RecoverPosture();
    }

    // Recibir daño
    public void Damage(float damage, float postureDamage, bool isHeavyAttack = false)
    {
        if (!isAlive) return;

        if (isStaggered)
        {
            animator.Play("HDamage");
            currentHealth -= damage * 2;
            UpdateHealthBar();
        }
        else
        {
            animator.Play("Damage");
            currentHealth -= damage;
            UpdateHealthBar();
            currentPosture -= postureDamage * (isHeavyAttack ? 2f : 1f);
            CheckPostureBreak();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void CriticalDamage(float damage)
    {
        currentHealth -= damage * 3;
        UpdateHealthBar();
        animator.Play("CDamage");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    // Reducir solo postura (por ejemplo, al recibir un parry)
    public void ReducePosture(float amount)
    {
        if (!isAlive || isStaggered) return;

        currentPosture -= amount;
        UpdatePostureBar();
        CheckPostureBreak();
    }

    // Recuperación de postura
    void RecoverPosture()
    {
        if (!isStaggered && currentPosture < maxPosture)
        {
            currentPosture += postureRecoveryRate * Time.deltaTime;
            currentPosture = Mathf.Min(currentPosture, maxPosture);
            UpdatePostureBar();
        }
    }
    private void UpdatePostureBar()
    {
        currentPosture = Mathf.Min(currentPosture, maxPosture);
        postureBar.value = currentPosture / maxPosture;
    }

    // Revisar si se rompe la postura
    void CheckPostureBreak()
    {
        if (currentPosture <= 0 && !isStaggered)
        {
            StartCoroutine(Stagger());
        }
    }

    // Tambaleo
    System.Collections.IEnumerator Stagger()
    {
        isStaggered = true;
        animator.Play("Stagger"); // Agrega esta animación
        Debug.Log("¡Enemigo tambaleado!");
        yield return new WaitForSeconds(postureBreakTime);
        currentPosture = maxPosture * 0.5f;
        isStaggered = false;
    }

    // Muerte
    void Die()
    {
        isAlive = false;
        enemyBehavior.DeadEvent();
        animator.SetBool("dead", true);
        canvas.SetActive(false);
        Debug.Log("Enemigo muerto.");
        // Aquí puedes desactivar IA, colisiones, etc.
    }
}

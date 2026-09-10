using Fusion;
using UnityEngine;
using UnityEngine.UI;
using static DamageSystem;

public class PlayerStats : NetworkBehaviour
{
    public PlayerControler controler;

    [SerializeField]
    public ArmorManager armorManager;

    [Header("Health")]
    public float maxHealth = 100f;
    [Networked] public float CurrentHealth { get; set; }

    [Header("Recovery")]
    public float maxRecovery = 100f;
    [Networked] public float CurrentRecovery { get; set; }
    [Networked] public float RecoveryTime { get; set; }

    [Header("Posture / Resistance")]
    public float maxPosture = 100f;
    [Networked] public float CurrentPosture { get; set; }
    [Networked] public float PoiseDefence { get; set; }
    public float postureRecoveryRate = 10f; // Por segundo
    public float postureBreakTime = 2f; // Tiempo que dura tambaleado
    private bool isStaggered = false;

    public Rigidbody2D rb;

    public float knockbackForce = 12f;
    float knockbackUpForce = 2f;

    [Header("Parry Settings")]
    public bool isBlocking = false;
    public bool isPerfectBlock = false;

    public Animator animator;

    public Slider healthBar;
    public Slider postureBar;
    public Slider recoveryBar;

    public GameObject blockParticle,parryParticle,bloodParticle;

    public FlashSprite flashSprite;

    void Start()
    {
        CurrentHealth = maxHealth;
        CurrentPosture = maxPosture;
        CurrentRecovery = maxRecovery;
    }

    void Update()
    {
        RecoverPosture();
        Recovery();
    }

    // Recibir Daño
    public void Damage(DamageData damageData, float postureDamage, GameObject enemyTransform, bool heavy)
    {
        if (controler.IsDodging) return;

        float dirX = transform.position.x < enemyTransform.transform.position.x ? -1f : 1f;

        if (isBlocking)
        {
            if (isPerfectBlock)
            {
                if (heavy)
                {
                    controler.PlayAnimationOnAll("Parry2");
                    rb.AddForce(new Vector2(dirX * (knockbackForce * 1.4f), knockbackUpForce), ForceMode2D.Impulse);
                }
                else
                {
                    //animator.Play("Parry");
                    controler.PlayAnimationOnAll("Parry");
                }

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
            else if(!heavy)
            {
                CurrentPosture -= postureDamage;
                UpdatePostureBar();
                CheckPostureBreak();
                Debug.Log("Bloqueo normal: daño a la resistencia");
                Instantiate(blockParticle,gameObject.transform);
                return;
            }
        }

        // Daño normal
        Debug.Log("Daño al juagdor");
        flashSprite.Flash();
        Instantiate(bloodParticle, transform.position, Quaternion.identity);
        if (controler.IsBeating)
        {            
            if(!animator.GetBool("isInteracting")) controler.PlayAnimationOnAll("DamageC");
        }
        else
        {
            if (postureDamage > PoiseDefence)
            {
                controler.PlayAnimationOnAll("Damage");
                rb.AddForce(new Vector2(dirX * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);
            }

            ///Defensa de Armadura
            float defense = armorManager.GetDefense(damageData.type);

            float finalDamage =
                DamageCalculator.CalculateDamage(
                    damageData.damage,
                    defense
                );

            Debug.Log(
                $"Daño: {damageData.damage} | " +
                $"Tipo: {damageData.type} | " +
                $"Defensa: {defense} | " +
                $"Final: {finalDamage}"
            );

            CurrentHealth -= finalDamage;
            CurrentPosture -= postureDamage;
            UpdatePostureBar();
            CheckPostureBreak();
            UpdateHealthBar();

            if (CurrentHealth <= 0)
            {
                StartCoroutine(Beating());
                GameFeelManager.Instance.DoImpactToKill();
                return;
            }
        }     
        GameFeelManager.Instance.DoImpactPlayer();
    }

    // Recuperación de resistencia si no está siendo golpeado
    void RecoverPosture()
    {
        if (!isBlocking && !isStaggered && CurrentPosture < maxPosture)
        {
            CurrentPosture += postureRecoveryRate * Time.deltaTime;
            UpdatePostureBar();
        }
    }

    void Recovery()
    {
        if (CurrentRecovery < maxRecovery)
        {
            CurrentRecovery += 3 * Time.deltaTime;
            UpdateRecoveryBar();
            if(CurrentRecovery >= maxRecovery)
            {
                controler.EndCrawlCount++;
                controler.IsBeating = false;
            }
        }
    }

    // Revisar si se rompe la resistencia
    void CheckPostureBreak()
    {
        if (CurrentPosture <= 0 && !isStaggered)
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
        CurrentPosture = maxPosture * 0.5f; // Empieza a la mitad
        isStaggered = false;
    }

    // Derrotado

    // Timepo de recuperacion
    System.Collections.IEnumerator Beating()
    {
        Debug.Log("Jugador ha Caido");
        controler.IsBeating = true;
        CurrentRecovery = 0;
        yield return new WaitForSeconds(RecoveryTime);
        controler.CrawlCount ++;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)CurrentHealth / maxHealth;
        }
    }
    private void UpdatePostureBar()
    {
        CurrentPosture = Mathf.Min(CurrentPosture, maxPosture);
        postureBar.value = CurrentPosture / maxPosture;
    }
    private void UpdateRecoveryBar()
    {
        CurrentRecovery = Mathf.Min(CurrentRecovery, maxRecovery);
        recoveryBar.value = CurrentRecovery / maxRecovery;
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

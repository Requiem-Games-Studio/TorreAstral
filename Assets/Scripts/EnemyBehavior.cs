using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float patrolRange = 3f;
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;
    public Transform player;


    private bool movingRight = true;
    private Rigidbody2D rb;
    public GameObject spriteEnemy;
    private Animator animator;
    private Vector2 startPos;
    private float lastAttackTime = 0;
    public bool dead;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        animator = spriteEnemy.GetComponent<Animator>();
        startPos = transform.position;
    }

    void Update()
    {
        if(!dead)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                AttackPlayer();
            }
            else if (distance <= patrolRange)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);

        float moveDirection = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(moveDirection * patrolSpeed, rb.linearVelocity.y);

        if (movingRight && transform.position.x >= startPos.x + patrolRange)
            movingRight = false;
        else if (!movingRight && transform.position.x <= startPos.x - patrolRange)
            movingRight = true;

        Flip(moveDirection);
    }
    void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        float direction = player.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.linearVelocity.y);

        Flip(direction);
    }
    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        if (Time.time - lastAttackTime > attackCooldown)
        {
            animator.SetTrigger("attack");
            lastAttackTime = Time.time;
        }

        Flip(player.position.x - transform.position.x);
    }

    void Flip(float direction)
    {
        if (direction != 0)
        {
            Vector3 scale = spriteEnemy.transform.localScale;
            scale.x = Mathf.Sign(direction) * Mathf.Abs(scale.x);
            spriteEnemy.transform.localScale = scale;
        }
    }

    public void DeadEvent()
    {       
        dead = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("BackEnemy");
    }
}

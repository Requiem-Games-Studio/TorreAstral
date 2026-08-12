using UnityEngine;
using System;
using Fusion;

public class EnemyBehavior : NetworkBehaviour
{
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float patrolRange = 3f;
    public float attackRange = 1f;
    public float attackCooldown = 1.5f;

    [Networked] public NetworkObject Target { get; set; }


    private Rigidbody2D rb;
    public GameObject spriteEnemy;
    private Animator animator;
    private Vector2 startPos;
    private float lastAttackTime = 0;
    private bool MovingRight = true;
    public bool Attacking;


    [Networked] public NetworkBool Dead { get; set; }
    [Networked] public NetworkBool IsWalking { get; set; }
    [Networked] public NetworkBool FacingRight { get; set; }


    public override void Spawned()
    {
        Debug.Log("Spawned Enemy");
        transform.parent = null;
        rb = GetComponent<Rigidbody2D>();
        animator = spriteEnemy.GetComponent<Animator>();
        startPos = transform.position;
    }

    public void SetTarget(NetworkObject attacker)
    {
        if (!HasStateAuthority)
            return;
        Debug.Log("Set Target");
        Target = attacker;
    }

    public override void FixedUpdateNetwork()
    {
        Debug.Log("Enemy Tick");
        if (!HasStateAuthority)
            return;

        if (Dead || Target == null)
            return;

        float distance = Vector2.Distance(transform.position, Target.transform.position);

        if (distance <= attackRange)
        { 
            AttackPlayer();
        }
        else if (distance <= patrolRange && !Attacking)
        {
            ChasePlayer();
        }
        else
        {
            if (!Attacking)
            {
                Patrol();
            }
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);

        float moveDirection = MovingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(moveDirection * patrolSpeed, rb.linearVelocity.y);

        if (MovingRight && transform.position.x >= startPos.x + patrolRange)
            MovingRight = false;
        else if (!MovingRight && transform.position.x <= startPos.x - patrolRange)
            MovingRight = true;

        Flip(moveDirection);
    }
    void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        float direction = Target.transform.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.linearVelocity.y);

        Flip(direction);
    }
    void AttackPlayer()
    {
        Attacking = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);

        if (Runner.SimulationTime - lastAttackTime > attackCooldown)
        {
            animator.SetTrigger("attack");
            lastAttackTime = Runner.SimulationTime;
        }

        //Flip(player.position.x - transform.position.x);
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
        Dead = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("BackEnemy");
    }
}

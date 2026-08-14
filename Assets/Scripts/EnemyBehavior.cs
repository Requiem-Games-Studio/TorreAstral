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
    public bool attacking;

    [Networked] private float Direction { get; set; }
    [Networked] public NetworkBool Dead { get; set; }
    [Networked] public NetworkBool FacingRight { get; set; }

    [Networked] public NetworkBool IsWalking { get; set; }
    private bool _lastIsWalking;

    [Networked] public int IsAttackingCount { get; set; }

    private ChangeDetector _changeDetector;


    public override void Spawned()
    {
        Debug.Log("Spawned Enemy");
        transform.parent = null;
        rb = GetComponent<Rigidbody2D>();
        animator = spriteEnemy.GetComponent<Animator>();
        startPos = transform.position;
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
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
        if (!HasStateAuthority)
            return;

        if (Dead || Target == null)
            return;

        float distance = Vector2.Distance(transform.position, Target.transform.position);

        if (distance <= attackRange)
        { 
            AttackPlayer();          
        }
        else if (distance <= patrolRange && !attacking)
        {
            ChasePlayer();            
        }
        else
        {
            if (!attacking)
            {
                Patrol();                
            }
        }
    }

    public override void Render()
    {
        Flip(Direction);

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(IsAttackingCount))
            {
                animator.Play("Attack");
            }
        }

        if (IsWalking != _lastIsWalking)
        {
            animator.SetBool("isWalking", IsWalking);
            _lastIsWalking = IsWalking;
        }
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

    void Patrol()
    {
        float moveDirection = MovingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(moveDirection * patrolSpeed, rb.linearVelocity.y);

        if (MovingRight && transform.position.x >= startPos.x + patrolRange)
            MovingRight = false;
        else if (!MovingRight && transform.position.x <= startPos.x - patrolRange)
            MovingRight = true;

        Direction = moveDirection;
        IsWalking = true;
    }
    void ChasePlayer()
    {        
        float direction = Target.transform.position.x - transform.position.x;
        rb.linearVelocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.linearVelocity.y);
        IsWalking = true;
        Direction = direction;
    }
    void AttackPlayer()
    {
        attacking = true;
        rb.linearVelocity = Vector2.zero;
        IsWalking = false;

        if (Runner.SimulationTime - lastAttackTime > attackCooldown)
        {
            IsAttackingCount++;
            lastAttackTime = Runner.SimulationTime;
        }
    }
   
    public void DeadEvent()
    {       
        Dead = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        gameObject.layer = LayerMask.NameToLayer("BackEnemy");
    }
}

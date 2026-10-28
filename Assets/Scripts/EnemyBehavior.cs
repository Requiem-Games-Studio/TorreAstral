using UnityEngine;
using System;
using Fusion;
using UnityEngine.EventSystems;

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
    public string[] attackAnim;
    private Vector2 startPos;
    private float lastAttackTime = 0;
    private bool MovingRight = true;
    public bool attacking,patrol;

    [Networked] private float Direction { get; set; }
    [Networked] public NetworkBool Dead { get; set; }

    [Networked] public NetworkBool IsInteracting { get; set; }
    [Networked] public NetworkBool FacingRight { get; set; }

    [Networked] public NetworkBool IsWalking { get; set; }
    private bool _lastIsWalking;

    [Networked] public int IsAttackingCount { get; set; }

    private ChangeDetector _changeDetector;

   PlayerControler _playerControler;

    [SerializeField] Transform groundCheck;
    [SerializeField] float wallDistance = 0.5f;
    [SerializeField] float groundDistance = 0.8f;
    [SerializeField] LayerMask groundLayer;


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
        _playerControler = attacker.GetComponent<PlayerControler>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || Dead)
            return;


        if(Target != null && !IsInteracting)
        {
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
                    Debug.Log("Target Perdido " + distance + "<=" + patrolRange);
                    IsWalking = false;
                    Target = null;
                }
            }
        }

        if (patrol && Target == null && !IsInteracting)
        {
            Patrol();
        }

    }

    public override void Render()
    {
        Flip(Direction);

        IsInteracting = animator.GetBool("isInteracting");

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(IsAttackingCount))
            {
                int randomAttack = UnityEngine.Random.Range(0, attackAnim.Length);

                animator.Play(attackAnim[randomAttack]);
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

        DetectGroundAndWall(moveDirection);

        if (MovingRight && transform.position.x >= startPos.x + patrolRange)
            MovingRight = false;
        else if (!MovingRight && transform.position.x <= startPos.x - patrolRange)
            MovingRight = true;

        Direction = moveDirection;
        IsWalking = true;
    }
    void DetectGroundAndWall(float moveDirection)
    {
        // Detectar muro al frente
        Vector2 direction = MovingRight ? Vector2.right : Vector2.left;

        bool wall = Physics2D.Raycast(
            transform.position,
            direction,
            wallDistance,
            groundLayer
        );

        // Detectar si hay piso adelante
        Vector2 groundOrigin = groundCheck.position;
        groundOrigin.x += moveDirection * groundDistance;

        bool groundAhead = Physics2D.Raycast(
            groundOrigin,
            Vector2.down,
            groundDistance,
            groundLayer
        );

        // Cambiar dirección si hay muro o no hay piso
        if (wall || !groundAhead)
        {         
            MovingRight = !MovingRight;
        }
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
        if (animator.GetBool("isInteracting"))
        {
            return;
        }

        if(_playerControler != null && _playerControler.IsBeating)
        {
            _playerControler = null;
            Target = null;
            return;
        }

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

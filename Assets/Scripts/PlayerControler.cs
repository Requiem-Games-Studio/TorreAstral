using UnityEngine;
using System.Collections;
using System;
using Fusion;
using Photon.Realtime;

public class PlayerControler : NetworkBehaviour
{
    private Rigidbody2D rb;
    public WeaponManager weaponManager;
    public Animator animator,animatorP,animatorC,animatorB;
    public SpriteRenderer spriteRenderer,spritePiernas,spriteCabeza,spriteBrazo;
    public Transform espadaPivot; // arrastra aquí tu EspadaPivot en el inspector

    public Transform groundCheck; // Punto en los pies para detectar el suelo
    public LayerMask groundLayer; // Capa del suelo

    [Header("Velocidades")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private float shiftPressTime;

    public float dodgeSpeed = 5f;  // Velocidad del dodge
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;

    [Header("Salto")]
    public float jumpForce = 7f;
    public float maxJumpTime = 0.2f; // Tiempo máximo de salto variable
    public float coyoteTime = 0.15f; // Tiempo extra después de dejar el suelo
    public float normalGravity;

    bool isGrounded;
    private bool canDodge = true;
    private bool isJumping, isInteracting;
    private float jumpTimeCounter;
    private float coyoteTimeCounter;

    bool isCrouching;

    //Caida
    public float fallThreshold = 0.5f; // Tiempo mínimo de caída para animación de aterrizaje
    private float fallStartTime;
    private bool isFalling = false;

    [Header("Combate")]
    //Combate
    public int comboStep = 0;
    public float comboTimer = 0f;
    public float comboDelay = 0.5f; // tiempo máximo entre ataques
    private bool isAttacking = false;
    private bool isBlocking;

    public float rayDistance = 1.5f;
    public LayerMask enemyLayer;
    public Transform rayOrigin;
    public float criticalDamage;

    public CameraFollow cameraFollow;
    public ChunkManagerByName chunkManager;

    private NetworkButtons previousButtons;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();

        if (HasInputAuthority)
        {
            // Este es mi jugador local
            cameraFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>(); 
            cameraFollow.player = this.gameObject.transform;
            cameraFollow.StartCamera();
            chunkManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<ChunkManagerByName>();
            chunkManager.player = this.gameObject.transform;
            chunkManager.StartChunkManager();
        }
    }


    public override void FixedUpdateNetwork()
    {       
        if (!GetInput(out NetworkInputData input))
            return;

        SetAnimatorParameters();

        // Attack Player
        if (input.buttons.WasPressed(previousButtons, InputButtons.Attack))
        {
            Debug.Log("FUSION Ataque presionado");
            AttackPlayer();
        }
        // Reducir tiempo de combo
        if (isAttacking)
        {
            comboTimer -= Runner.DeltaTime;
            if (comboTimer <= 0)
            {
                comboStep = 0;
                comboTimer = comboDelay;
                isAttacking = false;
            }
        }
      
        // Block Down and Up
        if (input.buttons.WasPressed(previousButtons, InputButtons.Block))
        {
            if (!isAttacking && !isInteracting)
            {
                animator.Play("StartBlock");
                animator.SetBool("blocking", true);
                if (!animatorP.GetBool("Walk") && !animatorP.GetBool("Run"))
                {
                    animatorP.Play("StartBlock");
                    animatorP.SetBool("blocking", true);
                }
                animatorC.Play("StartBlock");
                animatorC.SetBool("blocking", true);
                animatorB.Play("StartBlock");
                animatorB.SetBool("blocking", true);
                weaponManager.anim.Play("StartBlock");
                weaponManager.anim.SetBool("blocking", true);
            }
        }
        if (input.buttons.WasReleased(previousButtons, InputButtons.Block))
        {
            animator.SetBool("blocking", false);
            animatorP.SetBool("blocking", false);
            animatorC.SetBool("blocking", false);
            animatorB.SetBool("blocking", false);
            weaponManager.anim.SetBool("blocking", false);
        }

        // Salto
        if (input.buttons.WasPressed(previousButtons, InputButtons.Jump) && coyoteTimeCounter > 0f)
        {
            animator.Play("Jump");
            animatorP.Play("Jump");
            animatorC.Play("Jump");
            animatorB.Play("Jump");
            weaponManager.anim.Play("Jump");
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if (input.buttons.IsSet(InputButtons.Jump) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.gravityScale = 0.1f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Runner.DeltaTime;
            }
            else
            {
                isJumping = false;
                rb.gravityScale = normalGravity;
            }
        }
        if (input.buttons.WasReleased(previousButtons, InputButtons.Jump))
        {
            isJumping = false;
            rb.gravityScale = normalGravity;
        }
        // Activar la animación de salto
        animator.SetBool("isJumping", isJumping);
        animatorP.SetBool("isJumping", isJumping);
        animatorC.SetBool("isJumping", isJumping);
        animatorB.SetBool("isJumping", isJumping);
        weaponManager.anim.SetBool("isJumping", isJumping);
        // **Coyote Time**
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Runner.DeltaTime; ;
        }

        //Movimiento y vista del jugador solo si no esta iteractuando
        if (!isInteracting)
        {
            float moveInput = input.movement.x;
            // --- Detectar si corre ---
            bool isRunning = input.buttons.IsSet(InputButtons.Run);


            // --- Animaciones Walk y Run ---
            animator.SetBool("Walk", moveInput != 0 && !isRunning);
            animatorP.SetBool("Walk", moveInput != 0 && !isRunning);
            animatorC.SetBool("Walk", moveInput != 0 && !isRunning);
            animatorB.SetBool("Walk", moveInput != 0 && !isRunning);
            weaponManager.anim.SetBool("Walk", moveInput != 0 && !isRunning);


            animator.SetBool("Run", moveInput != 0 && isRunning);
            animatorP.SetBool("Run", moveInput != 0 && isRunning);
            animatorC.SetBool("Run", moveInput != 0 && isRunning);
            animatorB.SetBool("Run", moveInput != 0 && isRunning);
            weaponManager.anim.SetBool("Run", moveInput != 0 && isRunning);

            // Cambiar velocidad según estado (caminar/correr)
            float speed = isRunning ? runSpeed : walkSpeed;
            if (!isCrouching && !isInteracting)
            {
                rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
            }
            
            // **Rotar sprite según dirección**
            if (moveInput != 0)
            {
                spriteRenderer.flipX = moveInput < 0;
                spritePiernas.flipX = moveInput < 0;
                spriteCabeza.flipX = moveInput < 0;
                spriteBrazo.flipX = moveInput < 0;
                

                // **Flip del pivote de la espada**
                Vector3 scale = espadaPivot.localScale;
                scale.x = moveInput < 0 ? -1 : 1;
                espadaPivot.localScale = scale;
            }
        }

        //Agacharse y caida
        if (isGrounded)
        {
            if (input.buttons.WasPressed(previousButtons, InputButtons.Run))
            {
                shiftPressTime = Runner.DeltaTime; // Guardar el tiempo en que se presionó
            }
            if (input.buttons.WasReleased(previousButtons, InputButtons.Run) && canDodge)
            {
                float heldTime = Runner.DeltaTime - shiftPressTime;
                if (heldTime <= 0.2f) // Si se soltó rápido
                {
                    StartCoroutine(Dodge());
                }
            }
            
            float verticalInput = input.movement.y;
            // Cuando se presiona la flecha abajo (empieza a agacharse)
            if (verticalInput < 0 && !isCrouching)
            {
                isInteracting = true;
                isCrouching = true;
                rb.linearVelocityX = 0;
                animator.Play("StarCrouch");
                animator.SetBool("Crouch", true);   // Mantiene pose de agachado
                animatorP.Play("StarCrouch");
                animatorP.SetBool("Crouch", true);
                animatorC.Play("StarCrouch");
                animatorC.SetBool("Crouch", true);
                animatorB.Play("StarCrouch");
                animatorB.SetBool("Crouch", true);
                weaponManager.anim.Play("StarCrouch");
                weaponManager.anim.SetBool("Crouch", true);
            }
            // Cuando se suelta la flecha abajo (empieza a levantarse)
            if (verticalInput >= 0 && isCrouching)
            {
                isCrouching = false;
                animator.SetBool("Crouch", false);  // Termina pose de agachado
                animatorP.SetBool("Crouch", false);
                animatorC.SetBool("Crouch", false);
                animatorB.SetBool("Crouch", false);
                weaponManager.anim.SetBool("Crouch", false);
            }
        }

        // Detectar inicio de caída
        if (!isGrounded && rb.linearVelocity.y < 0 && !isFalling)
        {
            isFalling = true;
            fallStartTime = Runner.DeltaTime; ; // Guardar cuando empezó a caer
        }

        // Detectar aterrizaje
        if (isGrounded && isFalling)
        {
            isFalling = false;
            float fallDuration = Runner.DeltaTime - fallStartTime;

            if (fallDuration >= fallThreshold)
            {
                rb.linearVelocityX = 0;
                animatorP.Play("Land"); // Animación de aterrizaje
                animator.Play("Land");
                animatorC.Play("Land");
                animatorB.Play("Land");
                weaponManager.anim.Play("Land");
            }
            else
            {
                animatorP.Play("idle"); // Vuelve a idle normal
                animator.Play("idle");
                animatorC.Play("idle");
                animatorB.Play("idle");
                weaponManager.anim.Play("idle");
            }
        }

        previousButtons = input.buttons;
    }

    void SetAnimatorParameters()
    {
        isInteracting = animator.GetBool("isInteracting");
        isBlocking = animator.GetBool("blocking");
        animator.SetBool("isAttacking", isAttacking);
        animatorP.SetBool("isAttacking", isAttacking);
        animatorC.SetBool("isAttacking", isAttacking);
        animatorB.SetBool("isAttacking", isAttacking);
        weaponManager.anim.SetBool("isAttacking", isAttacking);
    }

    void AttackPlayer()
    {
        // Determina dirección basada en flipX
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, direction, rayDistance, enemyLayer);
        Debug.DrawRay(rayOrigin.position, direction * rayDistance, Color.red, 1f);

        if (hit.collider != null)
        {
            EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();

            if (enemy != null)
            {
                if (enemy.isStaggered)
                {
                    Debug.Log("¡Ataque crítico!");
                    animator.Play("Critical");
                    animatorP.Play("Critical");
                    animatorC.Play("Critical");
                    animatorB.Play("Critical");
                    weaponManager.anim.Play("Critical");
                    enemy.CriticalDamage(criticalDamage);
                    return;
                }
            }
        }
        //Ataque normal
        if (!isAttacking && !isInteracting)
        {
            Debug.Log("Attack button");
            isAttacking = true;
            comboStep = 1;
            comboTimer = comboDelay;
            animator.Play("Attack");
            if (!animatorP.GetBool("Walk") && !animatorP.GetBool("Run"))
            {
                animatorP.Play("Attack");
            }
            animatorC.Play("Attack");
            animatorB.Play("Attack");
            weaponManager.anim.Play("Attack");
        }
        else if (comboStep == 1 && comboTimer > 0)
        {
            Debug.Log("Attack1 button");
            comboStep = 2;
            comboTimer = comboDelay;
            isAttacking = true;
            //animator.Play("Attack1");
        }
        else if (comboStep == 2 && comboTimer > 0)
        {
            Debug.Log("Attack2 button");
            comboStep = 3;
            comboTimer = comboDelay;
            //animator.Play("Attack2");
            isAttacking = true;
            animator.SetBool("isInteracting", true);
        }
    }

    IEnumerator Dodge()
    {
        canDodge = false;
        Debug.Log("Dodgeeee");

        // Reproducir la animación
        animator.SetBool("isInteracting", true);
        if(!isCrouching)
        {
            animator.Play("Dodge");
            animatorP.Play("Dodge");
            animatorC.Play("Dodge");
            animatorB.Play("Dodge");
            weaponManager.anim.Play("Dodge");
        }
        else
        {
            animator.Play("CrouchSlide");
            animatorP.Play("CrouchSlide");
            animatorC.Play("CrouchSlide");
            animatorB.Play("CrouchSlide");
            weaponManager.anim.Play("CrouchSlide");
        }

        // Desactivar colisión con enemigos
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        // Determinar dirección del dodge
        float dodgeDirection = spriteRenderer.flipX ? -1 : 1;

        // Desactivar gravedad para evitar caída
        //rb.gravityScale = 0;

        // Aplicar movimiento durante el dodge
        float startTime = Time.time;
        while (Time.time < startTime + dodgeDuration)
        {
            rb.linearVelocity = new Vector2(dodgeDirection * dodgeSpeed, 0); // Velocidad en Y se mantiene en 0
            yield return null;
        }

        rb.linearVelocity = Vector2.zero; // Detener el movimiento después del dodge

        // Reactivar la gravedad
        rb.gravityScale = normalGravity;

        // Reactivar colisión con enemigos después del dodge
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

        // Esperar el cooldown antes de permitir otro dodge
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }   

    public void StopVelocity()
    {
        rb.linearVelocity = Vector2.zero;
        //rb.gravityScale = 0;
    }

    private void FixedUpdate()
    {
        if (!animator.GetBool("dodge"))
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
            animator.SetBool("Ground", isGrounded);
            animatorP.SetBool("Ground", isGrounded);
            animatorC.SetBool("Ground", isGrounded);
            animatorB.SetBool("Ground", isGrounded);
            weaponManager.anim.SetBool("Ground", isGrounded);
        }
    }
}

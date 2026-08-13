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
    private float shiftHeldTime;
    private bool _lastIsCrouching;

    public float dodgeSpeed = 5f;  // Velocidad del dodge
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;

    [Header("Salto")]
    public float jumpForce = 7f;
    public float maxJumpTime = 0.2f; // Tiempo máximo de salto variable
    public float coyoteTime = 0.15f; // Tiempo extra después de dejar el suelo
    public float normalGravity;


    private float jumpTimeCounter;
    private float coyoteTimeCounter;

    //Caida
    bool isGrounded;
    public float fallThreshold = 0.5f; // Tiempo mínimo de caída para animación de aterrizaje
    private float fallStartTime;


    [Header("Combate")]
    //Combate
    [Networked] public int ComboStep { get; set; }
    [Networked] public NetworkBool IsAttacking { get; set; }
    public float comboTimer = 0f;
    public float comboDelay = 0.5f; // tiempo máximo entre ataques
    private bool lastIsAttacking = false;

    public float rayDistance = 1.5f;
    public LayerMask enemyLayer;
    public Transform rayOrigin;
    public float criticalDamage;

    public GameObject cameraPlayer,canvas;
    [HideInInspector]
    public CameraFollow cameraFollow;
    public ChunkManagerByName chunkManager;

    private NetworkButtons previousButtons;

    [Networked] public NetworkBool IsFacingLeft { get; set; }
    [Networked] public NetworkBool IsJumping { get; set; }
    [Networked] public NetworkBool IsInteracting { get; set; }
    [Networked] public NetworkBool IsCrouching { get; set; }
    [Networked] public NetworkBool IsFalling { get; set; }
    [Networked] public NetworkBool CanDodge { get; set; }
    [Networked] public NetworkBool IsBlocking { get; set; }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();

        if (HasInputAuthority)
        {
            // Este es mi jugador local
            cameraPlayer.transform.parent = null;
            canvas.transform.parent = null;
            cameraFollow = cameraPlayer.GetComponent<CameraFollow>(); 
            cameraFollow.player = this.gameObject.transform;
            cameraFollow.StartCamera();
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (chunkManager != null && chunkManager.players.Contains(this.transform))
        {
            chunkManager.players.Remove(this.transform);
            chunkManager.UpdateChunks(); // Actualiza para descargar los chunks que hayan quedado solos
        }
    }

    public override void FixedUpdateNetwork()
    {       
        if (!GetInput(out NetworkInputData input))
            return;
       
        float moveInput = input.movement.x;

        if (moveInput != 0)
        {
            // Actualizamos la variable de red
            IsFacingLeft = moveInput < 0;
        }

        //Interaccion 
        if (input.buttons.WasPressed(previousButtons, InputButtons.Interact) && !IsInteracting)
        {
            if (IsCrouching)
            {
                Debug.Log("Act Crouching");
            }
            else
            {
                animator.Play("Act");
                animatorC.Play("Act");
                animatorB.Play("Act");
            }
        }

        // Attack Player
        if (input.buttons.WasPressed(previousButtons, InputButtons.Attack))
        {
            AttackPlayer();
        }
        // Reducir tiempo de combo
        if (IsAttacking)
        {
            comboTimer -= Runner.DeltaTime;
            if (comboTimer <= 0)
            {
                ComboStep = 0;
                comboTimer = comboDelay;
                IsAttacking = false;
            }
        }
      
        // Block Down and Up
        if (input.buttons.WasPressed(previousButtons, InputButtons.Block))
        {
            if (!IsAttacking && !IsInteracting)
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
            IsJumping = true;
            jumpTimeCounter = maxJumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if (input.buttons.IsSet(InputButtons.Jump) && IsJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.gravityScale = 0.1f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Runner.DeltaTime;
            }
            else
            {
                IsJumping = false;
                rb.gravityScale = normalGravity;
            }
        }
        if (input.buttons.WasReleased(previousButtons, InputButtons.Jump))
        {
            IsJumping = false;
            rb.gravityScale = normalGravity;
        }
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
        if (!IsInteracting)
        {
            
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
            if (!IsCrouching && !IsInteracting)
            {
                rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
            }
        }




        //Agacharse y caida
        if (isGrounded)
        {
            //Roll
            if (input.buttons.WasPressed(previousButtons, InputButtons.Run))
            {
                shiftHeldTime = 0f;
            }
            if (input.buttons.IsSet(InputButtons.Run))
            {
                shiftHeldTime += Runner.DeltaTime;
            }
            if (input.buttons.WasReleased(previousButtons, InputButtons.Run) && CanDodge)
            {
                if (shiftHeldTime <= 0.2f)
                {
                    StartCoroutine(Dodge());
                }
            }

            // =========================================================
            // INICIO DEL AGACHARSE 
            // =========================================================
            float verticalInput = input.movement.y;

            // Cuando se presiona abajo
            if (verticalInput < 0 && !IsCrouching)
            {
                IsInteracting = true;
                IsCrouching = true;
                rb.linearVelocityX = 0; // Lógica de física/movimiento
            }
            // Cuando se suelta abajo
            else if (verticalInput >= 0 && IsCrouching)
            {
                IsCrouching = false;
                // Si necesitas resetear IsInteracting aquí o al levantarte, hazlo según tu lógica
            }
        }

        // Detectar inicio de caída
        if (!isGrounded && rb.linearVelocity.y < 0 && !IsFalling)
        {
            IsFalling = true;
            fallStartTime = Runner.DeltaTime; ; // Guardar cuando empezó a caer
        }

        // Detectar aterrizaje
        if (isGrounded && IsFalling)
        {
            IsFalling = false;
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

    // 3. Render() se ejecuta localmente en cada frame (ideal para cambios visuales fluídos)
    public override void Render()
    {
        ActualizarOrientacionVisual(IsFacingLeft);

        // =========================================================
        // SINCRONIZAR CON EL ANIMATOR
        // =========================================================
        IsInteracting = animator.GetBool("isInteracting");
        IsBlocking = animator.GetBool("blocking");
        SetBoolOnAll("isAttacking", IsAttacking);
        SetBoolOnAll("isJumping", IsJumping);

        // =========================================================
        // INICIO DEL AGACHARSE 
        // =========================================================
        if (IsCrouching != _lastIsCrouching)
        {
            if (IsCrouching)
            {
                // Transición de inicio (StartCrouch)
                PlayAnimationOnAll("StarCrouch");
                SetBoolOnAll("Crouch", true);
            }
            else
            {
                // Transición al levantarse
                SetBoolOnAll("Crouch", false);
            }

            // Actualizamos la memoria local del cliente
            _lastIsCrouching = IsCrouching;
        }

        // =========================================================
        // INICIO DEL ATAQUE
        // =========================================================

        if (IsAttacking && !lastIsAttacking)
        {           
            if (!animatorP.GetBool("Walk") &&
                !animatorP.GetBool("Run"))
            {
                animatorP.Play("Attack");
            }
            animator.Play("Attack");
            animatorC.Play("Attack");
            animatorB.Play("Attack");

            if (weaponManager != null &&
                weaponManager.anim != null)
            {
                weaponManager.anim.Play("Attack");
            }
        }

        // Guardamos el estado anterior

        lastIsAttacking = IsAttacking;
    }

    // Métodos auxiliares para no repetir código de tus múltiples animadores
    private void SetBoolOnAll(string paramName, bool value)
    {
        if (animator) animator.SetBool(paramName, value);
        if (animatorP) animatorP.SetBool(paramName, value);
        if (animatorC) animatorC.SetBool(paramName, value);
        if (animatorB) animatorB.SetBool(paramName, value);
        if (weaponManager && weaponManager.anim) weaponManager.anim.SetBool(paramName, value);
    }

    private void PlayAnimationOnAll(string stateName)
    {
        if (animator) animator.Play(stateName);
        if (animatorP) animatorP.Play(stateName);
        if (animatorC) animatorC.Play(stateName);
        if (animatorB) animatorB.Play(stateName);
        if (weaponManager && weaponManager.anim) weaponManager.anim.Play(stateName);
    }

    private void ActualizarOrientacionVisual(bool facingLeft)
    {
        spriteRenderer.flipX = facingLeft;
        spritePiernas.flipX = facingLeft;
        spriteCabeza.flipX = facingLeft;
        spriteBrazo.flipX = facingLeft;

        // Scale del pivote
        Vector3 scale = espadaPivot.localScale;
        scale.x = facingLeft ? -1f : 1f;
        espadaPivot.localScale = scale;
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
        if (!IsAttacking && !IsInteracting)
        {
            // Comienza el combo

            IsAttacking = true;

            ComboStep = 1;

            comboTimer = comboDelay;

            return;

        }
        else if (ComboStep == 1 && comboTimer > 0)
        {
            ComboStep = 2;

            comboTimer = comboDelay;

            return;
        }
        else if (ComboStep == 2 && comboTimer > 0)
        {
            ComboStep = 3;

            comboTimer = comboDelay;

            return;
        }
    }

    IEnumerator Dodge()
    {
        CanDodge = false;

        // Reproducir la animación
        animator.SetBool("isInteracting", true);
        if(!IsCrouching)
        {
            PlayAnimationOnAll("Dodge");
        }
        else
        {
            PlayAnimationOnAll("CrouchSlide");
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
        CanDodge = true;
    }   

    public void StopVelocity()
    {
        animator.SetBool("isInteracting", true);
        rb.linearVelocity = Vector2.zero;
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

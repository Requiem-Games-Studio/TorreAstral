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

    // Para detectar cambios en Render
    private ChangeDetector _changeDetector;

    public GameObject cameraPlayer, canvas;
    [HideInInspector]
    public CameraFollow cameraFollow;
    public ChunkManagerByName chunkManager;

    private NetworkButtons previousButtons;

    [Header("Velocidades")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private float shiftHeldTime;
    private bool _lastIsCrouching;

    [Networked] private float MoveInput { get; set; }
    [Networked] private NetworkBool IsRunning { get; set; }

    [Networked] private NetworkBool IsDodging { get; set; }
    [Networked] private TickTimer DodgeTimer { get; set; }
    [Networked] private TickTimer DodgeCooldownTimer { get; set; }

    // Contador que se incrementa cada vez que se dispara un esquive.
    // Sirve para detectar el disparo instantáneo en Render().
    [Networked] private int DodgeCount { get; set; }

    public float dodgeSpeed = 5f;  // Velocidad del dodge
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 1f;

    [Header("Salto")]
    public float jumpForce = 7f;
    public float maxJumpTime = 0.2f; // Tiempo máximo de salto variable
    public float coyoteTime = 0.15f; // Tiempo extra después de dejar el suelo
    public float normalGravity;
    [Networked] private NetworkBool IsJumping { get; set; }
    [Networked] private int JumpCount { get; set; }
    [Networked] private int InteractCount { get; set; }

    // Tiempos y contadores manejados en FUN
    [Networked] private float CoyoteTimeCounter { get; set; }
    [Networked] private float JumpTimeCounter { get; set; }

    //Caida
    bool isGrounded;
    public float fallThreshold = 0.5f; // Tiempo mínimo de caída para animación de aterrizaje
    private float fallStartTime;

    [Networked] private NetworkBool IsFalling { get; set; }
    [Networked] private float FallStartTime { get; set; }

    // Contadores de eventos para Render()
    [Networked] private int HardLandCount { get; set; } // Aterrizaje pesado (Land)
    [Networked] private int SoftLandCount { get; set; } // Aterrizaje ligero (Idle)


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

    [Networked] public NetworkBool IsFacingLeft { get; set; }
    [Networked] public NetworkBool IsInteracting { get; set; }
    [Networked] public NetworkBool IsCrouching { get; set; }
    [Networked] public NetworkBool CanDodge { get; set; }
    [Networked] public NetworkBool IsBlocking { get; set; }
    [Networked] private int BlockStartCount { get; set; }

    private bool _lastIsBlocking;

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

        // En Fusion 2 pasas directamente ChangeDetector.Source.SimulationState
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
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

        if (!IsInteracting)
        {
            // Guardamos el input de movimiento horizontal (-1, 0, 1) y si presiona correr
            MoveInput = input.movement.x;
            IsRunning = input.buttons.IsSet(InputButtons.Run);

            // Cambiar velocidad física según estado
            float speed = IsRunning ? runSpeed : walkSpeed;

            if (MoveInput != 0)
            {
                // Actualizamos la variable de red
                IsFacingLeft = MoveInput < 0;
            }
            
            if (!IsCrouching && !IsInteracting)
            {
                rb.linearVelocity = new Vector2(MoveInput * speed, rb.linearVelocity.y);
            }
        }
        else
        {
            MoveInput = 0f;
            IsRunning = false;
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
                InteractCount++;
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
        // 1. Al presionar el botón de bloqueo
        if (input.buttons.WasPressed(previousButtons, InputButtons.Block))
        {
            if (!IsAttacking && !IsInteracting)
            {
                IsBlocking = true;
                BlockStartCount++; // Avisa a Render()
            }
        }
        // 2. Al soltar el botón de bloqueo
        if (input.buttons.WasReleased(previousButtons, InputButtons.Block))
        {
            IsBlocking = false; // Se vuelve false -> Render() pondrá "blocking" = false
        }

        // =========================================================
        // SALTO
        // =========================================================
        // 1. Manejo de Coyote Time
        if (isGrounded)
        {
            CoyoteTimeCounter = coyoteTime;
        }
        else
        {
            CoyoteTimeCounter -= Runner.DeltaTime;
        }
        // 2. Inicio del Salto (WasPressed)
        if (input.buttons.WasPressed(previousButtons, InputButtons.Jump) && CoyoteTimeCounter > 0f)
        {
            IsJumping = true;
            JumpTimeCounter = maxJumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Incrementamos el contador para avisarle a Render()
            JumpCount++;

            // Consumimos el coyote time para evitar saltos dobles en el mismo frame
            CoyoteTimeCounter = 0f;
        }
        // 3. Salto Sostenido (Mapeo de fuerza variable)
        if (input.buttons.IsSet(InputButtons.Jump) && IsJumping)
        {
            if (JumpTimeCounter > 0)
            {
                rb.gravityScale = 0.1f;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                JumpTimeCounter -= Runner.DeltaTime;
            }
            else
            {
                IsJumping = false;
                rb.gravityScale = normalGravity;
            }
        }
        // 4. Cancelar Salto (WasReleased)
        if (input.buttons.WasReleased(previousButtons, InputButtons.Jump))
        {
            IsJumping = false;
            rb.gravityScale = normalGravity;
        }

        //Agacharse 
        if (isGrounded)
        {
            if (input.buttons.WasPressed(previousButtons, InputButtons.Run))
            {
                shiftHeldTime = 0f;
            }
            if (input.buttons.IsSet(InputButtons.Run))
            {
                shiftHeldTime += Runner.DeltaTime;
            }

            // Comprobamos si puede esquivar mediante el TickTimer de Fusion
            bool canDodge = DodgeCooldownTimer.ExpiredOrNotRunning(Runner) && !IsDodging;

            if (input.buttons.WasReleased(previousButtons, InputButtons.Run) && canDodge)
            {
                if (shiftHeldTime <= 0.2f)
                {
                    TriggerDodge();
                }
            }

            // B) Lógica MIENTRAS está esquivando
            if (IsDodging)
            {
                if (!DodgeTimer.Expired(Runner))
                {
                    // Aplicar velocidad del dodge durante el tiempo activo
                    float dodgeDirection = spriteRenderer.flipX ? -1 : 1;
                    rb.linearVelocity = new Vector2(dodgeDirection * dodgeSpeed, 0);
                }
                else
                {
                    // El dodge ha terminado
                    IsDodging = false;
                    rb.linearVelocity = Vector2.zero;
                    rb.gravityScale = normalGravity;

                    // Iniciar cooldown
                    DodgeCooldownTimer = TickTimer.CreateFromSeconds(Runner, dodgeCooldown);

                    // Volvemos a ignorar la capa de enemigos (restaurar colisión)
                    SetEnemyCollision(false);
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

        // A) DETECTAR INICIO DE CAÍDA REAL
        // Aseguramos que no esté tocando suelo Y que su velocidad vertical vaya hacia abajo (caída)
        if (!isGrounded && !IsFalling && rb.linearVelocity.y < -0.1f)
        {
            IsFalling = true;
            FallStartTime = Runner.SimulationTime; // Guardamos el tiempo exacto en que empezó a caer
        }

        // B) DETECTAR ATERRIZAJE
        if (isGrounded && IsFalling)
        {
            IsFalling = false; // Desactivamos el estado de caída

            // Calculamos la duración total en segundos que estuvo cayendo
            float fallDuration = Runner.SimulationTime - FallStartTime;

            // Si cayó durante un tiempo igual o mayor al umbral -> Impacto Pesado
            if (fallDuration >= fallThreshold)
            {
                rb.linearVelocityX = 0; // Frenado físico por impacto
                HardLandCount++;       // Transición a "Land" en Render()
            }
            else
            {
                // Caída corta (un salto pequeño o bajó un escalón) -> Transición a Idle suave
                SoftLandCount++;       // Transición a "idle" en Render()
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
        SetBoolOnAll("isAttacking", IsAttacking);
        //SetBoolOnAll("isJumping", IsJumping);
        // 1. Calculamos el estado visual de movimiento
        bool isMoving = MoveInput != 0 && !IsInteracting;
        bool walkingState = isMoving && !IsRunning;
        bool runningState = isMoving && IsRunning;

        // 2. Asignamos a todos los animadores
        SetBoolOnAll("Walk", walkingState);
        SetBoolOnAll("Run", runningState);

        // =========================================================
        // CHANGE DETECTOR  FALL - JUMP - DODGE - ACT - BLOCK 
        // =========================================================
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            if (change == nameof(BlockStartCount))
            {
                // 2. FORZAMOS el bool "blocking" a true inmediatamente en los animadores
                SetBoolOnAll("blocking", true);

                // 1. Iniciamos el clip de transición "StartBlock"
                SetBlockStartAnimations();

                // Sincronizamos nuestro rastreador local para evitar duplicados
                _lastIsBlocking = true;
            }

            if (change == nameof(HardLandCount)) PlayAnimationOnAll("Land");
            if (change == nameof(SoftLandCount)) PlayAnimationOnAll("idle");

            if (change == nameof(JumpCount)) PlayAnimationOnAll("Jump");
            if (change == nameof(DodgeCount)) PlayAnimationOnAll(IsCrouching ? "CrouchSlide" : "Dodge");
            if (change == nameof(InteractCount)) PlayAnimationOnAll("Act");
        }
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
        // B) Sincronización continua de la postura sostenida
        if (IsBlocking != _lastIsBlocking)
        {
            SetBoolOnAll("blocking", IsBlocking);
            _lastIsBlocking = IsBlocking;
        }
        // Mantener la sincronización de estados continuos (por ejemplo, Crouch)
        if (IsCrouching != _lastIsCrouching)
        {
            SetBoolOnAll("Crouch", IsCrouching);
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

    private void SetBlockStartAnimations()
    {
        animator.Play("StartBlock");

        // Verificación especial que tenías para animatorP
        if (!animatorP.GetBool("Walk") && !animatorP.GetBool("Run"))
        {
            animatorP.Play("StartBlock");
        }

        animatorC.Play("StartBlock");
        animatorB.Play("StartBlock");
        if (weaponManager && weaponManager.anim) weaponManager.anim.Play("StartBlock");

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

    private void TriggerDodge()
    {
        IsDodging = true;
        IsInteracting = true;

        // Incrementamos el contador para alertar a Render() en todos los clientes
        DodgeCount++;

        // Creamos los timers sincronizados con los ticks del Runner
        DodgeTimer = TickTimer.CreateFromSeconds(Runner, dodgeDuration);

        // Ignorar colisión con enemigos localmente/en red
        SetEnemyCollision(true);
    }

    // Nota sobre colisiones: Es preferible usar Physics2D.IgnoreCollision entre Colliders específicos
    // para evitar desactivar la colisión a nivel global para todos los personajes.
    private void SetEnemyCollision(bool ignore)
    {
        // Ejemplo si tienes la referencia a tus colliders:
        // Physics2D.IgnoreCollision(myCollider, enemyCollider, ignore);
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

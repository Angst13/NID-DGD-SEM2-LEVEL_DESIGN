using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float acceleration = 90f;
    [SerializeField] private float deceleration = 70f;
    [SerializeField] private float velPower = 0.96f;          // FIX 1: raised for snappier feel

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float jumpCutMultiplier = 0.4f;
    [SerializeField] private float fallGravityMultiplier = 3.5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.15f);
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float defaultGravity;
    private bool isGrounded;
    private bool isJumping;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private Vector3 initialScale;

    // FIX 2: track jump animation separately from physics jump state
    private bool playingJumpAnim;
    private float jumpAnimTimer;
    [SerializeField] private float minJumpAnimDuration = 0.1f; // seconds before anim can end

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;
        initialScale = transform.localScale;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        CheckGround();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleJumpCut();
        HandleGravity();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // ─── Animation & Facing ───────────────────────────────────────────────────

    private void UpdateAnimations()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        // FIX 2: keep jump anim alive for at least minJumpAnimDuration
        if (playingJumpAnim)
        {
            jumpAnimTimer -= Time.deltaTime;
            if (jumpAnimTimer <= 0f && isGrounded)
                playingJumpAnim = false;
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isJumping", playingJumpAnim);   // use protected flag

        if (inputX > 0.1f)
            transform.localScale = new Vector3(
                Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (inputX < -0.1f)
            transform.localScale = new Vector3(
                -Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }

    // ─── Ground Check ─────────────────────────────────────────────────────────

    private void CheckGround()
    {
        if (groundCheckPoint != null)
            isGrounded = Physics2D.OverlapBox(
                groundCheckPoint.position, groundCheckSize, 0f, groundLayer);
        else
            isGrounded = Physics2D.OverlapCircle(
                transform.position + Vector3.down * 0.55f, 0.2f, groundLayer);
    }

    // ─── Coyote Time ──────────────────────────────────────────────────────────

    private void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            isJumping = false;          // physics state only — anim uses playingJumpAnim
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    // ─── Jump Buffer ──────────────────────────────────────────────────────────

    private void HandleJumpBuffer()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            Jump();
        }
    }

    // ─── Jump Cut ─────────────────────────────────────────────────────────────

    private void HandleJumpCut()
    {
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
            && isJumping && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier);
        }
    }

    // ─── Jump ─────────────────────────────────────────────────────────────────

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;

        // FIX 2: start protected animation window
        playingJumpAnim = true;
        jumpAnimTimer = minJumpAnimDuration;
    }

    // ─── Gravity ──────────────────────────────────────────────────────────────

    private void HandleGravity()
    {
        if (!isGrounded && rb.linearVelocity.y < 0f)
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        else
            rb.gravityScale = defaultGravity;
    }

    // ─── Movement ─────────────────────────────────────────────────────────────

    private void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal");
        float targetSpeed = input * maxSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        // FIX 1: clamp the force so it can't overshoot at low speedDiff values
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower)
                         * Mathf.Sign(speedDiff);
        movement = Mathf.Clamp(movement, -accelRate, accelRate);  // prevent overshooting

        rb.AddForce(movement * Vector2.right);
    }

    // ─── Gizmos ───────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.55f, 0.2f);
        }
    }
}
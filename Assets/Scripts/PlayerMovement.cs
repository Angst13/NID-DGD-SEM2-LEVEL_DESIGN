using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float groundAcceleration = 80f;   // how fast you reach max speed on ground
    [SerializeField] private float groundFriction = 60f;       // how fast you stop on ground
    [SerializeField] private float airAcceleration = 40f;      // less control in air
    [SerializeField] private float airFriction = 20f;          // less stopping in air

    [Header("Jump")]
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float coyoteTime = 0.12f;         // grace period after walking off ledge
    [SerializeField] private float jumpBufferTime = 0.15f;     // buffer jump press before landing
    [SerializeField] private float jumpCutMultiplier = 0.5f;   // short hop on early release
    [SerializeField] private float fallGravityMultiplier = 2.8f; // snappier fall

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.8f, 0.15f);
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float defaultGravity;

    private bool isGrounded;
    private bool isJumping;
    private float coyoteTimer;
    private float jumpBufferTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    private void Update()
    {
        CheckGround();
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleJumpCut();
        HandleGravity();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // ─── Ground ───────────────────────────────────────────────────────────────

    private void CheckGround()
    {
        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.OverlapBox(
                groundCheckPoint.position, groundCheckSize, 0f, groundLayer);
        }
        else
        {
            // Fallback if no ground check point is assigned
            isGrounded = Physics2D.OverlapCircle(
                transform.position + Vector3.down * 0.55f, 0.2f, groundLayer);
        }
    }

    // ─── Coyote Time ──────────────────────────────────────────────────────────

    private void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    // ─── Jump Buffer ──────────────────────────────────────────────────────────

    private void HandleJumpBuffer()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space)
                        || Input.GetButtonDown("Jump");

        if (jumpPressed)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            Jump();
    }

    // ─── Jump Cut (short hop) ─────────────────────────────────────────────────

    private void HandleJumpCut()
    {
        bool jumpReleased = Input.GetKeyUp(KeyCode.Space)
                         || Input.GetButtonUp("Jump");

        if (jumpReleased && isJumping && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
            coyoteTimer = 0f;
        }
    }

    // ─── Variable Gravity ─────────────────────────────────────────────────────

    private void HandleGravity()
    {
        // Fall faster than you rise — feels much more natural
        if (!isGrounded && rb.linearVelocity.y < 0f)
            rb.gravityScale = defaultGravity * fallGravityMultiplier;
        else
            rb.gravityScale = defaultGravity;
    }

    // ─── Horizontal Movement ──────────────────────────────────────────────────

    private void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal"); // -1, 0, or 1

        float accel = isGrounded ? groundAcceleration : airAcceleration;
        float friction = isGrounded ? groundFriction : airFriction;

        if (Mathf.Abs(input) > 0.01f)
        {
            // Accelerate toward target speed
            float targetSpeed = input * maxSpeed;
            float speedDiff = targetSpeed - rb.linearVelocity.x;
            float force = speedDiff * accel;
            rb.AddForce(new Vector2(force, 0f));
        }
        else
        {
            // No input — apply friction to slow down naturally
            float frictionForce = Mathf.Min(Mathf.Abs(rb.linearVelocity.x), friction)
                                  * -Mathf.Sign(rb.linearVelocity.x);
            rb.AddForce(new Vector2(frictionForce, 0f));
        }

        // Clamp horizontal speed
        rb.linearVelocity = new Vector2(
            Mathf.Clamp(rb.linearVelocity.x, -maxSpeed, maxSpeed),
            rb.linearVelocity.y
        );
    }

    // ─── Jump ─────────────────────────────────────────────────────────────────

    private void Jump()
    {
        // Reset Y velocity first so jump height is always consistent
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
        isJumping = true;
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
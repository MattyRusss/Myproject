using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Animator animator;

    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 8.0f;
    public float airControlForce = 10.0f;
    public float airControlMax = 1.5f;

    [Header("Ground Check")]
    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float xSpeed = Mathf.Abs(rigidBody.linearVelocity.x);
        animator.SetFloat("xspeed", xSpeed);
        // Jump input should be checked in Update() for responsiveness
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        if (rigidBody.linearVelocity.x*transform.localScale.x < 0.0f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void FixedUpdate()
    {
        // Optional: better ground detection using raycast/circle-check
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        float h = Input.GetAxis("Horizontal");

        if (isGrounded)
        {
            // Direct horizontal control on ground
            rigidBody.linearVelocity = new Vector2(h * speed, rigidBody.linearVelocity.y);
        }
        else
        {
            // Air control: apply force but limit max air speed
            float vx = rigidBody.linearVelocity.x;
            if (Mathf.Abs(vx) < airControlMax)
            {
                rigidBody.AddForce(new Vector2(h * airControlForce, 0f), ForceMode2D.Force);
            }
        }
    }

    // If you still want collision-based grounding (optional fallback)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
            isGrounded = false;
    }
}

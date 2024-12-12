using System.Net.Sockets;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public GameObject camera_pos;
    public Vector3 camera_offset;
    public float smoothSpeed = 0.125f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public GameObject bullet;
    public Transform muzzle_R;
    public Transform muzzle_L;

    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public Animator animator;

    public float coolDown = 1;

    public int spreadJump = 10;
    public int spreadRun = 10;
    public int spreadIdle = 10;
    public int spreadCrouch = 10;
    public int fireRange = 10;
    private bool isGrounded;
    private Vector3 previousPosition;
    private bool secondJump = true;
    private int finalSpread;
    private float currentCoolDown = 0;
    public AudioSource audioSource;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        finalSpread = spreadIdle;
    }

    void FixedUpdate()
    {
        Vector3 smoothPos = Vector3.Lerp(camera_pos.transform.position, transform.position + camera_offset, smoothSpeed);
        smoothPos.z = -10;
        camera_pos.transform.position = smoothPos;
    }


    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0) spriteRenderer.flipX = false;
        if (moveInput < 0) spriteRenderer.flipX = true;

        if (!isGrounded)
        {
            if (previousPosition.y > transform.localScale.y) animator.Play("JumpUp");
            if (previousPosition.y < transform.localScale.y) animator.Play("JumpDown");

            finalSpread = spreadJump;
            previousPosition = rb.linearVelocity;
        }
        else
        {
            if (moveInput != 0)
            {
                animator.Play("Run");
                finalSpread = spreadRun;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.Play("Crouch");
                finalSpread = spreadCrouch;
            }
            else
            {
                animator.Play("Idle");
                finalSpread = spreadIdle;
            }
        }


        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W)) && (isGrounded || secondJump))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (!isGrounded && (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W))) secondJump = false;
        if (isGrounded) secondJump = true;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButton("Fire1") && Time.time > currentCoolDown)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.Play();

            currentCoolDown = Time.time + coolDown;

            Transform muzzle = spriteRenderer.flipX ? muzzle_L : muzzle_R;
            Vector3 position = muzzle.transform.position;

            if (Input.GetKey(KeyCode.S)) position = SetYRelative(muzzle.transform, -0.122f);


            float rand = Random.Range(-finalSpread, finalSpread) + muzzle.transform.rotation.eulerAngles.z;
            Quaternion rotation = Quaternion.AngleAxis(rand, new Vector3(0, 0, 1));

            Instantiate(bullet, position, rotation);

            RaycastHit2D hit = Physics2D.Raycast(position, rotation * Vector3.right);
            Debug.DrawLine(position, position + rotation * Vector3.right * fireRange, Color.red, 0.5f);

            if (hit && hit.distance < fireRange && hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }

    private Vector3 SetYRelative(Transform t, float n)
    {
        return new Vector3(t.position.x, t.position.y + n, t.position.z);
    }

    void OnDrawGizmos()
    {

        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}

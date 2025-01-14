using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class Bomb : MonoBehaviour
{

    public Animator animator;
    public Transform posRight;
    public Transform posLeft;
    public Rigidbody2D rb;
    public float speed;
    public float rest;
    public HealthBar healthBar;
    public GameObject explosion;
    public float health = 5;
    public SpriteRenderer spriteRenderer;
    public bool isExploded = false;

    private float currentTime;
    private Vector3 nextMove;
    private bool flip = true;
    private Vector3 oldPos;
    private Transform player;
    private bool attack = false;
    private Vector3 _captureRight;
    private Vector3 _captureLeft;
    private float actualSpeed;
    public float actualHealth;

    void Start()
    {
        _captureLeft = posLeft.position;
        _captureRight = posRight.position;
        actualSpeed = speed;

        nextMove.x = posRight.position.x;

        actualHealth = health;
        healthBar.SetHealth(actualHealth, health);
    }

    public void GetDMG()
    {
        actualHealth -= 1;
        healthBar.SetHealth(actualHealth, health);
    }

    void FlipNextMove()
    {
        flip = !flip;

        if (flip) nextMove.x = _captureLeft.x;
        else nextMove.x = _captureRight.x;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject gameObject = GameObject.Find("Player");
            player = gameObject.transform;
            return;
        }

        nextMove.y = transform.position.y;


        bool posLarger = Mathf.Abs(transform.position.x) > Mathf.Abs(nextMove.x) * .95f;
        bool posSmaller = Mathf.Abs(transform.position.x) < Mathf.Abs(nextMove.x) * 1.05f;
        if (posLarger && posSmaller)
        {
            if (Time.time > currentTime + rest)
            {
                FlipNextMove();
            }
        }
        else
        {
            currentTime = Time.time;
        }

        spriteRenderer.flipX = transform.position.x < oldPos.x;

    }

    public void Explode()
    {
        Debug.Log("explode");
        isExploded = true;
    }

    public void MakeDead()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (actualHealth <= 0) Explode();
        if (isExploded) animator.Play("explode");
        else if (oldPos != transform.position && !attack) animator.Play("Bomb");
        else animator.Play("Idle");

        oldPos = transform.position;

        if (actualHealth > 0 && !attack)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, nextMove, actualSpeed * Time.deltaTime));
        }
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            _captureLeft = posLeft.position;
            _captureRight = posRight.position;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_captureLeft, _captureRight);
    }
}

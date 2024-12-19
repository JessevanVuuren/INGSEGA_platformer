using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyNight2 : MonoBehaviour
{

    public Animator animator;
    public Transform posRight;
    public Transform posLeft;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public float speed;
    public float playerFoundSpeed;
    public float rest;
    public float hitDistance = 1.6f;
    public HealthBar healthBar;
    public float health = 10;
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
    public bool playerInPlace = false;


    public void EndAttack()
    {
        attack = false;
    }

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


        if (player.position.x > _captureLeft.x && player.position.x < _captureRight.x &&
            player.position.y > transform.position.y - 1 && player.position.y < transform.position.y + 1)
        {
            playerInPlace = true;
            actualSpeed = playerFoundSpeed;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < hitDistance) attack = true;
            else nextMove.x = player.position.x;

        }
        else
        {
            playerInPlace = false;
            actualSpeed = speed;
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

        }

        if (playerInPlace)
        {
            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
        else
        {
            spriteRenderer.flipX = transform.position.x < oldPos.x;
        }
    }

    public void HitPlayer()
    {
        if (player && Vector3.Distance(transform.position, player.position) < hitDistance)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.GetDmg();
        }
    }

    void FixedUpdate()
    {

        if (actualHealth <= 0) animator.Play("Dead");
        else if (attack) animator.Play("hit");
        else if (oldPos != transform.position && !attack) animator.Play("Run");
        else animator.Play("Idle");

        oldPos = transform.position;

        if (actualHealth > 0 && !attack)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, nextMove, actualSpeed * Time.deltaTime));
        }
    }

    public void MakeDead()
    {
        Destroy(gameObject);
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

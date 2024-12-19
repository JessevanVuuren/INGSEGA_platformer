using Unity.VisualScripting;
using UnityEngine;

public class EnemyNight : MonoBehaviour
{

    public Animator animator;
    public float idleWalkWith;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public float speed;
    public float playerFoundSpeed;
    public float rest;
    public float lookRadius;
    public bool enableLook = false;
    private float currentTime;
    private Vector3 nextMove;
    private bool flip = true;
    private Vector3 originalPoint;
    private Vector3 oldPos;
    private Transform player;
    private bool playerFound = false;
    private bool attack = false;

    public void EndAttack()
    {
        attack = false;
    }

    void Start()
    {
        originalPoint = transform.position;
        currentTime = Time.time;

        Vector3 pos = transform.position;
        pos.x += idleWalkWith / 2;
        nextMove = pos;
    }

    void FlipNextMove()
    {
        flip = !flip;
        Vector3 pos = transform.position;
        if (flip) pos.x += idleWalkWith;
        else pos.x -= idleWalkWith;

        nextMove = pos;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").transform;
            return;
        }

        nextMove.y = transform.position.y;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > lookRadius && !playerFound)
        {
            if (Mathf.Abs(transform.position.x) > Mathf.Abs(nextMove.x) * .95f &&
                Mathf.Abs(transform.position.x) < Mathf.Abs(nextMove.x) * 1.05f)
            {
                if (Time.time > currentTime + rest)
                {
                    FlipNextMove();
                    spriteRenderer.flipX = !flip;

                }
            }
            else currentTime = Time.time;

        }
        else
        {
            playerFound = true;
            speed = playerFoundSpeed;

            if (distance < 1.5f)
            {
                attack = true;
            }
            else
            {
                nextMove.x = player.position.x;
            }

            spriteRenderer.flipX = transform.position.x > player.position.x;


        }



    }

    void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(transform.position, nextMove, speed * Time.deltaTime));

        if (attack) animator.Play("hit");
        else if (oldPos != transform.position && !attack) animator.Play("Run");
        else animator.Play("Idle");

        oldPos = transform.position;
    }

    void OnDrawGizmos()
    {
        Vector3 pos;

        if (!Application.isPlaying) pos = transform.position;
        else pos = originalPoint;
        pos.y -= .5f;

        Gizmos.color = Color.yellow;

        Vector3 start = pos;
        start.x -= idleWalkWith / 2;

        Vector3 end = pos;
        end.x += idleWalkWith / 2;

        if (!playerFound) Gizmos.DrawLine(start, end);

        Gizmos.color = Color.white;
        if (enableLook && !playerFound) Gizmos.DrawWireSphere(transform.position, lookRadius);


    }
}

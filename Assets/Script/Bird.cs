using UnityEngine;
using System.Linq;
using UnityEditor;

public class Bird : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 0;
    public HealthBar healthBar;
    public float health = 2;
    public GameObject explosion;
    public SpriteRenderer spriteRenderer;
    public float outerLook = 10;

    private Vector3 playerPos;
    public GameObject player;
    public float actualHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();


        actualHealth = health;
        healthBar.SetHealth(actualHealth, health);
    }

    public void GetDMG()
    {
        actualHealth -= 1;
        healthBar.SetHealth(actualHealth, health);

        if (actualHealth <= 0)
        {
            Explode();
        }
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            return;
        }


        playerPos = player.transform.position;

    }

    void FixedUpdate()
    {

        if (Vector3.Distance(transform.position, playerPos) < outerLook)
        {

            rb.MovePosition(Vector3.MoveTowards(transform.position, playerPos, speed * Time.deltaTime));

            Vector3 direction = playerPos - transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y);

            spriteRenderer.flipY = angle < 0;

            rb.MoveRotation(-(angle * Mathf.Rad2Deg) + 90);
        }
    }

    public void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerLook);
    }
}

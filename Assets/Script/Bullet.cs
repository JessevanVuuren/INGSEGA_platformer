using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 20f;
    public float destroy = 4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroy);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }
}

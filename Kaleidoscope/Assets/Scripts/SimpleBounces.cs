using System.Collections.Generic;
using UnityEngine;

public class SimpleBounces : MonoBehaviour
{
    public float speed = 50f;
    public float angularVelocity = 30f;
    public Vector2 velocity = new Vector2(1, 1);

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.angularDrag = 0;
        rb.angularVelocity = 0;
        rb.bodyType = RigidbodyType2D.Dynamic;
        velocity = velocity.normalized * speed;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    List<ContactPoint2D> contacts = new List<ContactPoint2D>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.scene == gameObject.scene)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, velocity);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider == collision)
                {
                    Vector3 N = hit.normal;
                    Vector3 R = Vector3.Reflect(velocity, N);
                    velocity = new Vector2(R.x, R.y).normalized * speed;
                    rb.velocity = velocity;
                    break;
                }
            }
        }
    }
}
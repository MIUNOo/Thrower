using UnityEngine;

public class Throwable : MonoBehaviour
{
    public int mass;
    public int damage;

    private Rigidbody2D rb;

    // Initialization for Throwable with Rigidbody
    private void Awake()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.gravityScale = 1f; // Set gravity if necessary
    }

    // Constructor for direct instantiation in code (optional in Unity)
    public Throwable(int mass, int damage)
    {
        this.mass = mass;
        this.damage = damage;
    }

    public Throwable()
    {
        mass = 1;
        damage = 10;
    }

    public void ApplyForce(Vector2 direction, float force)
    {
        if (rb != null)
        {
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    public void SetKinematic(bool state)
    {
        if (rb != null)
        {
            rb.isKinematic = state;
        }
    }

    public void ResetParent()
    {
        transform.SetParent(null);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Throwable collided with {collision.collider.name}, causing {damage} damage.");
    }
}

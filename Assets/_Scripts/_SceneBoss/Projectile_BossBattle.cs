using UnityEngine;

public class Projectile_BossBattle : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damageAmount; // Stores the damage this projectile will deal
    public float lifetime = 2f;

    /// <summary>
    /// Initializes the projectile with its direction, speed, and damage.
    /// This is the required method that takes 3 arguments.
    /// </summary>
    /// <param name="dir">The normalized direction vector.</param>
    /// <param name="spd">The speed of the projectile.</param>
    /// <param name="dmg">The amount of damage to deal on hit.</param>
    public void Initialize(Vector2 dir, float spd, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        damageAmount = dmg;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy_BossBattle"))
        {
            // Get the BaseBossController to damage any type of boss
            BaseBossController boss = other.GetComponent<BaseBossController>();
            if (boss != null)
            {
                boss.TakeDamage(damageAmount); // Use the stored damage
            }
            Destroy(gameObject);
        }
    }
}

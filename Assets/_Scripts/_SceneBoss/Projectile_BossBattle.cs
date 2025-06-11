using UnityEngine;

public class Projectile_BossBattle : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    public float lifetime = 2f;
    public float damageAmount = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // --- CRITICAL FIX ---
        // We now check for the "Enemy_BossBattle" tag, and get the BaseBossController component.
        if (other.CompareTag("Enemy_BossBattle"))
        {
            Debug.Log($"Player projectile hit {other.name}");
            BaseBossController boss = other.GetComponent<BaseBossController>();
            if (boss != null)
            {
                boss.TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
    }
}
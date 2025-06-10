using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    public float lifetime = 3f; // 子弹存活时间

    // 你也可以在这里定义子弹的伤害值
    // public float damage = 10f;

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
    if (other.CompareTag("PlayerCharacter_Boss"))
    {
        Debug.Log("Boss projectile hit the player!");
        PlayerController_BossBattle player = other.GetComponent<PlayerController_BossBattle>();
        if (player != null)
        {
            // 假设Boss子弹的伤害值是固定的，或者从Boss那里获取
            int damageToDeal = 10; // 示例伤害值
            player.TakeDamage(damageToDeal);
        }
        Destroy(gameObject); 
    }
}
        // 可以添加其他碰撞逻辑，比如碰到场景边界销毁等
        // else if (!other.CompareTag("Enemy_BossBattle") && !other.CompareTag("EnemyProjectile_BossBattle"))
        // {
        //     Destroy(gameObject);
        // }
}


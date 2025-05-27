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
        // 检查是否碰撞到玩家 (假设玩家有 "PlayerCharacter_Boss" 标签)
        if (other.CompareTag("PlayerCharacter_Boss"))
        {
            Debug.Log("Boss的子弹击中玩家!");
            // 在这里获取玩家的脚本并调用其受伤方法
            // PlayerController_BossBattle player = other.GetComponent<PlayerController_BossBattle>();
            // if (player != null)
            // {
            //     player.TakeDamage(damage); // 假设玩家有TakeDamage方法
            // }
            Destroy(gameObject); // 销毁子弹
        }
        // 可以添加其他碰撞逻辑，比如碰到场景边界销毁等
        // else if (!other.CompareTag("Enemy_BossBattle") && !other.CompareTag("EnemyProjectile_BossBattle"))
        // {
        //     Destroy(gameObject);
        // }
    }
}

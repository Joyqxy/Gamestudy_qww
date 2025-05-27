using UnityEngine;

public class Projectile_BossBattle : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    public float lifetime = 2f; // 子弹存活时间 (秒)
    public float damageAmount = 10f; // 子弹造成的伤害值

    void Start()
    {
        // 确保子弹预制体自身有正确的标签
        if (gameObject.tag != "PlayerProjectile_Boss")
        {
            Debug.LogWarning("玩家子弹预制体 (PlayerProjectile) 没有设置正确的标签 'PlayerProjectile_Boss'。请在预制体上设置。");
            // gameObject.tag = "PlayerProjectile_Boss"; // 或者在这里强制设置，但不推荐
        }
        Destroy(gameObject, lifetime); // 在存活时间结束后销毁子弹
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
        // 检查是否击中Boss (敌人)
        if (other.CompareTag("Enemy_BossBattle"))
        {
            Debug.Log("玩家子弹击中Boss!");
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damageAmount);
            }
            Destroy(gameObject); // 销毁子弹本身
        }
        // 避免子弹与玩家或玩家的其他子弹碰撞后自我销毁
        else if (other.CompareTag("PlayerCharacter_Boss") || other.CompareTag("PlayerProjectile_Boss"))
        {
            // 如果碰到玩家或者其他玩家子弹，不做任何事 (子弹继续飞行)
            // 或者你也可以选择在这里销毁子弹，如果不想让子弹穿过玩家
            // Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true); // 动态忽略本次碰撞，但更推荐用Layer Collision Matrix
            return;
        }
        // 如果碰到其他非特定物体 (比如场景边界或其他类型的障碍物)，则销毁子弹
        // 这个判断可以根据你的游戏设计来调整
        else
        {
            // Debug.Log($"玩家子弹碰到了其他物体: {other.name}，标签: {other.tag}");
            // Destroy(gameObject); // 如果需要碰到任何非敌人/非玩家物体就销毁，则取消这行注释
        }
    }
}

using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float moveSpeed = 2f; // 稍微提高一点追逐速度
    public float contactDamage = 15f; // Boss碰撞玩家时造成的伤害

    [Header("攻击设置")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 7f;
    public float fireRate = 1f;
    public float attackRange = 8f;     // 进入此范围Boss会停下射击
    public float chaseStopDistance = 2f; // Boss追逐玩家时，保持的最小距离（小于attackRange时生效，避免完全重叠）
    private float nextFireTime = 0f;

    [Header("行为模式")]
    private Transform playerTransform;
    private bool isChasing = false;
    private Rigidbody2D rb; // 添加 Rigidbody2D 引用

    [Header("移动边界 (世界坐标)")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    // 你需要根据你的场景和摄像机视野来调整这些边界值

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>(); // 获取 Rigidbody2D 组件
        if (rb == null)
        {
            Debug.LogError("Boss 对象上缺少 Rigidbody2D 组件!");
            enabled = false; // 禁用脚本以防出错
            return;
        }
        rb.isKinematic = true; // 确保是 Kinematic，因为我们直接控制位置

        GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerCharacter_Boss");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Boss 未能找到玩家对象! 请确保玩家对象有 'PlayerCharacter_Boss' 标签。");
            enabled = false; // 找不到玩家就禁用脚本
            return;
        }

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        if (distanceToPlayer <= attackRange)
        {
            // 玩家在攻击范围内
            isChasing = false; // 停止追逐
            FacePlayerLogic(directionToPlayer); // 面向玩家的逻辑
            AttemptToShoot();

            // 如果距离太近，可以稍微后退一点或停止移动，避免完全重叠在玩家身上
            if (distanceToPlayer < chaseStopDistance)
            {
                // 可以选择不移动，或者非常缓慢地调整位置
                // rb.velocity = Vector2.zero; // 如果是用velocity控制的话
            }
            else
            {
                // 在攻击范围内，但不在停止距离内，可以缓慢向玩家移动或进行特定攻击模式的移动
                MoveTowards(directionToPlayer, moveSpeed * 0.5f); // 例如以半速缓慢靠近
            }
        }
        else
        {
            // 玩家在攻击范围外，开始追逐
            isChasing = true;
            FacePlayerLogic(directionToPlayer);
            MoveTowards(directionToPlayer, moveSpeed);
        }
    }

    void MoveTowards(Vector2 direction, float speed)
    {
        // 使用 Rigidbody2D.MovePosition 来移动 Kinematic body，这样能更好地处理碰撞
        // MovePosition 需要在 FixedUpdate 中调用以获得最佳物理效果，
        // 但对于Kinematic且不依赖复杂物理交互的情况，在Update中配合Time.deltaTime也可以接受
        // 为简单起见，我们先直接用 transform.Translate 并结合边界限制

        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime;

        // 限制移动范围
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;

        // 如果想用 rb.MovePosition (通常在 FixedUpdate 中):
        // Vector2 targetPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        // targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        // targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        // rb.MovePosition(targetPosition);
    }

    void FacePlayerLogic(Vector2 directionToPlayer)
    {
        // 简单的视觉朝向 (如果Boss Sprite有明确左右朝向)
        // 例如，如果Boss的默认“右边”是朝前:
        // if (directionToPlayer.x < 0) // 玩家在左边
        // {
        //     transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        // }
        // else if (directionToPlayer.x > 0) // 玩家在右边
        // {
        //     transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        // }
        // 对于俯视角，子弹发射方向更重要
    }

    void AttemptToShoot()
    {
        if (projectilePrefab == null) return;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        Vector2 directionToShoot = (playerTransform.position - firePoint.position).normalized;
        // 如果Boss有视觉朝向，并且发射点也应该跟随朝向，这里的 firePoint.rotation 可能需要调整
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // 通常子弹不需要Boss的旋转
        BossProjectile bossProjectileScript = projectile.GetComponent<BossProjectile>();

        if (bossProjectileScript != null)
        {
            bossProjectileScript.Initialize(directionToShoot, projectileSpeed);
        }
        else
        {
            Debug.LogWarning("Boss的子弹预制体上缺少 BossProjectile 脚本!");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Boss 受到伤害: {amount}, 当前血量: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss 被击败!");
        Destroy(gameObject);
    }

    // 处理Boss与玩家的物理碰撞
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 确保Boss的Collider2D不是Trigger，玩家的Collider2D也不是Trigger
        // 并且两者都有Rigidbody2D
        if (collision.gameObject.CompareTag("PlayerCharacter_Boss"))
        {
            Debug.Log($"Boss 碰撞到玩家! 造成 {contactDamage} 点伤害。");
            // 之后在这里调用玩家的受伤函数
            // PlayerController_BossBattle player = collision.gameObject.GetComponent<PlayerController_BossBattle>();
            // if (player != null)
            // {
            //     player.TakeDamage(contactDamage); // 假设玩家有TakeDamage方法
            // }
        }
    }

    // 如果Boss的Collider是Trigger，则使用OnTriggerEnter2D
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("PlayerCharacter_Boss"))
    //     {
    //         Debug.Log($"Boss (Trigger) 碰撞到玩家! 造成 {contactDamage} 点伤害。");
    //     }
    // }
}

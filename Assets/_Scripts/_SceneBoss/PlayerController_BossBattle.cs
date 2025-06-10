using UnityEngine;

public class PlayerController_BossBattle : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 2f; // Shots per second

    [Header("Melee Attack Settings")]
    public Transform meleeAttackPoint; // 近战攻击的检测点
    public float meleeAttackRange = 0.5f; // 近战攻击的范围
    public LayerMask enemyLayers; // 定义哪些层是敌人，以便近战攻击检测
    public float meleeAttackRate = 1.5f; // 每秒近战攻击次数
    public int meleeDamage = 15; // 近战伤害值

    private float nextFireTime = 0f;
    private float nextMeleeTime = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        HandleMovement_Update();
        HandleAttack();
    }

    void HandleMovement_Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void HandleAttack()
    {
        if (Input.GetButton("Fire1")) // 默认鼠标左键
        {
            if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.HasEnoughBullets(1))
            {
                // 有子弹，执行远程攻击
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                // 没有子弹，执行近战攻击
                if (Time.time >= nextMeleeTime)
                {
                    MeleeAttack();
                    nextMeleeTime = Time.time + 1f / meleeAttackRate;
                }
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || PlayerDataManager.Instance == null) return;

        // 消耗子弹
        PlayerDataManager.Instance.SpendBullets(1);

        // 创建子弹
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile_BossBattle projectileScript = projectile.GetComponent<Projectile_BossBattle>();

        if (projectileScript != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;
            projectileScript.Initialize(direction, projectileSpeed);
        }
    }

    void MeleeAttack()
    {
        Debug.Log("Executing Melee Attack!");
        // 在这里可以播放近战攻击动画
        // GetComponent<Animator>()?.SetTrigger("MeleeAttack");

        // 检测近战范围内的敌人
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, enemyLayers);

        // 对检测到的每个敌人造成伤害
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Melee hit: {enemy.name}");
            BossController boss = enemy.GetComponent<BossController>(); // 假设敌人脚本是BossController
            if (boss != null)
            {
                boss.TakeDamage(meleeDamage);
            }
            // 如果有其他类型的敌人，在这里添加对它们的伤害逻辑
        }
    }

    // 玩家接受伤害的方法
    public void TakeDamage(int damage)
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.TakeDamage(damage);
        }
    }

    // 用于在编辑器中可视化近战攻击范围
    void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
    }
}
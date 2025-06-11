using UnityEngine;

public class PlayerController_BossBattle : MonoBehaviour
{
    [Header("Scene Dependencies")]
    public BossSceneManager sceneManager; // 在Inspector中链接BossSceneManager

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 2f; // 射速
    public float meleeAttackRate = 1.5f; // 近战攻速
    public int meleeDamage = 15;

    [Header("Melee Attack Config")]
    public Transform meleeAttackPoint;
    public float meleeAttackRange = 0.5f;
    public LayerMask enemyLayers;

    private float nextFireTime = 0f;
    private float nextMeleeTime = 0f;
    private bool outOfBullets = false; // 本地变量，用于决定攻击模式

    void Update()
    {
        HandleMovement_Update();
        CheckAttackMode(); // 检查攻击模式
        HandleAttack();
    }

    void HandleMovement_Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    // 决定是使用远程还是近战
    void CheckAttackMode()
    {
        // 在这个简化模型中，我们始终有子弹
        outOfBullets = false;

        // 如果未来要重新引入子弹概念，可以取消下面的注释
        // if (PlayerDataManager.Instance != null) {
        //     outOfBullets = !PlayerDataManager.Instance.HasEnoughBullets(1);
        // }
    }

    void HandleAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!outOfBullets) // 始终为false，所以总是尝试射击
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            // else // 近战逻辑暂时不会被触发，但保留结构
            // {
            //     if (Time.time >= nextMeleeTime)
            //     {
            //         MeleeAttack();
            //         nextMeleeTime = Time.time + 1f / meleeAttackRate;
            //     }
            // }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;
        // 不再需要消耗子弹
        // PlayerDataManager.Instance.SpendBullets(1); 

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

    public void TakeDamage(int damage)
    {
        if (sceneManager != null)
        {
            sceneManager.PlayerTakesDamage(damage);
        }
        else
        {
            Debug.LogError("[PlayerController] SceneManager reference not set!");
        }
    }

    // MeleeAttack 和 OnDrawGizmosSelected 方法保持不变，作为备用
    void MeleeAttack() { /* ... as before ... */ }
    void OnDrawGizmosSelected() { /* ... as before ... */ }
}
using UnityEngine;

public class PlayerController_BossBattle : MonoBehaviour
{
    public float moveSpeed = 5f; // 玩家移动速度
    public GameObject projectilePrefab; // 子弹预制体 (需要在Inspector中指定)
    public Transform firePoint;         // 子弹发射点 (可选, 可以是玩家中心，也可以是一个子对象)
    public float projectileSpeed = 10f; // 子弹速度
    public float fireRate = 0.5f;       // 开火速率 (每多少秒可以发射一次)
    private float nextFireTime = 0f;    // 下一次可以开火的时间

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (firePoint == null)
        {
            firePoint = transform; // 如果没有指定发射点，就从玩家中心发射
        }
    }

    void Update()
    {
        HandleMovement_Update(); // 使用Update处理移动输入并直接修改transform
        HandleShooting();
    }

    // void FixedUpdate()
    // {
    //     // 如果Rigidbody2D的Body Type是Dynamic，并且想用物理力移动，可以在这里处理
    //     // HandleMovement_FixedUpdate(); 
    // }

    void HandleMovement_Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // 获取水平输入 (A/D 或 左/右箭头)
        float moveY = Input.GetAxisRaw("Vertical");   // 获取垂直输入 (W/S 或 上/下箭头)

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized; // 归一化，确保斜向移动速度不会过快

        // 直接修改 transform.position (适用于 Kinematic Rigidbody 或无 Rigidbody 的情况)
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 如果你的 Body Type 是 Kinematic，并且希望通过 Rigidbody2D 的方法移动以获得更好的碰撞插值：
        // if (rb != null && rb.isKinematic)
        // {
        //     rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime); 
        //     // 注意: MovePosition 建议在 FixedUpdate 中使用，这里只是一个示例
        // }
    }

    // void HandleMovement_FixedUpdate()
    // {
    //     float moveX = Input.GetAxisRaw("Horizontal");
    //     float moveY = Input.GetAxisRaw("Vertical");
    //     Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

    //     if (rb != null && !rb.isKinematic) // 适用于 Dynamic Rigidbody
    //     {
    //         rb.velocity = moveDirection * moveSpeed;
    //     }
    //     else if (rb != null && rb.isKinematic) // 适用于 Kinematic Rigidbody
    //     {
    //          rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    //     }
    // }


    void HandleShooting()
    {
        // 示例：使用鼠标左键开火
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) // "Fire1" 通常是鼠标左键或Ctrl
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate; // 计算下一次可以开火的时间 (1f / fireRate 得到的是开火间隔)
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("子弹预制体 (Projectile Prefab) 未在PlayerController中指定!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile_BossBattle projectileScript = projectile.GetComponent<Projectile_BossBattle>();

        if (projectileScript != null)
        {
            // 获取鼠标在世界中的位置 (适用于2D正交相机)
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // 确保z轴为0 (因为我们是2D)

            Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;
            projectileScript.Initialize(direction, projectileSpeed);
        }
        else
        {
            // 如果子弹没有特定脚本，可以直接给它一个速度
            // Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            // if (projRb != null)
            // {
            //     Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     mouseWorldPosition.z = 0;
            //     Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;
            //     projRb.velocity = direction * projectileSpeed;
            // }
            Debug.LogWarning("子弹预制体上缺少 Projectile_BossBattle 脚本，或者子弹速度将不会被正确初始化。");
        }
    }
}

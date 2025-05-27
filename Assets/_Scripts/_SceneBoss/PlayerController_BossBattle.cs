using UnityEngine;

public class PlayerController_BossBattle : MonoBehaviour
{
    public float moveSpeed = 5f; // ����ƶ��ٶ�
    public GameObject projectilePrefab; // �ӵ�Ԥ���� (��Ҫ��Inspector��ָ��)
    public Transform firePoint;         // �ӵ������ (��ѡ, ������������ģ�Ҳ������һ���Ӷ���)
    public float projectileSpeed = 10f; // �ӵ��ٶ�
    public float fireRate = 0.5f;       // �������� (ÿ��������Է���һ��)
    private float nextFireTime = 0f;    // ��һ�ο��Կ����ʱ��

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (firePoint == null)
        {
            firePoint = transform; // ���û��ָ������㣬�ʹ�������ķ���
        }
    }

    void Update()
    {
        HandleMovement_Update(); // ʹ��Update�����ƶ����벢ֱ���޸�transform
        HandleShooting();
    }

    // void FixedUpdate()
    // {
    //     // ���Rigidbody2D��Body Type��Dynamic�����������������ƶ������������ﴦ��
    //     // HandleMovement_FixedUpdate(); 
    // }

    void HandleMovement_Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // ��ȡˮƽ���� (A/D �� ��/�Ҽ�ͷ)
        float moveY = Input.GetAxisRaw("Vertical");   // ��ȡ��ֱ���� (W/S �� ��/�¼�ͷ)

        Vector2 moveDirection = new Vector2(moveX, moveY).normalized; // ��һ����ȷ��б���ƶ��ٶȲ������

        // ֱ���޸� transform.position (������ Kinematic Rigidbody ���� Rigidbody �����)
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // ������ Body Type �� Kinematic������ϣ��ͨ�� Rigidbody2D �ķ����ƶ��Ի�ø��õ���ײ��ֵ��
        // if (rb != null && rb.isKinematic)
        // {
        //     rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime); 
        //     // ע��: MovePosition ������ FixedUpdate ��ʹ�ã�����ֻ��һ��ʾ��
        // }
    }

    // void HandleMovement_FixedUpdate()
    // {
    //     float moveX = Input.GetAxisRaw("Horizontal");
    //     float moveY = Input.GetAxisRaw("Vertical");
    //     Vector2 moveDirection = new Vector2(moveX, moveY).normalized;

    //     if (rb != null && !rb.isKinematic) // ������ Dynamic Rigidbody
    //     {
    //         rb.velocity = moveDirection * moveSpeed;
    //     }
    //     else if (rb != null && rb.isKinematic) // ������ Kinematic Rigidbody
    //     {
    //          rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    //     }
    // }


    void HandleShooting()
    {
        // ʾ����ʹ������������
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) // "Fire1" ͨ������������Ctrl
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate; // ������һ�ο��Կ����ʱ�� (1f / fireRate �õ����ǿ�����)
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("�ӵ�Ԥ���� (Projectile Prefab) δ��PlayerController��ָ��!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile_BossBattle projectileScript = projectile.GetComponent<Projectile_BossBattle>();

        if (projectileScript != null)
        {
            // ��ȡ����������е�λ�� (������2D�������)
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // ȷ��z��Ϊ0 (��Ϊ������2D)

            Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;
            projectileScript.Initialize(direction, projectileSpeed);
        }
        else
        {
            // ����ӵ�û���ض��ű�������ֱ�Ӹ���һ���ٶ�
            // Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            // if (projRb != null)
            // {
            //     Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //     mouseWorldPosition.z = 0;
            //     Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;
            //     projRb.velocity = direction * projectileSpeed;
            // }
            Debug.LogWarning("�ӵ�Ԥ������ȱ�� Projectile_BossBattle �ű��������ӵ��ٶȽ����ᱻ��ȷ��ʼ����");
        }
    }
}

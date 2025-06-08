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
    public Transform meleeAttackPoint; // ��ս�����ļ���
    public float meleeAttackRange = 0.5f; // ��ս�����ķ�Χ
    public LayerMask enemyLayers; // ������Щ���ǵ��ˣ��Ա��ս�������
    public float meleeAttackRate = 1.5f; // ÿ���ս��������
    public int meleeDamage = 15; // ��ս�˺�ֵ

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
        if (Input.GetButton("Fire1")) // Ĭ��������
        {
            if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.HasEnoughBullets(1))
            {
                // ���ӵ���ִ��Զ�̹���
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                // û���ӵ���ִ�н�ս����
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

        // �����ӵ�
        PlayerDataManager.Instance.SpendBullets(1);

        // �����ӵ�
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
        // ��������Բ��Ž�ս��������
        // GetComponent<Animator>()?.SetTrigger("MeleeAttack");

        // ����ս��Χ�ڵĵ���
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, enemyLayers);

        // �Լ�⵽��ÿ����������˺�
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Melee hit: {enemy.name}");
            BossController boss = enemy.GetComponent<BossController>(); // ������˽ű���BossController
            if (boss != null)
            {
                boss.TakeDamage(meleeDamage);
            }
            // ������������͵ĵ��ˣ���������Ӷ����ǵ��˺��߼�
        }
    }

    // ��ҽ����˺��ķ���
    public void TakeDamage(int damage)
    {
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.TakeDamage(damage);
        }
    }

    // �����ڱ༭���п��ӻ���ս������Χ
    void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
    }
}
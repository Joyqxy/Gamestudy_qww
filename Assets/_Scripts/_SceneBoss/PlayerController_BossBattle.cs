using UnityEngine;

public class PlayerController_BossBattle : MonoBehaviour
{
    [Header("Scene Dependencies")]
    public BossSceneManager sceneManager; // ��Inspector������BossSceneManager

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 2f; // ����
    public float meleeAttackRate = 1.5f; // ��ս����
    public int meleeDamage = 15;

    [Header("Melee Attack Config")]
    public Transform meleeAttackPoint;
    public float meleeAttackRange = 0.5f;
    public LayerMask enemyLayers;

    private float nextFireTime = 0f;
    private float nextMeleeTime = 0f;
    private bool outOfBullets = false; // ���ر��������ھ�������ģʽ

    void Update()
    {
        HandleMovement_Update();
        CheckAttackMode(); // ��鹥��ģʽ
        HandleAttack();
    }

    void HandleMovement_Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    // ������ʹ��Զ�̻��ǽ�ս
    void CheckAttackMode()
    {
        // �������ģ���У�����ʼ�����ӵ�
        outOfBullets = false;

        // ���δ��Ҫ���������ӵ��������ȡ�������ע��
        // if (PlayerDataManager.Instance != null) {
        //     outOfBullets = !PlayerDataManager.Instance.HasEnoughBullets(1);
        // }
    }

    void HandleAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!outOfBullets) // ʼ��Ϊfalse���������ǳ������
            {
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            // else // ��ս�߼���ʱ���ᱻ�������������ṹ
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
        // ������Ҫ�����ӵ�
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

    // MeleeAttack �� OnDrawGizmosSelected �������ֲ��䣬��Ϊ����
    void MeleeAttack() { /* ... as before ... */ }
    void OnDrawGizmosSelected() { /* ... as before ... */ }
}
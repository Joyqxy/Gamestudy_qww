using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("��������")]
    public float maxHealth = 100f;
    private float currentHealth;
    public float moveSpeed = 2f; // ��΢���һ��׷���ٶ�
    public float contactDamage = 15f; // Boss��ײ���ʱ��ɵ��˺�

    [Header("��������")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 7f;
    public float fireRate = 1f;
    public float attackRange = 8f;     // ����˷�ΧBoss��ͣ�����
    public float chaseStopDistance = 2f; // Boss׷�����ʱ�����ֵ���С���루С��attackRangeʱ��Ч��������ȫ�ص���
    private float nextFireTime = 0f;

    [Header("��Ϊģʽ")]
    private Transform playerTransform;
    private bool isChasing = false;
    private Rigidbody2D rb; // ��� Rigidbody2D ����

    [Header("�ƶ��߽� (��������)")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;
    // ����Ҫ������ĳ������������Ұ��������Щ�߽�ֵ

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>(); // ��ȡ Rigidbody2D ���
        if (rb == null)
        {
            Debug.LogError("Boss ������ȱ�� Rigidbody2D ���!");
            enabled = false; // ���ýű��Է�����
            return;
        }
        rb.isKinematic = true; // ȷ���� Kinematic����Ϊ����ֱ�ӿ���λ��

        GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerCharacter_Boss");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Boss δ���ҵ���Ҷ���! ��ȷ����Ҷ����� 'PlayerCharacter_Boss' ��ǩ��");
            enabled = false; // �Ҳ�����Ҿͽ��ýű�
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
            // ����ڹ�����Χ��
            isChasing = false; // ֹͣ׷��
            FacePlayerLogic(directionToPlayer); // ������ҵ��߼�
            AttemptToShoot();

            // �������̫����������΢����һ���ֹͣ�ƶ���������ȫ�ص����������
            if (distanceToPlayer < chaseStopDistance)
            {
                // ����ѡ���ƶ������߷ǳ������ص���λ��
                // rb.velocity = Vector2.zero; // �������velocity���ƵĻ�
            }
            else
            {
                // �ڹ�����Χ�ڣ�������ֹͣ�����ڣ����Ի���������ƶ�������ض�����ģʽ���ƶ�
                MoveTowards(directionToPlayer, moveSpeed * 0.5f); // �����԰��ٻ�������
            }
        }
        else
        {
            // ����ڹ�����Χ�⣬��ʼ׷��
            isChasing = true;
            FacePlayerLogic(directionToPlayer);
            MoveTowards(directionToPlayer, moveSpeed);
        }
    }

    void MoveTowards(Vector2 direction, float speed)
    {
        // ʹ�� Rigidbody2D.MovePosition ���ƶ� Kinematic body�������ܸ��õش�����ײ
        // MovePosition ��Ҫ�� FixedUpdate �е����Ի���������Ч����
        // ������Kinematic�Ҳ������������������������Update�����Time.deltaTimeҲ���Խ���
        // Ϊ�������������ֱ���� transform.Translate ����ϱ߽�����

        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime;

        // �����ƶ���Χ
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        transform.position = newPosition;

        // ������� rb.MovePosition (ͨ���� FixedUpdate ��):
        // Vector2 targetPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        // targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        // targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        // rb.MovePosition(targetPosition);
    }

    void FacePlayerLogic(Vector2 directionToPlayer)
    {
        // �򵥵��Ӿ����� (���Boss Sprite����ȷ���ҳ���)
        // ���磬���Boss��Ĭ�ϡ��ұߡ��ǳ�ǰ:
        // if (directionToPlayer.x < 0) // ��������
        // {
        //     transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        // }
        // else if (directionToPlayer.x > 0) // ������ұ�
        // {
        //     transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        // }
        // ���ڸ��ӽǣ��ӵ����䷽�����Ҫ
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
        // ���Boss���Ӿ����򣬲��ҷ����ҲӦ�ø��泯������� firePoint.rotation ������Ҫ����
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // ͨ���ӵ�����ҪBoss����ת
        BossProjectile bossProjectileScript = projectile.GetComponent<BossProjectile>();

        if (bossProjectileScript != null)
        {
            bossProjectileScript.Initialize(directionToShoot, projectileSpeed);
        }
        else
        {
            Debug.LogWarning("Boss���ӵ�Ԥ������ȱ�� BossProjectile �ű�!");
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Boss �ܵ��˺�: {amount}, ��ǰѪ��: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss ������!");
        Destroy(gameObject);
    }

    // ����Boss����ҵ�������ײ
    void OnCollisionEnter2D(Collision2D collision)
    {
        // ȷ��Boss��Collider2D����Trigger����ҵ�Collider2DҲ����Trigger
        // �������߶���Rigidbody2D
        if (collision.gameObject.CompareTag("PlayerCharacter_Boss"))
        {
            Debug.Log($"Boss ��ײ�����! ��� {contactDamage} ���˺���");
            // ֮�������������ҵ����˺���
            // PlayerController_BossBattle player = collision.gameObject.GetComponent<PlayerController_BossBattle>();
            // if (player != null)
            // {
            //     player.TakeDamage(contactDamage); // ���������TakeDamage����
            // }
        }
    }

    // ���Boss��Collider��Trigger����ʹ��OnTriggerEnter2D
    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("PlayerCharacter_Boss"))
    //     {
    //         Debug.Log($"Boss (Trigger) ��ײ�����! ��� {contactDamage} ���˺���");
    //     }
    // }
}

using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    public float lifetime = 3f; // �ӵ����ʱ��

    // ��Ҳ���������ﶨ���ӵ����˺�ֵ
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
    if (other.CompareTag("PlayerCharacter_Boss"))
    {
        Debug.Log("Boss projectile hit the player!");
        PlayerController_BossBattle player = other.GetComponent<PlayerController_BossBattle>();
        if (player != null)
        {
            // ����Boss�ӵ����˺�ֵ�ǹ̶��ģ����ߴ�Boss�����ȡ
            int damageToDeal = 10; // ʾ���˺�ֵ
            player.TakeDamage(damageToDeal);
        }
        Destroy(gameObject); 
    }
}
        // �������������ײ�߼����������������߽����ٵ�
        // else if (!other.CompareTag("Enemy_BossBattle") && !other.CompareTag("EnemyProjectile_BossBattle"))
        // {
        //     Destroy(gameObject);
        // }
}


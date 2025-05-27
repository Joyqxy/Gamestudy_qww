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
        // ����Ƿ���ײ����� (��������� "PlayerCharacter_Boss" ��ǩ)
        if (other.CompareTag("PlayerCharacter_Boss"))
        {
            Debug.Log("Boss���ӵ��������!");
            // �������ȡ��ҵĽű������������˷���
            // PlayerController_BossBattle player = other.GetComponent<PlayerController_BossBattle>();
            // if (player != null)
            // {
            //     player.TakeDamage(damage); // ���������TakeDamage����
            // }
            Destroy(gameObject); // �����ӵ�
        }
        // �������������ײ�߼����������������߽����ٵ�
        // else if (!other.CompareTag("Enemy_BossBattle") && !other.CompareTag("EnemyProjectile_BossBattle"))
        // {
        //     Destroy(gameObject);
        // }
    }
}

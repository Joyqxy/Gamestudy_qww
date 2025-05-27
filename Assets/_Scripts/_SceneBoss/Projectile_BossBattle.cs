using UnityEngine;

public class Projectile_BossBattle : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    public float lifetime = 2f; // �ӵ����ʱ�� (��)
    public float damageAmount = 10f; // �ӵ���ɵ��˺�ֵ

    void Start()
    {
        // ȷ���ӵ�Ԥ������������ȷ�ı�ǩ
        if (gameObject.tag != "PlayerProjectile_Boss")
        {
            Debug.LogWarning("����ӵ�Ԥ���� (PlayerProjectile) û��������ȷ�ı�ǩ 'PlayerProjectile_Boss'������Ԥ���������á�");
            // gameObject.tag = "PlayerProjectile_Boss"; // ����������ǿ�����ã������Ƽ�
        }
        Destroy(gameObject, lifetime); // �ڴ��ʱ������������ӵ�
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
        // ����Ƿ����Boss (����)
        if (other.CompareTag("Enemy_BossBattle"))
        {
            Debug.Log("����ӵ�����Boss!");
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damageAmount);
            }
            Destroy(gameObject); // �����ӵ�����
        }
        // �����ӵ�����һ���ҵ������ӵ���ײ����������
        else if (other.CompareTag("PlayerCharacter_Boss") || other.CompareTag("PlayerProjectile_Boss"))
        {
            // ���������һ�����������ӵ��������κ��� (�ӵ���������)
            // ������Ҳ����ѡ�������������ӵ�������������ӵ��������
            // Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true); // ��̬���Ա�����ײ�������Ƽ���Layer Collision Matrix
            return;
        }
        // ��������������ض����� (���糡���߽���������͵��ϰ���)���������ӵ�
        // ����жϿ��Ը��������Ϸ���������
        else
        {
            // Debug.Log($"����ӵ���������������: {other.name}����ǩ: {other.tag}");
            // Destroy(gameObject); // �����Ҫ�����κηǵ���/�������������٣���ȡ������ע��
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI_BossBattle : MonoBehaviour
{
    [Header("UI Element References")]
    public Image healthBarFill;
    public Text bulletCountText;

    // ������Ҫ�����¼�����Ϊ��BossSceneManagerֱ�ӹ���
    void OnEnable() { }
    void OnDisable() { }
    void Start() { }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthBarFill != null)
        {
            if (maxHealth == 0) return; // ����������
            float healthPercent = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = healthPercent;
        }
    }

    // ������ʾ�����ӵ�
    public void UpdateBulletsInfinite()
    {
        if (bulletCountText != null)
        {
            bulletCountText.text = "�ӵ�: ��";
        }
    }
}

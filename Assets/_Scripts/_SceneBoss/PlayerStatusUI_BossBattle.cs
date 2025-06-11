using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI_BossBattle : MonoBehaviour
{
    [Header("UI Element References")]
    public Image healthBarFill;
    public Text bulletCountText;

    // 不再需要订阅事件，因为由BossSceneManager直接管理
    void OnEnable() { }
    void OnDisable() { }
    void Start() { }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (healthBarFill != null)
        {
            if (maxHealth == 0) return; // 避免除零错误
            float healthPercent = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = healthPercent;
        }
    }

    // 用于显示无限子弹
    public void UpdateBulletsInfinite()
    {
        if (bulletCountText != null)
        {
            bulletCountText.text = "子弹: ∞";
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI_BossBattle : MonoBehaviour
{
    [Header("Player UI References")]
    public Image playerHealthBarFill;
    public Text bulletCountText;

    [Header("Boss UI References")]
    public Image bossHealthBarFill;

    private BaseBossController subscribedBoss = null;

    void OnDisable()
    {
        if (subscribedBoss != null)
        {
            Debug.Log($"[PlayerStatusUI] OnDisable: Unsubscribing from {subscribedBoss.name}'s health updates.");
            subscribedBoss.OnHealthChanged -= UpdateBossHealthUI;
        }
    }

    void Start()
    {
        if (bossHealthBarFill != null)
        {
            bossHealthBarFill.transform.parent.gameObject.SetActive(false);
        }
        UpdateBulletsInfinite();
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (playerHealthBarFill != null)
        {
            if (maxHealth <= 0) return;
            float healthPercent = (float)currentHealth / maxHealth;
            playerHealthBarFill.fillAmount = healthPercent;
        }
    }

    public void UpdateBulletsInfinite()
    {
        if (bulletCountText != null)
        {
            bulletCountText.text = "×Óµ¯: ¡Þ";
        }
    }

    public void RegisterBossHealthBar(BaseBossController boss)
    {
        if (boss == null)
        {
            Debug.LogError("[PlayerStatusUI] RegisterBossHealthBar was called with a NULL boss.");
            if (bossHealthBarFill != null) bossHealthBarFill.transform.parent.gameObject.SetActive(false);
            return;
        }

        if (subscribedBoss != null)
        {
            subscribedBoss.OnHealthChanged -= UpdateBossHealthUI;
        }

        subscribedBoss = boss;
        subscribedBoss.OnHealthChanged += UpdateBossHealthUI;

        if (bossHealthBarFill != null)
        {
            bossHealthBarFill.transform.parent.gameObject.SetActive(true);
            Debug.Log("[PlayerStatusUI] Boss health bar activated.");
        }

        Debug.Log($"[PlayerStatusUI] Successfully registered and subscribed to health updates for boss: {boss.name}");
    }

    private void UpdateBossHealthUI(float currentHealth, float maxHealth)
    {
        if (bossHealthBarFill == null)
        {
            Debug.LogError("[PlayerStatusUI] UpdateBossHealthUI called, but bossHealthBarFill is not assigned!");
            return;
        }

        if (maxHealth <= 0)
        {
            Debug.LogWarning("[PlayerStatusUI] Boss maxHealth is 0. Cannot update health bar.");
            return;
        }

        float healthPercent = currentHealth / maxHealth;
        bossHealthBarFill.fillAmount = healthPercent;
        Debug.Log($"[PlayerStatusUI] BOSS HEALTH UI UPDATED. Fill Amount set to: {healthPercent}");
    }
}
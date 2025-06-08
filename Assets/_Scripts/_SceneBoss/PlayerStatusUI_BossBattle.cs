// PlayerStatusUI_BossBattle.cs (带有诊断日志的修正版)
using UnityEngine;
using UnityEngine.UI;
// using TMPro;

public class PlayerStatusUI_BossBattle : MonoBehaviour
{
    [Header("UI Element References")]
    public Image healthBarFill;
    public Text bulletCountText;
    // public TextMeshProUGUI bulletCountText_TMP;

    void OnEnable()
    {
        Debug.Log("[PlayerStatusUI] OnEnable: Subscribing to OnPlayerDataUpdated.");
        PlayerDataManager.OnPlayerDataUpdated += UpdateUI;
    }

    void OnDisable()
    {
        Debug.Log("[PlayerStatusUI] OnDisable: Unsubscribing from OnPlayerDataUpdated.");
        PlayerDataManager.OnPlayerDataUpdated -= UpdateUI;
    }

    void Start()
    {
        // 检查UI链接是否正确
        if (healthBarFill == null)
        {
            Debug.LogError("[PlayerStatusUI] ERROR: Health Bar Fill Image is not assigned in the Inspector!");
        }
        if (bulletCountText == null)
        {
            Debug.LogError("[PlayerStatusUI] ERROR: Bullet Count Text is not assigned in the Inspector!");
        }

        // 初始更新
        Debug.Log("[PlayerStatusUI] Start: Performing initial UI update.");
        UpdateUI();
    }

    // 更新所有UI元素的方法
    void UpdateUI()
    {
        Debug.Log("[PlayerStatusUI] UpdateUI method called."); // 确认事件被接收

        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("[PlayerStatusUI] PlayerDataManager.Instance not found. UI update aborted.");
            return;
        }

        // 更新血条
        if (healthBarFill != null)
        {
            // 确保maxPlayerHealth不为0，避免除零错误
            if (PlayerDataManager.Instance.maxPlayerHealth == 0)
            {
                Debug.LogError("[PlayerStatusUI] maxPlayerHealth is 0! Cannot calculate health percentage.");
                return;
            }

            float healthPercent = (float)PlayerDataManager.Instance.currentPlayerHealth / PlayerDataManager.Instance.maxPlayerHealth;
            Debug.Log($"[PlayerStatusUI] Updating Health Bar. Current Health: {PlayerDataManager.Instance.currentPlayerHealth}, Max Health: {PlayerDataManager.Instance.maxPlayerHealth}, Calculated Fill Amount: {healthPercent}");

            healthBarFill.fillAmount = healthPercent;
        }
        else
        {
            Debug.LogWarning("[PlayerStatusUI] healthBarFill reference is null, cannot update health bar.");
        }

        // 更新子弹数
        if (bulletCountText != null)
        {
            bulletCountText.text = $"子弹: {PlayerDataManager.Instance.currentBulletCount}";
        }
    }
}
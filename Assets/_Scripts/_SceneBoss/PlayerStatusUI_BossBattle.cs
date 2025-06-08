// PlayerStatusUI_BossBattle.cs (���������־��������)
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
        // ���UI�����Ƿ���ȷ
        if (healthBarFill == null)
        {
            Debug.LogError("[PlayerStatusUI] ERROR: Health Bar Fill Image is not assigned in the Inspector!");
        }
        if (bulletCountText == null)
        {
            Debug.LogError("[PlayerStatusUI] ERROR: Bullet Count Text is not assigned in the Inspector!");
        }

        // ��ʼ����
        Debug.Log("[PlayerStatusUI] Start: Performing initial UI update.");
        UpdateUI();
    }

    // ��������UIԪ�صķ���
    void UpdateUI()
    {
        Debug.Log("[PlayerStatusUI] UpdateUI method called."); // ȷ���¼�������

        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("[PlayerStatusUI] PlayerDataManager.Instance not found. UI update aborted.");
            return;
        }

        // ����Ѫ��
        if (healthBarFill != null)
        {
            // ȷ��maxPlayerHealth��Ϊ0������������
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

        // �����ӵ���
        if (bulletCountText != null)
        {
            bulletCountText.text = $"�ӵ�: {PlayerDataManager.Instance.currentBulletCount}";
        }
    }
}
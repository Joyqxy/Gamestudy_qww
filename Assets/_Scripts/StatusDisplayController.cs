using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatusDisplayController : MonoBehaviour
{
    public static StatusDisplayController Instance { get; private set; }

    [Header("UI Panel")]
    public GameObject statusPanel;

    [Header("Upgrade Text References")]
    public Text speedUpgradeText;
    public Text damageUpgradeText;
    public Text fireRateUpgradeText;
    // If using TextMeshPro, change the type of these variables to TextMeshProUGUI

    private bool isPanelVisible = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        if (statusPanel != null)
        {
            statusPanel.SetActive(false);
            isPanelVisible = false;
        }
    }

    void OnEnable()
    {
        PlayerDataManager.OnPlayerDataUpdated += UpdateDisplay;
    }

    void OnDisable()
    {
        PlayerDataManager.OnPlayerDataUpdated -= UpdateDisplay;
    }

    void Start()
    {
        UpdateDisplay();
    }

    public void ToggleStatusPanel()
    {
        isPanelVisible = !isPanelVisible;
        if (statusPanel != null)
        {
            statusPanel.SetActive(isPanelVisible);
            if (isPanelVisible)
            {
                UpdateDisplay();
            }
        }
    }

    public void UpdateDisplay()
    {
        if (PlayerDataManager.Instance == null) return;

        // Only update text if the panel is meant to be visible and is active
        if (statusPanel != null && statusPanel.activeInHierarchy)
        {
            string activatedText = "�Ѽ���";   // "Activated"
            string notActivatedText = "δ����"; // "Not Activated"

            if (speedUpgradeText != null)
                speedUpgradeText.text = $"��������: {(PlayerDataManager.Instance.HasSpeedUpgrade ? activatedText : notActivatedText)}";

            if (damageUpgradeText != null)
                damageUpgradeText.text = $"�˺�����: {(PlayerDataManager.Instance.HasDamageUpgrade ? activatedText : notActivatedText)}";

            if (fireRateUpgradeText != null)
                fireRateUpgradeText.text = $"��������: {(PlayerDataManager.Instance.HasFireRateUpgrade ? activatedText : notActivatedText)}";
        }
    }
}
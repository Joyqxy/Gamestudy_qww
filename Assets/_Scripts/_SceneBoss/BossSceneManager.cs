using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [Header("Player State")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Object References")]
    public PlayerController_BossBattle playerController; // ��Inspector��������Ҷ���
    public BaseBossController bossController; // ��Inspector������Boss����

    [Header("UI References")]
    public PlayerStatusUI_BossBattle statusUI; // ��Inspector������UI������
    public GameObject gameOverPanel; // ��Inspector��������Ϸ����UI���
    public Button restartButton;
    public Button returnToCorridorButton;

    [Header("Scene Transition")]
    public string corridorSceneName = "_SceneLong";

    private bool isGameOver = false;

    void Start()
    {
        InitializeScene();
    }

    void InitializeScene()
    {
        isGameOver = false;
        currentHealth = maxHealth;

        // ȷ��UI�����ĳ�ʼ״̬��ȷ
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (statusUI != null)
        {
            statusUI.UpdateHealth(currentHealth, maxHealth);
            statusUI.UpdateBulletsInfinite(); // �����ӵ�UIΪ����
        }

        // Ϊ��ť��Ӽ����¼�
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartScene);
        }
        if (returnToCorridorButton != null)
        {
            returnToCorridorButton.onClick.AddListener(ReturnToCorridor);
        }

        Debug.Log("[BossSceneManager] Scene Initialized. Player health is " + currentHealth);
    }

    public void PlayerTakesDamage(int damage)
    {
        if (isGameOver || currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"[BossSceneManager] Player took {damage} damage. Current health: {currentHealth}");

        // ����Ѫ��UI
        if (statusUI != null)
        {
            statusUI.UpdateHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            HandlePlayerDeath();
        }
    }

    void HandlePlayerDeath()
    {
        isGameOver = true;
        Debug.Log("[BossSceneManager] Player has been defeated. Showing Game Over panel.");

        // ������Һ�Boss���ж�
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        if (bossController != null)
        {
            bossController.enabled = false; // ����Boss��Updateѭ��
        }

        // ��ʾ��Ϸ�������
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartScene()
    {
        Debug.Log("[BossSceneManager] Restarting current scene.");
        // ���¼��ص�ǰ����������״̬���Զ�����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToCorridor()
    {
        Debug.Log("[BossSceneManager] Returning to corridor scene.");
        // ���ﲻ����Ҫ�������ݣ���ΪBossս�Ƕ����ĻỰ
        SceneManager.LoadScene(corridorSceneName);
    }
}
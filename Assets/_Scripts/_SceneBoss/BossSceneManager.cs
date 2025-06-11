using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [Header("Player State")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Object References")]
    public PlayerController_BossBattle playerController; // 在Inspector中链接玩家对象
    public BaseBossController bossController; // 在Inspector中链接Boss对象

    [Header("UI References")]
    public PlayerStatusUI_BossBattle statusUI; // 在Inspector中链接UI管理器
    public GameObject gameOverPanel; // 在Inspector中链接游戏结束UI面板
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

        // 确保UI和面板的初始状态正确
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (statusUI != null)
        {
            statusUI.UpdateHealth(currentHealth, maxHealth);
            statusUI.UpdateBulletsInfinite(); // 更新子弹UI为无限
        }

        // 为按钮添加监听事件
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

        // 更新血条UI
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

        // 禁用玩家和Boss的行动
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        if (bossController != null)
        {
            bossController.enabled = false; // 禁用Boss的Update循环
        }

        // 显示游戏结束面板
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartScene()
    {
        Debug.Log("[BossSceneManager] Restarting current scene.");
        // 重新加载当前场景，所有状态将自动重置
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToCorridor()
    {
        Debug.Log("[BossSceneManager] Returning to corridor scene.");
        // 这里不再需要保存数据，因为Boss战是独立的会话
        SceneManager.LoadScene(corridorSceneName);
    }
}
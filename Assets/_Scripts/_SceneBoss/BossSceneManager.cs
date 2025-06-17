using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
    [Header("Player State (Local to this scene)")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Object References")]
    public PlayerController_BossBattle playerController;
    public BaseBossController bossController;

    [Header("UI References")]
    public PlayerStatusUI_BossBattle statusUI;
    public GameObject gameOverPanel;
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

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (statusUI == null)
        {
            Debug.LogError("[BossSceneManager] CRITICAL ERROR: Status UI reference is not set!");
            return;
        }

        statusUI.UpdateHealth(currentHealth, maxHealth);
        statusUI.UpdateBulletsInfinite();

        if (bossController != null)
        {
            Debug.Log("[BossSceneManager] Boss Controller found. Attempting to register its health bar with the UI.");
            statusUI.RegisterBossHealthBar(bossController);
        }
        else
        {
            Debug.LogError("[BossSceneManager] CRITICAL ERROR: Boss Controller reference is not set! Cannot initialize boss health bar.");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartScene);
        }
        if (returnToCorridorButton != null)
        {
            returnToCorridorButton.onClick.AddListener(ReturnToCorridor);
        }

        Debug.Log("[BossSceneManager] Scene Initialized. Player local health is " + currentHealth);
    }

    public void PlayerTakesDamage(int damage)
    {
        if (isGameOver || currentHealth <= 0) return;
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
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
        if (playerController != null) playerController.enabled = false;
        if (bossController != null) bossController.enabled = false;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToCorridor()
    {
        SceneManager.LoadScene(corridorSceneName);
    }
}


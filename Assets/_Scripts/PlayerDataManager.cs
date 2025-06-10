using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

// Public enum to define the different stages of the game's progression.
public enum GameProgressionState
{
    Start,                  // The very beginning of the game
    SpringFestival_Complete,  // Player has defeated the Spring Festival boss
    DuanwuFestival_Complete, // Player has defeated the Duanwu Festival boss
    ZhongqiuFestival_Complete // Player has defeated the Zhongqiu Festival boss
}

[System.Serializable]
public class SaveData
{
    // All data to be saved to a file
    public int bulletCount;
    public int playerHealth;
    public int maxPlayerHealth;
    public int currentSpecialItemCount;
    public int maxSpecialItemCount;
    public bool isSpeedBoostActive;
    public bool isAttackBoostActive;
    public bool isFireRateBoostActive;
    public GameProgressionState currentProgression; // Game progression state
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Header("Game Progression")]
    public GameProgressionState currentProgressionState = GameProgressionState.Start;

    [Header("Core Combat Stats")]
    public int currentBulletCount = 0;
    public int currentPlayerHealth = 100;
    public int maxPlayerHealth = 100;

    [Header("Special Items")]
    public int currentSpecialItemCount = 0;
    public int maxSpecialItemCount = 3;

    [Header("Active Buffs")]
    public bool isSpeedBoostActive = false;
    public bool isAttackBoostActive = false;
    public bool isFireRateBoostActive = false;

    private string saveFilePath;
    public static event System.Action OnPlayerDataUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Application.persistentDataPath + "/gameSave_v2.json";
            LoadDataFromFile();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (StatusDisplayController.Instance != null)
            {
                StatusDisplayController.Instance.ToggleStatusPanel();
            }
        }
    }

    // --- Progression Management ---
    public void SetProgressionState(GameProgressionState newState)
    {
        if (newState > currentProgressionState)
        {
            currentProgressionState = newState;
            Debug.Log($"[PlayerDataManager] Progression state advanced to: {newState}");
            OnPlayerDataUpdated?.Invoke();
        }
    }

    // --- Bullet Management ---
    public void AddBullets(int amount)
    {
        if (amount <= 0) return;
        currentBulletCount += amount;
        OnPlayerDataUpdated?.Invoke();
    }

    public void SpendBullets(int amount)
    {
        if (amount <= 0) return;
        currentBulletCount -= amount;
        if (currentBulletCount < 0) currentBulletCount = 0;
        OnPlayerDataUpdated?.Invoke();
    }

    public bool HasEnoughBullets(int amountNeeded)
    {
        return currentBulletCount >= amountNeeded;
    }

    // --- Health Management ---
    public void TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0) return;
        currentPlayerHealth -= damageAmount;
        if (currentPlayerHealth <= 0)
        {
            currentPlayerHealth = 0;
            HandlePlayerDeath();
        }
        OnPlayerDataUpdated?.Invoke();
    }

    public void Heal(int healAmount)
    {
        if (healAmount <= 0 || currentPlayerHealth >= maxPlayerHealth) return;
        currentPlayerHealth += healAmount;
        if (currentPlayerHealth > maxPlayerHealth) currentPlayerHealth = maxPlayerHealth;
        OnPlayerDataUpdated?.Invoke();
    }

    public void SetMaxHealth(int newMaxHealth, bool healToFull = false)
    {
        maxPlayerHealth = Mathf.Max(1, newMaxHealth);
        if (healToFull)
        {
            currentPlayerHealth = maxPlayerHealth;
        }
        else
        {
            currentPlayerHealth = Mathf.Min(currentPlayerHealth, maxPlayerHealth);
        }
        OnPlayerDataUpdated?.Invoke();
    }

    private void HandlePlayerDeath()
    {
        Debug.Log("Player is dead!");
        // TODO: Implement game over logic here.
        OnPlayerDataUpdated?.Invoke();
    }

    // --- Special Item Management ---
    public void AddSpecialItem(int amount = 1)
    {
        if (amount <= 0) return;
        currentSpecialItemCount += amount;
        if (currentSpecialItemCount > maxSpecialItemCount) currentSpecialItemCount = maxSpecialItemCount;
        OnPlayerDataUpdated?.Invoke();
    }

    public bool UseSpecialItem()
    {
        if (currentSpecialItemCount > 0)
        {
            currentSpecialItemCount--;
            OnPlayerDataUpdated?.Invoke();
            return true;
        }
        return false;
    }

    public void SetMaxSpecialItems(int newMax)
    {
        maxSpecialItemCount = Mathf.Max(0, newMax);
        currentSpecialItemCount = Mathf.Min(currentSpecialItemCount, maxSpecialItemCount);
        OnPlayerDataUpdated?.Invoke();
    }

    // --- Skill/Buff Management ---
    public void SetSkillActive(string skillName, bool isActive)
    {
        bool changed = false;
        switch (skillName.ToLower())
        {
            case "speedboost":
                if (isSpeedBoostActive != isActive) { isSpeedBoostActive = isActive; changed = true; }
                break;
            case "attackboost":
                if (isAttackBoostActive != isActive) { isAttackBoostActive = isActive; changed = true; }
                break;
            case "firerateboost":
                if (isFireRateBoostActive != isActive) { isFireRateBoostActive = isActive; changed = true; }
                break;
            default:
                return;
        }
        if (changed) OnPlayerDataUpdated?.Invoke();
    }

    // --- Data Persistence ---
    public void SaveDataToFile()
    {
        SaveData data = new SaveData();
        data.bulletCount = currentBulletCount;
        data.playerHealth = currentPlayerHealth;
        data.maxPlayerHealth = maxPlayerHealth;
        data.currentSpecialItemCount = currentSpecialItemCount;
        data.maxSpecialItemCount = maxSpecialItemCount;
        data.isSpeedBoostActive = isSpeedBoostActive;
        data.isAttackBoostActive = isAttackBoostActive;
        data.isFireRateBoostActive = isFireRateBoostActive;
        data.currentProgression = currentProgressionState;

        string json = JsonUtility.ToJson(data, true);
        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"[PlayerDataManager] Game data saved to: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayerDataManager] Failed to save data: {e.Message}");
        }
    }

    public void LoadDataFromFile()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

                currentBulletCount = loadedData.bulletCount;
                maxPlayerHealth = loadedData.maxPlayerHealth;
                currentPlayerHealth = Mathf.Min(loadedData.playerHealth, maxPlayerHealth);
                maxSpecialItemCount = loadedData.maxSpecialItemCount;
                currentSpecialItemCount = Mathf.Min(loadedData.currentSpecialItemCount, maxSpecialItemCount);
                isSpeedBoostActive = loadedData.isSpeedBoostActive;
                isAttackBoostActive = loadedData.isAttackBoostActive;
                isFireRateBoostActive = loadedData.isFireRateBoostActive;
                currentProgressionState = loadedData.currentProgression;

                Debug.Log($"[PlayerDataManager] Game data loaded. Progression state is: {currentProgressionState}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[PlayerDataManager] Failed to load data: {e.Message}. Using default values.");
                InitializeDefaultData();
            }
        }
        else
        {
            Debug.Log("[PlayerDataManager] Save file not found. Using default values.");
            InitializeDefaultData();
        }
        OnPlayerDataUpdated?.Invoke();
    }

    public void InitializeDefaultData()
    {
        currentBulletCount = 10;
        maxPlayerHealth = 100;
        currentPlayerHealth = maxPlayerHealth;
        maxSpecialItemCount = 3;
        currentSpecialItemCount = 0;
        isSpeedBoostActive = false;
        isAttackBoostActive = false;
        isFireRateBoostActive = false;
        currentProgressionState = GameProgressionState.Start;
        Debug.Log("[PlayerDataManager] Player data initialized to defaults.");
    }

    public void StartNewGameReset()
    {
        InitializeDefaultData();
        SaveDataToFile();
        OnPlayerDataUpdated?.Invoke();
    }

    void OnApplicationQuit()
    {
        SaveDataToFile();
    }
}

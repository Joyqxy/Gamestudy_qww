
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public int upgradesAcquired;
    public string lastSceneName; // CRITICAL: Added field to store the last scene name.
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    [Header("Player Power Level")]
    public int upgradesAcquired = 0;

    // This variable will hold the name of the scene to load when continuing.
    // It is loaded from the save file.
    [Header("Game State")]
    public string sceneToLoadOnContinue = "_SceneLong"; // Default start scene

    public bool HasSpeedUpgrade => upgradesAcquired >= 1;
    public bool HasDamageUpgrade => upgradesAcquired >= 2;
    public bool HasFireRateUpgrade => upgradesAcquired >= 3;

    private string saveFilePath;
    public static event System.Action OnPlayerDataUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Application.persistentDataPath + "/gameSave_simplified_v2.json"; // New save file version
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

    public void GrantNextUpgrade()
    {
        if (upgradesAcquired < 3)
        {
            upgradesAcquired++;
            Debug.Log($"[PlayerDataManager] Upgrade acquired! Player power level is now: {upgradesAcquired}");
            OnPlayerDataUpdated?.Invoke();
        }
        else
        {
            Debug.Log($"[PlayerDataManager] Max upgrades reached. No new upgrade granted.");
        }
    }

    public void SaveDataToFile()
    {
        SaveData data = new SaveData();
        data.upgradesAcquired = upgradesAcquired;

        // CRITICAL: Get the current active scene and save its name.
        // We ensure we don't save utility scenes like the main menu as a load point.
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenuScene") // Assuming your main menu scene is named "MainMenuScene"
        {
            data.lastSceneName = currentScene;
        }
        else
        {
            // If for some reason we save from the main menu, default to the first gameplay scene.
            data.lastSceneName = "_SceneLong";
        }

        string json = JsonUtility.ToJson(data, true);
        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"[PlayerDataManager] Game data saved. Upgrade level: {upgradesAcquired}, Last Scene: {data.lastSceneName}");
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
                upgradesAcquired = loadedData.upgradesAcquired;

                // CRITICAL: Load the last scene name. If it's empty, default to the first gameplay scene.
                if (!string.IsNullOrEmpty(loadedData.lastSceneName))
                {
                    sceneToLoadOnContinue = loadedData.lastSceneName;
                }
                else
                {
                    sceneToLoadOnContinue = "_SceneLong"; // Fallback
                }

                Debug.Log($"[PlayerDataManager] Game data loaded. Upgrade level: {upgradesAcquired}, Scene to load on continue: {sceneToLoadOnContinue}");
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
        upgradesAcquired = 0;
        sceneToLoadOnContinue = "_SceneLong"; // A new game always starts at the first corridor scene.
        Debug.Log("[PlayerDataManager] Player data initialized to defaults. Upgrade level: 0.");
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

using UnityEngine;
using System.IO; // 用于文件操作

// 这个类用于定义要保存到文件的数据结构
[System.Serializable] // 确保这个类可以被JsonUtility序列化
public class SaveData
{
    public int bulletCount;
    // 未来可以添加更多需要保存的数据，比如玩家血量、最高分、解锁的节日等
    // public int playerHealth;
    // public string lastScene;
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; } // 单例模式

    public int currentBulletCount = 0; // 运行时在内存中的子弹数量

    private string saveFilePath; // 存档文件的完整路径

    void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 关键：让这个对象在切换场景时不被销毁
            saveFilePath = Application.persistentDataPath + "/gameSave.json"; // 定义存档文件名和路径
            LoadDataFromFile(); // 游戏启动时（或者这个对象第一次创建时）尝试加载数据
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 如果已存在实例，销毁新的，防止重复
        }
    }

    // 增加子弹
    public void AddBullets(int amount)
    {
        currentBulletCount += amount;
        Debug.Log($"子弹增加: {amount}. 当前子弹数 (内存): {currentBulletCount}");
        // 在这里可以触发一个事件，通知UI更新子弹显示
        // UIManager.Instance.UpdateBulletDisplay(currentBulletCount);
    }

    // （可选）消耗子弹的方法
    public void SpendBullets(int amount)
    {
        currentBulletCount -= amount;
        if (currentBulletCount < 0)
        {
            currentBulletCount = 0;
        }
        Debug.Log($"消耗子弹: {amount}. 当前子弹数 (内存): {currentBulletCount}");
    }

    // 保存数据到本地文件
    public void SaveDataToFile()
    {
        SaveData data = new SaveData();
        data.bulletCount = currentBulletCount;
        // data.playerHealth = someOtherVariable; // 如果有其他数据

        string json = JsonUtility.ToJson(data, true); // true表示格式化JSON，易于阅读

        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"游戏数据已成功保存到: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存数据失败: {e.Message}");
        }
    }

    // 从本地文件加载数据
    public void LoadDataFromFile()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

                currentBulletCount = loadedData.bulletCount;
                // this.playerHealth = loadedData.playerHealth; // 如果有其他数据

                Debug.Log($"游戏数据已从 {saveFilePath} 加载. 当前子弹数: {currentBulletCount}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载数据失败: {e.Message}. 将使用默认值。");
                InitializeDefaultData(); // 加载失败则使用默认值
            }
        }
        else
        {
            Debug.Log("存档文件不存在，使用默认数据初始化。");
            InitializeDefaultData(); // 没有存档文件也使用默认值
        }
    }

    // 初始化默认数据（比如新游戏开始时）
    public void InitializeDefaultData()
    {
        currentBulletCount = 0; // 新游戏时子弹为0
        // playerHealth = 100;
        Debug.Log("玩家数据已初始化为默认值。");
    }

    // （可选）在游戏退出时自动保存
    void OnApplicationQuit()
    {
        SaveDataToFile();
    }
}

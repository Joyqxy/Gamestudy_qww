using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [Header("按钮引用")]
    public Button continueButton;
    public Button newGameButton; // 你可以重命名为 startGameButton 或者保留
    // public Button startGameButton; // 或者专门为“开始游戏”按钮创建一个新的引用

    [Header("场景名称")]
    public string gameSceneName = "_SceneLong"; // 点击“开始游戏”或“继续游戏”后跳转的场景

    private string saveFilePath;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/gameSave_v2.json"; // 确保与PlayerDataManager中的文件名一致

        // 检查存档文件以启用/禁用“继续游戏”按钮
        // 这个逻辑我们之后再完善，当你有明确的“继续”和“全新开始”时
        if (continueButton != null) // 如果你现在就有Continue按钮
        {
            if (File.Exists(saveFilePath))
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }

        // 为“开始游戏”按钮（我们暂时复用newGameButton的逻辑或你可以单独设置）添加监听
        if (newGameButton != null) // 假设“开始游戏”按钮就是这个
        {
            newGameButton.onClick.AddListener(StartGame);
        }
    }

    // “开始游戏”按钮调用的方法
    public void StartGame()
    {
        Debug.Log("开始游戏按钮被点击");
        // PlayerDataManager 的 Awake() 方法应该已经处理了加载存档或初始化默认数据
        // 所以这里我们只需要确保 PlayerDataManager 实例存在即可
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("错误：PlayerDataManager 未被初始化！请确保它在主菜单场景或更早的场景被创建并设置为DontDestroyOnLoad。");
            // 可以考虑在这里强制加载一个包含 PlayerDataManager 的初始场景，或者提示错误
            // 例如: SceneManager.LoadScene("InitializationScene"); return;
            return;
        }

        // 直接加载游戏场景（长廊）
        SceneManager.LoadScene(gameSceneName);
    }

    // --- 以下是未来完善“新游戏”和“继续游戏”时可能用到的方法 ---
    // public void OnNewGameTrulyClicked()
    // {
    //     Debug.Log("真正的“新游戏”按钮被点击");
    //     if (PlayerDataManager.Instance != null)
    //     {
    //         PlayerDataManager.Instance.StartNewGameReset(); // 这会初始化数据并保存一次初始存档
    //     }
    //     SceneManager.LoadScene(gameSceneName);
    // }

    // public void OnContinueGameClicked()
    // {
    //     Debug.Log("继续游戏按钮被点击");
    //     // PlayerDataManager 应该已经在Awake时加载了数据
    //     SceneManager.LoadScene(gameSceneName); // 或者加载玩家上次所在的场景 PlayerDataManager.Instance.lastSceneName
    // }

    public void OnQuitGameButtonClicked()
    {
        Debug.Log("退出游戏按钮被点击");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [Header("��ť����")]
    public Button continueButton;
    public Button newGameButton; // �����������Ϊ startGameButton ���߱���
    // public Button startGameButton; // ����ר��Ϊ����ʼ��Ϸ����ť����һ���µ�����

    [Header("��������")]
    public string gameSceneName = "_SceneLong"; // �������ʼ��Ϸ���򡰼�����Ϸ������ת�ĳ���

    private string saveFilePath;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/gameSave_v2.json"; // ȷ����PlayerDataManager�е��ļ���һ��

        // ���浵�ļ�������/���á�������Ϸ����ť
        // ����߼�����֮�������ƣ���������ȷ�ġ��������͡�ȫ�¿�ʼ��ʱ
        if (continueButton != null) // ��������ھ���Continue��ť
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

        // Ϊ����ʼ��Ϸ����ť��������ʱ����newGameButton���߼�������Ե������ã���Ӽ���
        if (newGameButton != null) // ���衰��ʼ��Ϸ����ť�������
        {
            newGameButton.onClick.AddListener(StartGame);
        }
    }

    // ����ʼ��Ϸ����ť���õķ���
    public void StartGame()
    {
        Debug.Log("��ʼ��Ϸ��ť�����");
        // PlayerDataManager �� Awake() ����Ӧ���Ѿ������˼��ش浵���ʼ��Ĭ������
        // ������������ֻ��Ҫȷ�� PlayerDataManager ʵ�����ڼ���
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("����PlayerDataManager δ����ʼ������ȷ���������˵����������ĳ���������������ΪDontDestroyOnLoad��");
            // ���Կ���������ǿ�Ƽ���һ������ PlayerDataManager �ĳ�ʼ������������ʾ����
            // ����: SceneManager.LoadScene("InitializationScene"); return;
            return;
        }

        // ֱ�Ӽ�����Ϸ���������ȣ�
        SceneManager.LoadScene(gameSceneName);
    }

    // --- ������δ�����ơ�����Ϸ���͡�������Ϸ��ʱ�����õ��ķ��� ---
    // public void OnNewGameTrulyClicked()
    // {
    //     Debug.Log("�����ġ�����Ϸ����ť�����");
    //     if (PlayerDataManager.Instance != null)
    //     {
    //         PlayerDataManager.Instance.StartNewGameReset(); // ����ʼ�����ݲ�����һ�γ�ʼ�浵
    //     }
    //     SceneManager.LoadScene(gameSceneName);
    // }

    // public void OnContinueGameClicked()
    // {
    //     Debug.Log("������Ϸ��ť�����");
    //     // PlayerDataManager Ӧ���Ѿ���Awakeʱ����������
    //     SceneManager.LoadScene(gameSceneName); // ���߼�������ϴ����ڵĳ��� PlayerDataManager.Instance.lastSceneName
    // }

    public void OnQuitGameButtonClicked()
    {
        Debug.Log("�˳���Ϸ��ť�����");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

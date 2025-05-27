using UnityEngine;
using System.IO; // �����ļ�����

// ��������ڶ���Ҫ���浽�ļ������ݽṹ
[System.Serializable] // ȷ���������Ա�JsonUtility���л�
public class SaveData
{
    public int bulletCount;
    // δ��������Ӹ�����Ҫ��������ݣ��������Ѫ������߷֡������Ľ��յ�
    // public int playerHealth;
    // public string lastScene;
}

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; } // ����ģʽ

    public int currentBulletCount = 0; // ����ʱ���ڴ��е��ӵ�����

    private string saveFilePath; // �浵�ļ�������·��

    void Awake()
    {
        // ����ģʽʵ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �ؼ���������������л�����ʱ��������
            saveFilePath = Application.persistentDataPath + "/gameSave.json"; // ����浵�ļ�����·��
            LoadDataFromFile(); // ��Ϸ����ʱ��������������һ�δ���ʱ�����Լ�������
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ����Ѵ���ʵ���������µģ���ֹ�ظ�
        }
    }

    // �����ӵ�
    public void AddBullets(int amount)
    {
        currentBulletCount += amount;
        Debug.Log($"�ӵ�����: {amount}. ��ǰ�ӵ��� (�ڴ�): {currentBulletCount}");
        // ��������Դ���һ���¼���֪ͨUI�����ӵ���ʾ
        // UIManager.Instance.UpdateBulletDisplay(currentBulletCount);
    }

    // ����ѡ�������ӵ��ķ���
    public void SpendBullets(int amount)
    {
        currentBulletCount -= amount;
        if (currentBulletCount < 0)
        {
            currentBulletCount = 0;
        }
        Debug.Log($"�����ӵ�: {amount}. ��ǰ�ӵ��� (�ڴ�): {currentBulletCount}");
    }

    // �������ݵ������ļ�
    public void SaveDataToFile()
    {
        SaveData data = new SaveData();
        data.bulletCount = currentBulletCount;
        // data.playerHealth = someOtherVariable; // �������������

        string json = JsonUtility.ToJson(data, true); // true��ʾ��ʽ��JSON�������Ķ�

        try
        {
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"��Ϸ�����ѳɹ����浽: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"��������ʧ��: {e.Message}");
        }
    }

    // �ӱ����ļ���������
    public void LoadDataFromFile()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

                currentBulletCount = loadedData.bulletCount;
                // this.playerHealth = loadedData.playerHealth; // �������������

                Debug.Log($"��Ϸ�����Ѵ� {saveFilePath} ����. ��ǰ�ӵ���: {currentBulletCount}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"��������ʧ��: {e.Message}. ��ʹ��Ĭ��ֵ��");
                InitializeDefaultData(); // ����ʧ����ʹ��Ĭ��ֵ
            }
        }
        else
        {
            Debug.Log("�浵�ļ������ڣ�ʹ��Ĭ�����ݳ�ʼ����");
            InitializeDefaultData(); // û�д浵�ļ�Ҳʹ��Ĭ��ֵ
        }
    }

    // ��ʼ��Ĭ�����ݣ���������Ϸ��ʼʱ��
    public void InitializeDefaultData()
    {
        currentBulletCount = 0; // ����Ϸʱ�ӵ�Ϊ0
        // playerHealth = 100;
        Debug.Log("��������ѳ�ʼ��ΪĬ��ֵ��");
    }

    // ����ѡ������Ϸ�˳�ʱ�Զ�����
    void OnApplicationQuit()
    {
        SaveDataToFile();
    }
}

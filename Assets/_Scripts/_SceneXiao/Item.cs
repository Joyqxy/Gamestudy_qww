using UnityEngine;
using UnityEngine.SceneManagement;

public class Item : MonoBehaviour
{
    public int x;
    public int y;
    public BoardManager boardManager;

    // ͳһö�٣�����ԭ���� + ��������
    public enum ItemType
    {
        Lantern,      // 0: ԭ����-����
        Dumpling,     // 1: ԭ����-����
        Firecracker,  // 2: ԭ����-����
        Fan,          // 3: ԭ����-����
        duanwu1,      // 4: ����-�������
        duanwu2,      // 5: ����-���絰
        duanwu3,      // 6: ����-����
        duanwu4,      // 7: ����-��ɫ����
        zhongqiu1,    // 8: ����-����
        zhongqiu2,    // 9: ����-�±�
        zhongqiu3,    // 10: ����-����
        zhongqiu4,    // 11: ����-����
        yuanxiao1,    // 12: Ԫ��-��ɫԪ������Դ��yuanxiao1��
        yuanxiao2,    // 13: Ԫ��-��ɫ���ƣ���Դ��yuanxiao2��
        yuanxiao3,    // 14: Ԫ��-���������Դ��yuanxiao3��
        yuanxiao4     // 15: Ԫ��-�̵�������Դ��yuanxiao4��
    }

    public ItemType type;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y, BoardManager manager, ItemType itemType)
    {
        this.x = x;
        this.y = y;
        this.boardManager = manager;
        this.type = itemType;
        SetVisuals();
    }

    public void SetVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Item Prefab ȱ�� SpriteRenderer��");
                return;
            }
        }

        // 1. �жϵ�ǰ������������Դ·��ǰ׺
        string sceneName = SceneManager.GetActiveScene().name;
        string spriteFolder;

        if (sceneName == "_SceneXiao_duanwu")
        {
            spriteFolder = "duanwu/";
        }
        else if (sceneName == "_SceneXiao_zhongqiu")
        {
            spriteFolder = "zhongqiu/";
        }
        else if (sceneName == "_SceneXiao_yuanxiao") // ����Ԫ�������ж�
        {
            spriteFolder = "yuanxiao/";
        }
        else
        {
            spriteFolder = ""; // ԭ����
        }

        // 2. ������Դ��·����Sprites/[folder]/type��
        Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/{spriteFolder}{type}");


        if (loadedSprite != null)
        {
            spriteRenderer.sprite = loadedSprite;
            spriteRenderer.color = Color.white;
        }
        else
        {
            Debug.LogWarning($"ȱ����Դ��Sprites/{spriteFolder}{type}");
            // ��ԭ����������ɫ fallback�����糡��ֱ�ӻ�ɫ
            if (sceneName != "_SceneXiao_duanwu")
            {
                switch (type)
                {
                    case ItemType.Lantern: spriteRenderer.color = new Color(1f, 0.5f, 0f); break;
                    case ItemType.Dumpling: spriteRenderer.color = new Color(0.8f, 1f, 0.8f); break;
                    case ItemType.Firecracker: spriteRenderer.color = new Color(0.7f, 0f, 0f); break;
                    case ItemType.Fan: spriteRenderer.color = new Color(0.8f, 0.8f, 1f); break;
                    default: spriteRenderer.color = Color.grey; break;
                }
            }
            else
            {
                spriteRenderer.color = Color.grey;
            }
        }
    }

    void OnMouseDown()
    {
        if (boardManager != null && !boardManager.IsBoardBusy())
        {
            boardManager.SelectItem(this);
        }
    }
}
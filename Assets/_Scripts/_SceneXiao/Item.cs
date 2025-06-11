using UnityEngine;
using UnityEngine.SceneManagement;

public class Item : MonoBehaviour
{
    public int x;
    public int y;
    public BoardManager boardManager;

    // 统一枚举：包含原类型 + 端午类型
    public enum ItemType
    {
        Lantern,      // 0: 原场景-灯笼
        Dumpling,     // 1: 原场景-粽子
        Firecracker,  // 2: 原场景-鞭炮
        Fan,          // 3: 原场景-扇子
        duanwu1,      // 4: 端午-五彩香囊
        duanwu2,      // 5: 端午-端午蛋
        duanwu3,      // 6: 端午-粽子
        duanwu4,      // 7: 端午-紫色香囊
        zhongqiu1,    // 8: 中秋-灯笼
        zhongqiu2,    // 9: 中秋-月饼
        zhongqiu3,    // 10: 中秋-兔子
        zhongqiu4,    // 11: 中秋-月亮
        yuanxiao1,    // 12: 元宵-白色元宵（资源：yuanxiao1）
        yuanxiao2,    // 13: 元宵-蓝色花纹（资源：yuanxiao2）
        yuanxiao3,    // 14: 元宵-红灯笼（资源：yuanxiao3）
        yuanxiao4     // 15: 元宵-绿灯笼（资源：yuanxiao4）
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
                Debug.LogError("Item Prefab 缺少 SpriteRenderer！");
                return;
            }
        }

        // 1. 判断当前场景，决定资源路径前缀
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
        else if (sceneName == "_SceneXiao_yuanxiao") // 新增元宵场景判断
        {
            spriteFolder = "yuanxiao/";
        }
        else
        {
            spriteFolder = ""; // 原场景
        }

        // 2. 加载资源（路径：Sprites/[folder]/type）
        Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/{spriteFolder}{type}");


        if (loadedSprite != null)
        {
            spriteRenderer.sprite = loadedSprite;
            spriteRenderer.color = Color.white;
        }
        else
        {
            Debug.LogWarning($"缺少资源：Sprites/{spriteFolder}{type}");
            // 仅原场景保留颜色 fallback，端午场景直接灰色
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
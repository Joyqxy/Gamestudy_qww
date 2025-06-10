using UnityEngine;

public class Item : MonoBehaviour
{
    public int x; // 元素在棋盘网格中的 x 坐标
    public int y; // 元素在棋盘网格中的 y 坐标
    public BoardManager boardManager; // 对棋盘管理器的引用

    // 定义不同类型的元素
    public enum ItemType
    {
        //Red,
        //Green,
        //Blue,
        //Yellow,
        //Purple,
        Lantern,      // 灯笼
        Dumpling,     // 粽子
        Firecracker,  // 鞭炮
        Fan           // 扇子
    }

    public ItemType type; // 当前元素的类型

    private SpriteRenderer spriteRenderer; // 用于显示元素的图片

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 初始化元素
    public void Initialize(int x, int y, BoardManager manager, ItemType itemType)
    {
        this.x = x;
        this.y = y;
        this.boardManager = manager;
        this.type = itemType;
        SetVisuals(); // 初始化时设置视觉表现
    }

    // 根据类型设置视觉表现（Sprite优先，找不到则用颜色）
    public void SetVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Item Prefab 上缺少 SpriteRenderer 组件!");
                return;
            }
        }

        // 尝试从 Resources/Sprites 中加载与类型同名的Sprite
        Sprite loadedSprite = Resources.Load<Sprite>("Sprites/" + type.ToString());

        if (loadedSprite != null)
        {
            spriteRenderer.sprite = loadedSprite;
            spriteRenderer.color = Color.white; // 确保Sprite显示真实颜色
        }
        else
        {
            Debug.LogWarning("找不到对应的贴图资源：Sprites/" + type.ToString() + "，使用颜色代替");

            // 如果贴图找不到，用颜色区分
            switch (type)
            {
                //case ItemType.Red:
                //    spriteRenderer.color = Color.red;
                //    break;
                //case ItemType.Green:
                //    spriteRenderer.color = Color.green;
                //    break;
                //case ItemType.Blue:
                //    spriteRenderer.color = Color.blue;
                //    break;
                //case ItemType.Yellow:
                //    spriteRenderer.color = Color.yellow;
                //    break;
                //case ItemType.Purple:
                //    spriteRenderer.color = new Color(0.5f, 0f, 0.5f); // 紫色
                    //break;
                case ItemType.Lantern:
                    spriteRenderer.color = new Color(1f, 0.5f, 0f); // 橙色
                    break;
                case ItemType.Dumpling:
                    spriteRenderer.color = new Color(0.8f, 1f, 0.8f); // 淡绿色
                    break;
                case ItemType.Firecracker:
                    spriteRenderer.color = new Color(0.7f, 0f, 0f); // 暗红
                    break;
                case ItemType.Fan:
                    spriteRenderer.color = new Color(0.8f, 0.8f, 1f); // 淡蓝
                    break;
                default:
                    spriteRenderer.color = Color.grey;
                    break;
            }
        }
    }

    // 当鼠标点击此元素时调用
    void OnMouseDown()
    {
        if (boardManager != null && !boardManager.IsBoardBusy())
        {
            boardManager.SelectItem(this);
        }
    }
}

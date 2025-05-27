using UnityEngine;

public class Item : MonoBehaviour
{
    public int x; // 元素在棋盘网格中的 x 坐标
    public int y; // 元素在棋盘网格中的 y 坐标
    public BoardManager boardManager; // 对棋盘管理器的引用

    // 定义不同类型的元素
    public enum ItemType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        // 后续可以根据你的策划案添加更多类型, 例如: 灯笼, 粽子
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

    // 根据类型设置视觉表现 (颜色或Sprite)
    public void SetVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) // 如果还是找不到，就报错
            {
                Debug.LogError("Item Prefab 上缺少 SpriteRenderer 组件!");
                return;
            }
        }

        // 示例: 根据类型设置颜色。
        // 未来你可以扩展这个方法，根据 ItemType 来加载不同的 Sprite
        switch (type)
        {
            case ItemType.Red:
                spriteRenderer.color = Color.red;
                break;
            case ItemType.Green:
                spriteRenderer.color = Color.green;
                break;
            case ItemType.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case ItemType.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
            case ItemType.Purple:
                spriteRenderer.color = new Color(0.5f, 0f, 0.5f); // 紫色
                break;
            default:
                spriteRenderer.color = Color.grey; // 默认灰色
                break;
        }
        // 如果你有不同类型的Sprite，可以在这里加载：
        // spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());
        // (需要确保你的Sprite图片放在 Assets/Resources/Sprites/ 目录下，并且命名与ItemType枚举一致)
    }

    // 当鼠标点击此元素时调用
    void OnMouseDown()
    {
        // 增加棋盘繁忙状态检查
        if (boardManager != null && !boardManager.IsBoardBusy())
        {
            boardManager.SelectItem(this);
        }
    }
}

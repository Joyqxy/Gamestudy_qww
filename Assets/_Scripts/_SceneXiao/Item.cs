using UnityEngine;

public class Item : MonoBehaviour
{
    public int x; // Ԫ�������������е� x ����
    public int y; // Ԫ�������������е� y ����
    public BoardManager boardManager; // �����̹�����������

    // ���岻ͬ���͵�Ԫ��
    public enum ItemType
    {
        //Red,
        //Green,
        //Blue,
        //Yellow,
        //Purple,
        Lantern,      // ����
        Dumpling,     // ����
        Firecracker,  // ����
        Fan           // ����
    }

    public ItemType type; // ��ǰԪ�ص�����

    private SpriteRenderer spriteRenderer; // ������ʾԪ�ص�ͼƬ

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ��ʼ��Ԫ��
    public void Initialize(int x, int y, BoardManager manager, ItemType itemType)
    {
        this.x = x;
        this.y = y;
        this.boardManager = manager;
        this.type = itemType;
        SetVisuals(); // ��ʼ��ʱ�����Ӿ�����
    }

    // �������������Ӿ����֣�Sprite���ȣ��Ҳ���������ɫ��
    public void SetVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Item Prefab ��ȱ�� SpriteRenderer ���!");
                return;
            }
        }

        // ���Դ� Resources/Sprites �м���������ͬ����Sprite
        Sprite loadedSprite = Resources.Load<Sprite>("Sprites/" + type.ToString());

        if (loadedSprite != null)
        {
            spriteRenderer.sprite = loadedSprite;
            spriteRenderer.color = Color.white; // ȷ��Sprite��ʾ��ʵ��ɫ
        }
        else
        {
            Debug.LogWarning("�Ҳ�����Ӧ����ͼ��Դ��Sprites/" + type.ToString() + "��ʹ����ɫ����");

            // �����ͼ�Ҳ���������ɫ����
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
                //    spriteRenderer.color = new Color(0.5f, 0f, 0.5f); // ��ɫ
                    //break;
                case ItemType.Lantern:
                    spriteRenderer.color = new Color(1f, 0.5f, 0f); // ��ɫ
                    break;
                case ItemType.Dumpling:
                    spriteRenderer.color = new Color(0.8f, 1f, 0.8f); // ����ɫ
                    break;
                case ItemType.Firecracker:
                    spriteRenderer.color = new Color(0.7f, 0f, 0f); // ����
                    break;
                case ItemType.Fan:
                    spriteRenderer.color = new Color(0.8f, 0.8f, 1f); // ����
                    break;
                default:
                    spriteRenderer.color = Color.grey;
                    break;
            }
        }
    }

    // ���������Ԫ��ʱ����
    void OnMouseDown()
    {
        if (boardManager != null && !boardManager.IsBoardBusy())
        {
            boardManager.SelectItem(this);
        }
    }
}

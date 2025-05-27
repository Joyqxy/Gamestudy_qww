using UnityEngine;

public class Item : MonoBehaviour
{
    public int x; // Ԫ�������������е� x ����
    public int y; // Ԫ�������������е� y ����
    public BoardManager boardManager; // �����̹�����������

    // ���岻ͬ���͵�Ԫ��
    public enum ItemType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        // �������Ը�����Ĳ߻�����Ӹ�������, ����: ����, ����
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

    // �������������Ӿ����� (��ɫ��Sprite)
    public void SetVisuals()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) // ��������Ҳ������ͱ���
            {
                Debug.LogError("Item Prefab ��ȱ�� SpriteRenderer ���!");
                return;
            }
        }

        // ʾ��: ��������������ɫ��
        // δ���������չ������������� ItemType �����ز�ͬ�� Sprite
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
                spriteRenderer.color = new Color(0.5f, 0f, 0.5f); // ��ɫ
                break;
            default:
                spriteRenderer.color = Color.grey; // Ĭ�ϻ�ɫ
                break;
        }
        // ������в�ͬ���͵�Sprite��������������أ�
        // spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());
        // (��Ҫȷ�����SpriteͼƬ���� Assets/Resources/Sprites/ Ŀ¼�£�����������ItemTypeö��һ��)
    }

    // ���������Ԫ��ʱ����
    void OnMouseDown()
    {
        // �������̷�æ״̬���
        if (boardManager != null && !boardManager.IsBoardBusy())
        {
            boardManager.SelectItem(this);
        }
    }
}

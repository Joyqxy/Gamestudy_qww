using UnityEngine;
using UnityEngine.UI; // ��ҪUI�����ռ�
using UnityEngine.SceneManagement; // ��Ҫ�������������ռ�
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject itemPrefab;
    public float itemSpacing = 1.1f;

    private const float SWAP_ANIM_DURATION = 0.2f;
    private const float FALL_ANIM_DURATION = 0.3f;
    private const float REFILL_DELAY = 0.3f;
    private const float CASCADE_CHECK_DELAY = 0.5f;
    private float offsetX ;
    private float offsetY ;

    private Item[,] grid;
    public Item selectedItem = null;
    private bool isBoardBusy = false;

    [Header("��Ϸ�߼�")]
    public int scorePerItem = 10;
    public int targetScore = 1000;
    public int itemsToConvertForOneBullet = 5;
    private int currentScore = 0;
    private int itemsEliminatedForBulletConversion = 0;

    [Header("UI����")]
    public Text scoreTextUI;
    public Text bulletCountTextUI;

    [Header("������ת")]
    public string corridorSceneName = "_SceneLong"; // ��ȷ�����������ȳ�������ȷ����

    void Start()
    {
        grid = new Item[width, height];
        StartCoroutine(InitializeBoardAndUI());
        float boardVisualWidth = (width - 1) * itemSpacing;
        float boardVisualHeight = (height - 1) * itemSpacing;
        offsetX = -boardVisualWidth / 2f;
        offsetY = -boardVisualHeight / 2f;
    }

    IEnumerator InitializeBoardAndUI()
    {
        isBoardBusy = true;
        currentScore = 0;
        itemsEliminatedForBulletConversion = 0;

        UpdateScoreUI();
        UpdateBulletUI();
        yield return StartCoroutine(SetupBoard());
        isBoardBusy = false;
    }

    public bool IsBoardBusy()
    {
        return isBoardBusy;
    }

    IEnumerator SetupBoard()
    {
        // �������������Ӿ��ϵ��ܿ�Ⱥ��ܸ߶�
        // ���Ǵӵ�һ��Ԫ�ص����ĵ����һ��Ԫ�ص����ĵľ���
        float boardVisualWidth = (width - 1) * itemSpacing;
        float boardVisualHeight = (height - 1) * itemSpacing;

        // ������Ҫ��ƫ������ʹ���̵����Ķ�׼ (0,0)
        float offsetX = -boardVisualWidth / 2f;
        float offsetY = -boardVisualHeight / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // ����ÿ��Ԫ�ص�X��Y���꣬��Ӧ��ƫ����
                float itemXPos = x * itemSpacing + offsetX;
                float itemYPos = y * itemSpacing + offsetY;

                Vector2 position = new Vector2(itemXPos, itemYPos);

                // ʵ����Ԫ�أ������丸��������Ϊ��ǰBoardManager���ڵ�GameObject��transform
                // ������Ԫ�ص�λ�þ��������BoardManager�ľֲ�λ����
                GameObject newItemGO = Instantiate(itemPrefab, position, Quaternion.identity, this.transform);
                newItemGO.name = $"Item ({x},{y})";

                Item itemComponent = newItemGO.GetComponent<Item>();
                Item.ItemType randomType;

                do
                {
                    randomType = GetRandomItemType();
                    // CausesInitialMatch ������ grid �������Ѿ����ڵ�Ԫ��
                    // ��������ȫ����ǰ�����ڱ߽總����Ԫ�أ���������ܲ�����
                    // �����ڴ󲿷��ڲ�Ԫ������Ч�ġ�
                } while (CausesInitialMatch(x, y, randomType));

                itemComponent.Initialize(x, y, this, randomType); // Item�ڲ���x,y��Ȼ����������
                grid[x, y] = itemComponent;
            }
        }
        yield return null;
    }

    public void SelectItem(Item item)
    {
        if (isBoardBusy || item == null) return;

        if (selectedItem == null)
        {
            selectedItem = item;
            // Debug.Log($"Selected item at ({item.x}, {item.y})");
        }
        else
        {
            if (selectedItem == item) return; // �����ͬһ������������

            if (AreItemsAdjacent(selectedItem, item))
            {
                StartCoroutine(SwapAndProcessMatches(selectedItem, item));
            }
            else // �����ڣ����µ������Ϊѡ��
            {
                selectedItem = item;
                // Debug.Log($"Newly selected item at ({item.x}, {item.y})");
            }
        }
    }

    bool AreItemsAdjacent(Item item1, Item item2)
    {
        if (item1 == null || item2 == null) return false;
        return (Mathf.Abs(item1.x - item2.x) == 1 && item1.y == item2.y) ||
               (Mathf.Abs(item1.y - item2.y) == 1 && item1.x == item2.x);
    }

    void SwapGridPositions(Item item1, Item item2)
    {
        if (item1 == null || item2 == null) return;
        // ���������е�����
        grid[item1.x, item1.y] = item2;
        grid[item2.x, item2.y] = item1;

        // ����Item�����ڲ���¼������
        int tempX = item1.x;
        int tempY = item1.y;
        item1.x = item2.x;
        item1.y = item2.y;
        item2.x = tempX;
        item2.y = tempY;
    }

    IEnumerator AnimateSwap(Item item1, Item item2)
    {
        if (item1 == null || item2 == null) yield break;
        Vector2 pos1 = item1.transform.position;
        Vector2 pos2 = item2.transform.position;
        float elapsed = 0f;
        while (elapsed < SWAP_ANIM_DURATION)
        {
            if (item1 == null || item2 == null)
            { // ���������п��ܱ�����
                Debug.LogWarning("AnimateSwap: Item was destroyed mid-animation.");
                yield break;
            }
            item1.transform.position = Vector2.Lerp(pos1, pos2, elapsed / SWAP_ANIM_DURATION);
            item2.transform.position = Vector2.Lerp(pos2, pos1, elapsed / SWAP_ANIM_DURATION);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item1 != null) item1.transform.position = pos2; // ȷ������λ��
        if (item2 != null) item2.transform.position = pos1; // ȷ������λ��
    }

    IEnumerator SwapAndProcessMatches(Item item1, Item item2)
    {
        isBoardBusy = true;
        // Item tempSelectedItem = selectedItem; // ���ڴ���δƥ��ʱ�ָ�ѡ������߼�����ʱ��
        selectedItem = null; // ���ѡ�У���ֹ��������

        yield return StartCoroutine(AnimateSwap(item1, item2)); // �Ӿ�����
        SwapGridPositions(item1, item2); // ���ݽ���

        List<Item> matchedItems = FindAllMatches();
        if (matchedItems.Count > 0)
        {
            ProcessClearedItems(matchedItems.Count);
            yield return StartCoroutine(ProcessMatchesLoop(matchedItems));
        }
        else
        {
            Debug.Log("û��ƥ�䣬���ڽ���������");
            yield return StartCoroutine(AnimateSwap(item1, item2)); // �Ӿ���������
            SwapGridPositions(item1, item2); // ���ݽ�������
        }

        // ���û��ƥ�䣬selectedItem Ӧ��Ϊnull�����������ѡ��
        // �����Ҫ�ָ�֮ǰ��ѡ�������ֻ�����һ�Σ����δƥ����ָ������߼�������ӡ�
        // ��ǰ��Ϊ�������Ƿ�ƥ�䣬һ�ν������������selectedItem��
        selectedItem = null;

        isBoardBusy = false;
    }

    void ProcessClearedItems(int count)
    {
        currentScore += count * scorePerItem;
        itemsEliminatedForBulletConversion += count;

        Debug.Log($"������ {count} ��Ԫ��. ��ǰ����: {currentScore}. �ۼ�ת���ӵ�Ԫ��: {itemsEliminatedForBulletConversion}");
        UpdateScoreUI();

        if (itemsEliminatedForBulletConversion >= itemsToConvertForOneBullet)
        {
            int bulletsToAdd = itemsEliminatedForBulletConversion / itemsToConvertForOneBullet;

            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.AddBullets(bulletsToAdd);
            }
            else
            {
                Debug.LogWarning("PlayerDataManager.Instance δ�ҵ����޷������ӵ���");
            }

            itemsEliminatedForBulletConversion %= itemsToConvertForOneBullet;
            UpdateBulletUI();
        }

        if (currentScore >= targetScore)
        {
            WinGame();
        }
    }

    IEnumerator ProcessMatchesLoop(List<Item> initialMatchedItems)
    {
        List<Item> currentMatches = new List<Item>(initialMatchedItems);

        while (currentMatches.Count > 0)
        {
            ClearItems(currentMatches);

            yield return new WaitForSeconds(REFILL_DELAY);
            yield return StartCoroutine(CollapseColumns());
            yield return StartCoroutine(FillNewItems());
            yield return new WaitForSeconds(CASCADE_CHECK_DELAY);

            List<Item> newCascadeMatches = FindAllMatches();
            if (newCascadeMatches.Count > 0)
            {
                ProcessClearedItems(newCascadeMatches.Count);
            }
            currentMatches = newCascadeMatches;
        }
    }

    List<Item> FindAllMatches()
    {
        HashSet<Item> allMatches = new HashSet<Item>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == null) continue;
                Item.ItemType currentType = grid[x, y].type;

                // ˮƽ���
                if (x <= width - 3)
                {
                    if (grid[x + 1, y] != null && grid[x + 1, y].type == currentType &&
                        grid[x + 2, y] != null && grid[x + 2, y].type == currentType)
                    {
                        List<Item> horizontalRun = new List<Item> { grid[x, y], grid[x + 1, y], grid[x + 2, y] };
                        for (int i = x + 3; i < width; i++)
                        {
                            if (grid[i, y] != null && grid[i, y].type == currentType)
                                horizontalRun.Add(grid[i, y]);
                            else
                                break;
                        }
                        if (horizontalRun.Count >= 3) foreach (var item in horizontalRun) allMatches.Add(item);
                    }
                }

                // ��ֱ���
                if (y <= height - 3)
                {
                    if (grid[x, y + 1] != null && grid[x, y + 1].type == currentType &&
                        grid[x, y + 2] != null && grid[x, y + 2].type == currentType)
                    {
                        List<Item> verticalRun = new List<Item> { grid[x, y], grid[x, y + 1], grid[x, y + 2] };
                        for (int i = y + 3; i < height; i++)
                        {
                            if (grid[x, i] != null && grid[x, i].type == currentType)
                                verticalRun.Add(grid[x, i]);
                            else
                                break;
                        }
                        if (verticalRun.Count >= 3) foreach (var item in verticalRun) allMatches.Add(item);
                    }
                }
            }
        }
        return allMatches.ToList();
    }

    void ClearItems(List<Item> itemsToClear)
    {
        foreach (Item item in itemsToClear)
        {
            if (item != null)
            {
                // ȷ��Ҫɾ����itemȷʵ������Ķ�Ӧλ�ã���ֹ��ʱ�������õ��´���
                if (item.x >= 0 && item.x < width && item.y >= 0 && item.y < height && grid[item.x, item.y] == item)
                {
                    grid[item.x, item.y] = null;
                }
                // ��ѡ�����grid�ж�Ӧλ�ò������item��Ҳ����null������˵�����ݲ�һ��
                else if (item.x >= 0 && item.x < width && item.y >= 0 && item.y < height && grid[item.x, item.y] != null)
                {
                    Debug.LogWarning($"ClearItems: grid[{item.x},{item.y}] was not the item being destroyed ({item.name}), or item coordinates are stale. Item in grid: {grid[item.x, item.y]?.name}");
                }
                Destroy(item.gameObject);
            }
        }
    }

    void UpdateScoreUI()
    {
        if (scoreTextUI != null)
        {
            scoreTextUI.text = $"����: {currentScore} / {targetScore}";
        }
    }

    void UpdateBulletUI()
    {
        if (bulletCountTextUI != null && PlayerDataManager.Instance != null)
        {
            bulletCountTextUI.text = $"�ӵ�: {PlayerDataManager.Instance.currentBulletCount}";
        }
        else if (bulletCountTextUI != null)
        {
            bulletCountTextUI.text = "�ӵ�: N/A";
        }
    }

    void WinGame()
    {
        Debug.Log("�ﵽĿ���������Ϸʤ����");
        isBoardBusy = true;

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveDataToFile();
        }
        SceneManager.LoadScene(corridorSceneName);
    }

    public void OnQuitButtonClicked()
    {
        if (isBoardBusy && currentScore < targetScore)
        {
            Debug.Log("���̷�æ�����Ժ��˳���");
            return;
        }
        Debug.Log("�˳���ť����������س��ȡ�");

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveDataToFile();
        }
        SceneManager.LoadScene(corridorSceneName);
    }

    bool CausesInitialMatch(int x, int y, Item.ItemType type)
    {
        // ����������
        if (x >= 2 && grid[x - 1, y] != null && grid[x - 1, y].type == type &&
            grid[x - 2, y] != null && grid[x - 2, y].type == type)
        {
            return true;
        }
        // �����������
        if (y >= 2 && grid[x, y - 1] != null && grid[x, y - 1].type == type &&
            grid[x, y - 2] != null && grid[x, y - 2].type == type)
        {
            return true;
        }
        return false;
    }

    Item.ItemType GetRandomItemType()
    {
        int numTypes = System.Enum.GetValues(typeof(Item.ItemType)).Length;
        return (Item.ItemType)Random.Range(0, numTypes);
    }

    IEnumerator CollapseColumns()
    {
        List<Coroutine> activeFallingAnimations = new List<Coroutine>();
        for (int x = 0; x < width; x++)
        {
            int emptySpaces = 0;
            for (int y = 0; y < height; y++) // �������ϱ���
            {
                if (grid[x, y] == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    Item itemToMove = grid[x, y];
                    grid[x, y - emptySpaces] = itemToMove; // ������������
                    grid[x, y] = null; // ԭλ���ÿ�

                    itemToMove.y -= emptySpaces; // ����Item�����ڲ�������y����

                    // ʹ�� offsetX �� offsetY ����Ŀ���Ӿ�λ��
                    float targetVisualX = itemToMove.x * itemSpacing + offsetX;
                    float targetVisualY = itemToMove.y * itemSpacing + offsetY;
                    Vector2 targetPosition = new Vector2(targetVisualX, targetVisualY);

                    activeFallingAnimations.Add(StartCoroutine(MoveItemToPosition(itemToMove, targetPosition, FALL_ANIM_DURATION)));
                }
            }
        }
        // �ȴ��������䶯�����
        foreach (Coroutine anim in activeFallingAnimations)
        {
            if (anim != null) yield return anim;
        }
    }

    IEnumerator FillNewItems()
    {
        List<Coroutine> activeFallingAnimations = new List<Coroutine>();
        for (int x = 0; x < width; x++)
        {
            int newItemsInColumn = 0;
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    // ������Ԫ�ص�Ŀ���Ӿ�λ�� (Ӧ��offsetX, offsetY)
                    float targetVisualX = x * itemSpacing + offsetX;
                    float targetVisualY = y * itemSpacing + offsetY;
                    Vector2 targetPosition = new Vector2(targetVisualX, targetVisualY);

                    // �����ʼ�Ӿ�λ�� (��Ŀ��λ�õ����Ϸ���Ҳ��Ҫ����offsetX)
                    // ��������Ԫ�ش����̿ɼ������ͬһ�߶��Ϸ���ʼ���䣬�������д�
                    float initialYWorld = offsetY + (height - 1) * itemSpacing + itemSpacing; // �������Ԫ�ص�����Y + һ�����
                    initialYWorld += newItemsInColumn * (itemSpacing * 0.5f); // ͬһ�е���Ԫ����΢��һ���ʼ�߶ȣ���ֹͬʱ��������ȫ��ͬ��λ�ÿ�ʼ����
                    Vector2 initialPosition = new Vector2(targetVisualX, initialYWorld);

                    newItemsInColumn++;

                    GameObject newItemGO = Instantiate(itemPrefab, initialPosition, Quaternion.identity, this.transform);
                    newItemGO.name = $"��Ԫ�� ({x},{y})";

                    Item itemComponent = newItemGO.GetComponent<Item>();
                    Item.ItemType randomType;
                    int attempts = 0;
                    do
                    {
                        randomType = GetRandomItemType();
                        attempts++;
                        if (attempts > 100)
                        {
                            Debug.LogError($"FillNewItems: �޷�Ϊ({x},{y})�ҵ�һ��������ƥ������ͣ�");
                            break;
                        }
                    } while (CausesPotentialMatchWithNewItemFill(x, y, randomType));

                    itemComponent.Initialize(x, y, this, randomType); // x, y ������������
                    grid[x, y] = itemComponent;

                    activeFallingAnimations.Add(StartCoroutine(MoveItemToPosition(itemComponent, targetPosition, FALL_ANIM_DURATION)));
                }
            }
        }
        // �ȴ�������Ԫ�����䶯�����
        foreach (Coroutine anim in activeFallingAnimations)
        {
            if (anim != null) yield return anim;
        }
    }

    bool CausesPotentialMatchWithNewItemFill(int x, int y, Item.ItemType type)
    {
        // ��鴹ֱ���� (�·�����)
        if (y >= 2)
        {
            Item itemBelow1 = null;
            // �����ʱ��grid[x,y-1] Ӧ�����Ѿ���Ԫ���ˣ�������������ײ���������Ҳ�Ǹձ����ģ�
            // Ϊ�˸�׼ȷ������Ӧ��ֱ�Ӽ�� grid[x,y-1] �� grid[x,y-2]
            // ���� FillNewItems ��ѭ���У�grid[x,y-1] ���ܾ��ǵ�ǰ�����ж��Ƿ���������Ԫ�ص�λ�õ���һ��
            // ���ԣ�����ļ����Ҫ�Ƿ�ֹ��Ԫ�����º��������·��������Ѵ��ڵģ�������ģ�Ԫ���γ�������
            if (y - 1 >= 0) itemBelow1 = grid[x, y - 1]; // ��ȡ���·���Ԫ��
            Item itemBelow2 = null;
            if (y - 2 >= 0) itemBelow2 = grid[x, y - 2]; // ��ȡ���·���Ԫ��

            if (itemBelow1 != null && itemBelow1.type == type &&
                itemBelow2 != null && itemBelow2.type == type)
            {
                return true;
            }
        }

        // ���ˮƽ���� (���Ҹ�һ�����γ� X [��] X)
        if (x >= 1 && x < width - 1)
        { // ȷ�����Ҷ��и���
            Item itemLeft = grid[x - 1, y]; // ��ȡ��ߵ�Ԫ��
            Item itemRight = grid[x + 1, y]; // ��ȡ�ұߵ�Ԫ��
            if (itemLeft != null && itemLeft.type == type &&
                itemRight != null && itemRight.type == type)
            {
                return true;
            }
        }
        // ���ˮƽ���� (����������γ� X X [��])
        if (x >= 2)
        {
            Item itemLeft1 = grid[x - 1, y];
            Item itemLeft2 = grid[x - 2, y];
            if (itemLeft1 != null && itemLeft1.type == type &&
                itemLeft2 != null && itemLeft2.type == type)
            {
                return true;
            }
        }
        // ���ˮƽ���� (�ұ��������γ� [��] X X)
        if (x <= width - 3)
        {
            Item itemRight1 = grid[x + 1, y];
            Item itemRight2 = grid[x + 2, y];
            if (itemRight1 != null && itemRight1.type == type &&
                itemRight2 != null && itemRight2.type == type)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator MoveItemToPosition(Item item, Vector2 targetPosition, float duration)
    {
        if (item == null) yield break; // Ԫ�ؿ�����;������
        Vector2 startPosition = item.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (item == null)
            { // �ٴμ��
                Debug.LogWarning("MoveItemToPosition: Item was destroyed mid-animation.");
                yield break;
            }
            item.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item != null) item.transform.position = targetPosition; // ȷ������λ��
    }
}

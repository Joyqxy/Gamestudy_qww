using UnityEngine;
using UnityEngine.UI; // 需要UI命名空间
using UnityEngine.SceneManagement; // 需要场景管理命名空间
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

    [Header("游戏逻辑")]
    public int scorePerItem = 10;
    public int targetScore = 1000;
    public int itemsToConvertForOneBullet = 5;
    private int currentScore = 0;
    private int itemsEliminatedForBulletConversion = 0;

    [Header("UI引用")]
    public Text scoreTextUI;
    public Text bulletCountTextUI;

    [Header("场景跳转")]
    public string corridorSceneName = "_SceneLong"; // 请确保这是您长廊场景的正确名称

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
        // 计算整个棋盘视觉上的总宽度和总高度
        // 这是从第一个元素的中心到最后一个元素的中心的距离
        float boardVisualWidth = (width - 1) * itemSpacing;
        float boardVisualHeight = (height - 1) * itemSpacing;

        // 计算需要的偏移量，使棋盘的中心对准 (0,0)
        float offsetX = -boardVisualWidth / 2f;
        float offsetY = -boardVisualHeight / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 计算每个元素的X和Y坐标，并应用偏移量
                float itemXPos = x * itemSpacing + offsetX;
                float itemYPos = y * itemSpacing + offsetY;

                Vector2 position = new Vector2(itemXPos, itemYPos);

                // 实例化元素，并将其父对象设置为当前BoardManager所在的GameObject的transform
                // 这样，元素的位置就是相对于BoardManager的局部位置了
                GameObject newItemGO = Instantiate(itemPrefab, position, Quaternion.identity, this.transform);
                newItemGO.name = $"Item ({x},{y})";

                Item itemComponent = newItemGO.GetComponent<Item>();
                Item.ItemType randomType;

                do
                {
                    randomType = GetRandomItemType();
                    // CausesInitialMatch 依赖于 grid 数组中已经存在的元素
                    // 在棋盘完全生成前，对于边界附近的元素，这个检查可能不完整
                    // 但对于大部分内部元素是有效的。
                } while (CausesInitialMatch(x, y, randomType));

                itemComponent.Initialize(x, y, this, randomType); // Item内部的x,y仍然是网格索引
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
            if (selectedItem == item) return; // 点击了同一个，不做处理

            if (AreItemsAdjacent(selectedItem, item))
            {
                StartCoroutine(SwapAndProcessMatches(selectedItem, item));
            }
            else // 不相邻，则将新点击的设为选中
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
        // 交换网格中的引用
        grid[item1.x, item1.y] = item2;
        grid[item2.x, item2.y] = item1;

        // 交换Item对象内部记录的坐标
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
            { // 动画过程中可能被销毁
                Debug.LogWarning("AnimateSwap: Item was destroyed mid-animation.");
                yield break;
            }
            item1.transform.position = Vector2.Lerp(pos1, pos2, elapsed / SWAP_ANIM_DURATION);
            item2.transform.position = Vector2.Lerp(pos2, pos1, elapsed / SWAP_ANIM_DURATION);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item1 != null) item1.transform.position = pos2; // 确保最终位置
        if (item2 != null) item2.transform.position = pos1; // 确保最终位置
    }

    IEnumerator SwapAndProcessMatches(Item item1, Item item2)
    {
        isBoardBusy = true;
        // Item tempSelectedItem = selectedItem; // 用于处理未匹配时恢复选中项的逻辑，暂时简化
        selectedItem = null; // 清除选中，防止连续操作

        yield return StartCoroutine(AnimateSwap(item1, item2)); // 视觉交换
        SwapGridPositions(item1, item2); // 数据交换

        List<Item> matchedItems = FindAllMatches();
        if (matchedItems.Count > 0)
        {
            ProcessClearedItems(matchedItems.Count);
            yield return StartCoroutine(ProcessMatchesLoop(matchedItems));
        }
        else
        {
            Debug.Log("没有匹配，正在交换回来。");
            yield return StartCoroutine(AnimateSwap(item1, item2)); // 视觉交换回来
            SwapGridPositions(item1, item2); // 数据交换回来
        }

        // 如果没有匹配，selectedItem 应该为null，让玩家重新选择。
        // 如果需要恢复之前的选中项（比如只允许点一次，如果未匹配则恢复），逻辑会更复杂。
        // 当前简化为：无论是否匹配，一次交换操作后都清除selectedItem。
        selectedItem = null;

        isBoardBusy = false;
    }

    void ProcessClearedItems(int count)
    {
        currentScore += count * scorePerItem;
        itemsEliminatedForBulletConversion += count;

        Debug.Log($"消除了 {count} 个元素. 当前分数: {currentScore}. 累计转换子弹元素: {itemsEliminatedForBulletConversion}");
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
                Debug.LogWarning("PlayerDataManager.Instance 未找到，无法增加子弹！");
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

                // 水平检查
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

                // 垂直检查
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
                // 确保要删除的item确实在网格的对应位置，防止因时序或旧引用导致错误
                if (item.x >= 0 && item.x < width && item.y >= 0 && item.y < height && grid[item.x, item.y] == item)
                {
                    grid[item.x, item.y] = null;
                }
                // 可选：如果grid中对应位置不是这个item但也不是null，可能说明数据不一致
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
            scoreTextUI.text = $"分数: {currentScore} / {targetScore}";
        }
    }

    void UpdateBulletUI()
    {
        if (bulletCountTextUI != null && PlayerDataManager.Instance != null)
        {
            bulletCountTextUI.text = $"子弹: {PlayerDataManager.Instance.currentBulletCount}";
        }
        else if (bulletCountTextUI != null)
        {
            bulletCountTextUI.text = "子弹: N/A";
        }
    }

    void WinGame()
    {
        Debug.Log("达到目标分数！游戏胜利！");
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
            Debug.Log("棋盘繁忙，请稍后退出。");
            return;
        }
        Debug.Log("退出按钮被点击，返回长廊。");

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveDataToFile();
        }
        SceneManager.LoadScene(corridorSceneName);
    }

    bool CausesInitialMatch(int x, int y, Item.ItemType type)
    {
        // 检查左边两个
        if (x >= 2 && grid[x - 1, y] != null && grid[x - 1, y].type == type &&
            grid[x - 2, y] != null && grid[x - 2, y].type == type)
        {
            return true;
        }
        // 检查下面两个
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
            for (int y = 0; y < height; y++) // 从下往上遍历
            {
                if (grid[x, y] == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    Item itemToMove = grid[x, y];
                    grid[x, y - emptySpaces] = itemToMove; // 更新网格数据
                    grid[x, y] = null; // 原位置置空

                    itemToMove.y -= emptySpaces; // 更新Item对象内部的网格y坐标

                    // 使用 offsetX 和 offsetY 计算目标视觉位置
                    float targetVisualX = itemToMove.x * itemSpacing + offsetX;
                    float targetVisualY = itemToMove.y * itemSpacing + offsetY;
                    Vector2 targetPosition = new Vector2(targetVisualX, targetVisualY);

                    activeFallingAnimations.Add(StartCoroutine(MoveItemToPosition(itemToMove, targetPosition, FALL_ANIM_DURATION)));
                }
            }
        }
        // 等待所有下落动画完成
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
                    // 计算新元素的目标视觉位置 (应用offsetX, offsetY)
                    float targetVisualX = x * itemSpacing + offsetX;
                    float targetVisualY = y * itemSpacing + offsetY;
                    Vector2 targetPosition = new Vector2(targetVisualX, targetVisualY);

                    // 计算初始视觉位置 (在目标位置的正上方，也需要考虑offsetX)
                    // 让所有新元素从棋盘可见区域的同一高度上方开始下落，或者略有错开
                    float initialYWorld = offsetY + (height - 1) * itemSpacing + itemSpacing; // 棋盘最顶行元素的中心Y + 一个间距
                    initialYWorld += newItemsInColumn * (itemSpacing * 0.5f); // 同一列的新元素稍微错开一点初始高度，防止同时生成在完全相同的位置开始下落
                    Vector2 initialPosition = new Vector2(targetVisualX, initialYWorld);

                    newItemsInColumn++;

                    GameObject newItemGO = Instantiate(itemPrefab, initialPosition, Quaternion.identity, this.transform);
                    newItemGO.name = $"新元素 ({x},{y})";

                    Item itemComponent = newItemGO.GetComponent<Item>();
                    Item.ItemType randomType;
                    int attempts = 0;
                    do
                    {
                        randomType = GetRandomItemType();
                        attempts++;
                        if (attempts > 100)
                        {
                            Debug.LogError($"FillNewItems: 无法为({x},{y})找到一个不立即匹配的类型！");
                            break;
                        }
                    } while (CausesPotentialMatchWithNewItemFill(x, y, randomType));

                    itemComponent.Initialize(x, y, this, randomType); // x, y 仍是网格索引
                    grid[x, y] = itemComponent;

                    activeFallingAnimations.Add(StartCoroutine(MoveItemToPosition(itemComponent, targetPosition, FALL_ANIM_DURATION)));
                }
            }
        }
        // 等待所有新元素下落动画完成
        foreach (Coroutine anim in activeFallingAnimations)
        {
            if (anim != null) yield return anim;
        }
    }

    bool CausesPotentialMatchWithNewItemFill(int x, int y, Item.ItemType type)
    {
        // 检查垂直方向 (下方两个)
        if (y >= 2)
        {
            Item itemBelow1 = null;
            // 在填充时，grid[x,y-1] 应该是已经有元素了（除非是棋盘最底部，或者它也是刚被填充的）
            // 为了更准确，我们应该直接检查 grid[x,y-1] 和 grid[x,y-2]
            // 但在 FillNewItems 的循环中，grid[x,y-1] 可能就是当前正在判断是否能生成新元素的位置的下一行
            // 所以，这里的检查主要是防止新元素落下后，与它正下方的两个已存在的（或刚填充的）元素形成三连。
            if (y - 1 >= 0) itemBelow1 = grid[x, y - 1]; // 获取正下方的元素
            Item itemBelow2 = null;
            if (y - 2 >= 0) itemBelow2 = grid[x, y - 2]; // 获取下下方的元素

            if (itemBelow1 != null && itemBelow1.type == type &&
                itemBelow2 != null && itemBelow2.type == type)
            {
                return true;
            }
        }

        // 检查水平方向 (左右各一个，形成 X [新] X)
        if (x >= 1 && x < width - 1)
        { // 确保左右都有格子
            Item itemLeft = grid[x - 1, y]; // 获取左边的元素
            Item itemRight = grid[x + 1, y]; // 获取右边的元素
            if (itemLeft != null && itemLeft.type == type &&
                itemRight != null && itemRight.type == type)
            {
                return true;
            }
        }
        // 检查水平方向 (左边两个，形成 X X [新])
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
        // 检查水平方向 (右边两个，形成 [新] X X)
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
        if (item == null) yield break; // 元素可能中途被销毁
        Vector2 startPosition = item.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (item == null)
            { // 再次检查
                Debug.LogWarning("MoveItemToPosition: Item was destroyed mid-animation.");
                yield break;
            }
            item.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item != null) item.transform.position = targetPosition; // 确保最终位置
    }
}

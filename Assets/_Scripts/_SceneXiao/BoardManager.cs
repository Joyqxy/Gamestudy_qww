using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    [Header("Board Dimensions")]
    public int width = 8;
    public int height = 8;
    public GameObject itemPrefab;
    public float itemSpacing = 1.1f;

    [Header("Animation & Delays")]
    private const float SWAP_ANIM_DURATION = 0.2f;
    private const float FALL_ANIM_DURATION = 0.3f;
    private const float REFILL_DELAY = 0.3f;
    private const float CASCADE_CHECK_DELAY = 0.5f;

    [Header("Game Logic")]
    public int scorePerItem = 10;
    public int targetScore = 1000;
    [Tooltip("每消除多少个元素可以转换成1个子弹 (Defines how many items must be cleared to get 1 bullet)")]
    public int itemsToConvertForOneBullet = 10;

    [Header("UI References")]
    public Text scoreTextUI;
    public Text bulletCountTextUI;

    [Header("Scene Transition")]
    public string corridorSceneName = "_SceneLong";

    // Private State Variables
    private Item[,] grid;
    private Item selectedItem = null;
    private bool isBoardBusy = false;
    private float offsetX;
    private float offsetY;
    private int currentScore = 0;
    private int itemsEliminatedForBulletConversion = 0;

    void Start()
    {
        float boardVisualWidth = (width - 1) * itemSpacing;
        float boardVisualHeight = (height - 1) * itemSpacing;
        offsetX = -boardVisualWidth / 2f;
        offsetY = -boardVisualHeight / 2f;

        grid = new Item[width, height];
        StartCoroutine(InitializeBoardAndUI());
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

    IEnumerator SetupBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float itemXPos = x * itemSpacing + offsetX;
                float itemYPos = y * itemSpacing + offsetY;
                Vector2 position = new Vector2(itemXPos, itemYPos);

                GameObject newItemGO = Instantiate(itemPrefab, position, Quaternion.identity, this.transform);
                newItemGO.name = $"Item ({x},{y})";

                Item itemComponent = newItemGO.GetComponent<Item>();
                Item.ItemType randomType;

                do
                {
                    randomType = GetRandomItemType();
                } while (CausesInitialMatch(x, y, randomType));

                itemComponent.Initialize(x, y, this, randomType);
                grid[x, y] = itemComponent;
            }
        }
        yield return null;
    }

    public bool IsBoardBusy()
    {
        return isBoardBusy;
    }

    public void SelectItem(Item item)
    {
        if (isBoardBusy || item == null) return;

        if (selectedItem == null)
        {
            selectedItem = item;
        }
        else
        {
            if (selectedItem == item) return;

            if (AreItemsAdjacent(selectedItem, item))
            {
                StartCoroutine(SwapAndProcessMatches(selectedItem, item));
            }
            else
            {
                selectedItem = item;
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
        grid[item1.x, item1.y] = item2;
        grid[item2.x, item2.y] = item1;
        int tempX = item1.x; int tempY = item1.y;
        item1.x = item2.x; item1.y = item2.y;
        item2.x = tempX; item2.y = tempY;
    }

    IEnumerator AnimateSwap(Item item1, Item item2)
    {
        if (item1 == null || item2 == null) yield break;
        Vector2 pos1 = item1.transform.position;
        Vector2 pos2 = item2.transform.position;
        float elapsed = 0f;
        while (elapsed < SWAP_ANIM_DURATION)
        {
            if (item1 == null || item2 == null) yield break;
            item1.transform.position = Vector2.Lerp(pos1, pos2, elapsed / SWAP_ANIM_DURATION);
            item2.transform.position = Vector2.Lerp(pos2, pos1, elapsed / SWAP_ANIM_DURATION);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item1 != null) item1.transform.position = pos2;
        if (item2 != null) item2.transform.position = pos1;
    }

    IEnumerator SwapAndProcessMatches(Item item1, Item item2)
    {
        isBoardBusy = true;
        selectedItem = null;

        yield return StartCoroutine(AnimateSwap(item1, item2));
        SwapGridPositions(item1, item2);

        // 添加音效调用（玩家主动操作 → 传true）
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayEliminationSound(true);
        }

        List<Item> matchedItems = FindAllMatches();
        if (matchedItems.Count > 0)
        {
            ProcessClearedItems(matchedItems.Count);
            yield return StartCoroutine(ProcessMatchesLoop(matchedItems));
        }
        else
        {
            // 无匹配，交换回原位
            yield return StartCoroutine(AnimateSwap(item1, item2));
            SwapGridPositions(item1, item2);
        }

        selectedItem = null;
        isBoardBusy = false;
    }

    void ProcessClearedItems(int count)
    {
        currentScore += count * scorePerItem;
        itemsEliminatedForBulletConversion += count;

        UpdateScoreUI();
        CheckAndConvertBullets();

        if (currentScore >= targetScore)
        {
            WinGame();
        }
        
        // 新增：这里可以添加额外的连击逻辑（如果需要）
        // 例如：SoundManager.Instance.UpdateCombo(count);
    }

    IEnumerator ProcessMatchesLoop(List<Item> initialMatchedItems)
    {
        // 标记自动消除开始
        // 原代码问题行：
        // if (SoundManager.Instance != null && !initialMatchedItems.IsNullOrEmpty())

        // 修正后：
        if (SoundManager.Instance != null && initialMatchedItems != null && initialMatchedItems.Count > 0)
        {
            SoundManager.Instance.PlayEliminationSound(false);
        }

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
                // 不需要再次调用PlayEliminationSound，因为在ClearItems中已经调用
            }
            currentMatches = newCascadeMatches;
        }

        // 标记自动消除结束
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.MarkAutoMatchesFinished();
        }
    }

    void CheckAndConvertBullets()
    {
        if (itemsToConvertForOneBullet <= 0) return;

        if (itemsEliminatedForBulletConversion >= itemsToConvertForOneBullet)
        {
            int bulletsToAdd = itemsEliminatedForBulletConversion / itemsToConvertForOneBullet;

            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.AddBullets(bulletsToAdd);
            }
            else
            {
                Debug.LogError("[BoardManager] PlayerDataManager.Instance is NULL! Cannot add bullets.");
            }

            itemsEliminatedForBulletConversion %= itemsToConvertForOneBullet;
            UpdateBulletUI();
        }
    }

    void WinGame()
    {
        Debug.Log("Target score reached! Victory!");
        isBoardBusy = true;
        HandleExit();
    }

    public void OnQuitButtonClicked()
    {
        if (isBoardBusy && currentScore < targetScore)
        {
            Debug.Log("Board is busy, please wait to quit.");
            return;
        }
        Debug.Log("Quit button clicked, returning to corridor.");
        HandleExit();
    }

    private void HandleExit()
    {
        Debug.Log("[BoardManager] Performing final checks before exiting scene...");
        CheckAndConvertBullets();

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SaveDataToFile();
        }

        SceneManager.LoadScene(corridorSceneName);
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
                if (x <= width - 3)
                {
                    if (grid[x + 1, y] != null && grid[x + 1, y].type == currentType && grid[x + 2, y] != null && grid[x + 2, y].type == currentType)
                    {
                        List<Item> horizontalRun = new List<Item> { grid[x, y], grid[x + 1, y], grid[x + 2, y] };
                        for (int i = x + 3; i < width; i++)
                        {
                            if (grid[i, y] != null && grid[i, y].type == currentType) horizontalRun.Add(grid[i, y]);
                            else break;
                        }
                        if (horizontalRun.Count >= 3) foreach (var item in horizontalRun) allMatches.Add(item);
                    }
                }
                if (y <= height - 3)
                {
                    if (grid[x, y + 1] != null && grid[x, y + 1].type == currentType && grid[x, y + 2] != null && grid[x, y + 2].type == currentType)
                    {
                        List<Item> verticalRun = new List<Item> { grid[x, y], grid[x, y + 1], grid[x, y + 2] };
                        for (int i = y + 3; i < height; i++)
                        {
                            if (grid[x, i] != null && grid[x, i].type == currentType) verticalRun.Add(grid[x, i]);
                            else break;
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
                if (item.x >= 0 && item.x < width && item.y >= 0 && item.y < height && grid[item.x, item.y] == item)
                {
                    grid[item.x, item.y] = null;
                }

                // 添加音效调用（系统自动消除 → 传false）
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEliminationSound(false);
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

    bool CausesInitialMatch(int x, int y, Item.ItemType type)
    {
        if (x >= 2 && grid[x - 1, y] != null && grid[x - 1, y].type == type && grid[x - 2, y] != null && grid[x - 2, y].type == type) return true;
        if (y >= 2 && grid[x, y - 1] != null && grid[x, y - 1].type == type && grid[x, y - 2] != null && grid[x, y - 2].type == type) return true;
        return false;
    }

	public Item.ItemType GetRandomItemType()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		if (sceneName == "_SceneXiao_duanwu")
		{
			return (Item.ItemType)Random.Range(4, 8); // 端午：4~7
		}
		else if (sceneName == "_SceneXiao_zhongqiu")
		{
			return (Item.ItemType)Random.Range(8, 12); // 中秋：8~11
		}
		else if (sceneName == "_SceneXiao_yuanxiao") // 新增元宵场景
		{
			return (Item.ItemType)Random.Range(12, 16); // 元宵：12~15（左闭右开，故上限为16）
		}
		else
		{
			return (Item.ItemType)Random.Range(0, 4); // 原场景：0~3
		}
	}

	IEnumerator CollapseColumns()
    {
        List<Coroutine> activeFallingAnimations = new List<Coroutine>();
        for (int x = 0; x < width; x++)
        {
            int emptySpaces = 0;
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    Item itemToMove = grid[x, y];
                    grid[x, y - emptySpaces] = itemToMove;
                    grid[x, y] = null;
                    itemToMove.y -= emptySpaces;
                    float targetVisualX = itemToMove.x * itemSpacing + offsetX;
                    float targetVisualY = itemToMove.y * itemSpacing + offsetY;
                    Vector2 targetPosition = new Vector2(targetVisualX, targetVisualY);
                    activeFallingAnimations.Add(StartCoroutine(MoveItemToPosition(itemToMove, targetPosition, FALL_ANIM_DURATION)));
                }
            }
        }
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
                    float targetVisualX = x * itemSpacing + offsetX;
                    float targetVisualY = y * itemSpacing + offsetY;
                    Vector2 targetPosition = new Vector2(targetVisualX, targetVisualY);
                    float initialYWorld = offsetY + (height - 1) * itemSpacing + itemSpacing;
                    initialYWorld += newItemsInColumn * (itemSpacing * 0.5f);
                    Vector2 initialPosition = new Vector2(targetVisualX, initialYWorld);
                    newItemsInColumn++;
                    GameObject newItemGO = Instantiate(itemPrefab, initialPosition, Quaternion.identity, this.transform);
                    newItemGO.name = $"New Item ({x},{y})";
                    Item itemComponent = newItemGO.GetComponent<Item>();
                    Item.ItemType randomType;
                    int attempts = 0;
                    do
                    {
                        randomType = GetRandomItemType();
                        attempts++;
                        if (attempts > 100) { Debug.LogError($"FillNewItems: Could not find a non-matching type for ({x},{y})!"); break; }
                    } while (CausesPotentialMatchWithNewItemFill(x, y, randomType));
                    itemComponent.Initialize(x, y, this, randomType);
                    grid[x, y] = itemComponent;
                    activeFallingAnimations.Add(StartCoroutine(MoveItemToPosition(itemComponent, targetPosition, FALL_ANIM_DURATION)));
                }
            }
        }
        foreach (Coroutine anim in activeFallingAnimations)
        {
            if (anim != null) yield return anim;
        }
    }

    bool CausesPotentialMatchWithNewItemFill(int x, int y, Item.ItemType type)
    {
        if (y >= 2) { Item itemBelow1 = grid[x, y - 1]; Item itemBelow2 = grid[x, y - 2]; if (itemBelow1 != null && itemBelow1.type == type && itemBelow2 != null && itemBelow2.type == type) return true; }
        if (x >= 1 && x < width - 1) { Item itemLeft = grid[x - 1, y]; Item itemRight = grid[x + 1, y]; if (itemLeft != null && itemLeft.type == type && itemRight != null && itemRight.type == type) return true; }
        if (x >= 2) { Item itemLeft1 = grid[x - 1, y]; Item itemLeft2 = grid[x - 2, y]; if (itemLeft1 != null && itemLeft1.type == type && itemLeft2 != null && itemLeft2.type == type) return true; }
        if (x <= width - 3) { Item itemRight1 = grid[x + 1, y]; Item itemRight2 = grid[x + 2, y]; if (itemRight1 != null && itemRight1.type == type && itemRight2 != null && itemRight2.type == type) return true; }
        return false;
    }

    IEnumerator MoveItemToPosition(Item item, Vector2 targetPosition, float duration)
    {
        if (item == null) yield break;
        Vector2 startPosition = item.transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (item == null) yield break;
            item.transform.position = Vector2.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (item != null) item.transform.position = targetPosition;
    }
}

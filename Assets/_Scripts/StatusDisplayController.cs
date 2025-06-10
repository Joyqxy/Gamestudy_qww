using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Required for OnSceneLoaded

public class StatusDisplayController : MonoBehaviour
{
    public static StatusDisplayController Instance { get; private set; }

    public GameObject statusPanel; // Link this in the Inspector

    // Link these Text components in the Inspector
    public Text healthText;
    public Text bulletText;
    public Text specialItemText;
    public Text speedBoostText;
    public Text attackBoostText;
    public Text fireRateBoostText;
    // If using TextMeshPro, change type to TextMeshProUGUI

    private bool isPanelVisible = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this GameObject (and its children, including the Canvas if it's a child) persistent
            Debug.Log("[StatusDisplayController] Instance created and set to DontDestroyOnLoad.");
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"[StatusDisplayController] Duplicate instance found on {gameObject.name}. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        if (statusPanel != null)
        {
            statusPanel.SetActive(false);
            isPanelVisible = false; // Ensure visibility state matches actual state
        }
        else
        {
            Debug.LogError("[StatusDisplayController] StatusPanel is not assigned in the Inspector!");
        }
    }

    void OnEnable()
    {
        PlayerDataManager.OnPlayerDataUpdated += UpdateDisplay;
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event
        Debug.Log("[StatusDisplayController] Subscribed to OnPlayerDataUpdated and OnSceneLoaded.");
    }

    void OnDisable()
    {
        PlayerDataManager.OnPlayerDataUpdated -= UpdateDisplay;
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe
        Debug.Log("[StatusDisplayController] Unsubscribed from OnPlayerDataUpdated and OnSceneLoaded.");
    }

    void Start()
    {
        // Initial update if PlayerDataManager is already available
        if (PlayerDataManager.Instance != null)
        {
            UpdateDisplay();
        }
    }

    // Called when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[StatusDisplayController] Scene loaded: {scene.name}. Panel should be: {(isPanelVisible ? "Visible" : "Hidden")}");
        if (statusPanel != null)
        {
            statusPanel.SetActive(isPanelVisible); // Re-apply the visibility state
            if (isPanelVisible)
            {
                EnsureCanvasIsTopmostAndActive(); // Ensure its Canvas is properly set up
                UpdateDisplay();
            }
        }
    }

    void EnsureCanvasIsTopmostAndActive()
    {
        Canvas canvas = null;
        // Attempt to get the Canvas this StatusPanel is part of.
        // The StatusDisplayController script could be on the Canvas itself,
        // or on a manager object that has the Canvas pobreza a child,
        // or the statusPanel (which is a Panel) is a child of the Canvas.
        if (statusPanel != null)
        {
            canvas = statusPanel.GetComponentInParent<Canvas>();
        }

        if (canvas == null) // If not found in parent of statusPanel, check this object
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas == null && statusPanel != null) // If statusPanel is a child of this object, and this object isn't the canvas
        {
            canvas = GetComponentInChildren<Canvas>(); // This might get a child canvas, be careful
        }

        if (canvas != null)
        {
            // Ensure the Canvas itself is active if the panel is supposed to be visible
            if (!canvas.gameObject.activeInHierarchy && isPanelVisible)
            {
                // This case is tricky, if the canvas itself was deactivated, reactivating it here might be unexpected.
                // It's better to ensure the Canvas GameObject (that is DontDestroyOnLoad) is always active,
                // and only its child panel (statusPanel) is toggled.
                Debug.LogWarning("[StatusDisplayController] Parent Canvas was inactive while trying to show StatusPanel. Ensure the persistent Canvas GameObject is active.");
            }

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                canvas.sortingOrder = 100; // Set a high sorting order
                Debug.Log($"[StatusDisplayController] Ensured Canvas '{canvas.name}' sortingOrder is {canvas.sortingOrder}");
            }

            // Ensure an EventSystem exists in the scene for UI interaction
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                // Typically, you don't want to make the EventSystem DontDestroyOnLoad
                // as each scene might manage its own, or you might have one global one.
                // If this UI is the *only* UI that needs an event system and it's global,
                // then making a global EventSystem DontDestroyOnLoad might be an option.
                Debug.LogWarning("[StatusDisplayController] No EventSystem found. Created a new one. This might lead to multiple EventSystems if not managed carefully across scenes.");
            }
        }
        else
        {
            Debug.LogWarning("[StatusDisplayController] Could not find a Canvas associated with the StatusPanel or this GameObject to ensure it's topmost.");
        }
    }

    public void ToggleStatusPanel()
    {
        isPanelVisible = !isPanelVisible;
        if (statusPanel != null)
        {
            statusPanel.SetActive(isPanelVisible);
            if (isPanelVisible)
            {
                EnsureCanvasIsTopmostAndActive(); // Call this when making panel visible
                UpdateDisplay();
            }
        }
        Debug.Log($"[StatusDisplayController] Status panel visibility toggled to: {isPanelVisible}");
    }

    public void UpdateDisplay()
    {
        if (PlayerDataManager.Instance == null)
        {
            // Display N/A or default values if PlayerDataManager is not available
            if (healthText) healthText.text = "血量: N/A"; // UI Text string can be Chinese
            if (bulletText) bulletText.text = "子弹: N/A";
            if (specialItemText) specialItemText.text = "特殊道具: N/A";
            if (speedBoostText) speedBoostText.text = "移速提升: N/A";
            if (attackBoostText) attackBoostText.text = "攻击提升: N/A";
            if (fireRateBoostText) fireRateBoostText.text = "攻速提升: N/A";
            return;
        }

        // Only update text if the panel is active in hierarchy (meaning it's actually visible)
        if (statusPanel != null && statusPanel.activeInHierarchy)
        {
            // Debug.Log("[StatusDisplayController] Updating status display..."); // Can be spammy
            if (healthText != null)
                healthText.text = $"血量: {PlayerDataManager.Instance.currentPlayerHealth} / {PlayerDataManager.Instance.maxPlayerHealth}";

            if (bulletText != null)
                bulletText.text = $"子弹: {PlayerDataManager.Instance.currentBulletCount}";

            if (specialItemText != null)
                specialItemText.text = $"特殊道具: {PlayerDataManager.Instance.currentSpecialItemCount} / {PlayerDataManager.Instance.maxSpecialItemCount}";

            if (speedBoostText != null)
                speedBoostText.text = $"移速提升: {(PlayerDataManager.Instance.isSpeedBoostActive ? "激活" : "未激活")}";

            if (attackBoostText != null)
                attackBoostText.text = $"攻击提升: {(PlayerDataManager.Instance.isAttackBoostActive ? "激活" : "未激活")}";

            if (fireRateBoostText != null)
                fireRateBoostText.text = $"攻速提升: {(PlayerDataManager.Instance.isFireRateBoostActive ? "激活" : "未激活")}";
        }
    }
}

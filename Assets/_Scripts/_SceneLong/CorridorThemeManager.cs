using UnityEngine;

public class CorridorThemeManager : MonoBehaviour
{
    // Defines the visual themes available for the corridor.
    public enum FestivalTheme
    {
        Default,
        Pre_SpringFestival,
        Pre_DuanwuFestival,
        Pre_ZhongqiuFestival
    }

    [Header("Theme Elements")]
    [Tooltip("The main background sprite renderer to change color or sprite.")]
    public SpriteRenderer backgroundSprite;
    [Tooltip("Parent GameObject holding all decorations for Spring Festival.")]
    public GameObject springFestivalDecorations;
    [Tooltip("Parent GameObject holding all decorations for Duanwu Festival.")]
    public GameObject duanwuFestivalDecorations;
    [Tooltip("Parent GameObject holding all decorations for Zhongqiu Festival.")]
    public GameObject zhongqiuFestivalDecorations;
    [Tooltip("Optional: Parent GameObject for default/post-game decorations.")]
    public GameObject defaultDecorations;

    [Header("Theme Colors (Example)")]
    public Color defaultBgColor = Color.gray;
    public Color springBgColor = new Color(1f, 0.8f, 0.8f);
    public Color duanwuBgColor = new Color(0.8f, 1f, 0.8f);
    public Color zhongqiuBgColor = new Color(0.8f, 0.8f, 1f);

    /// <summary>
    /// Called when the script instance is being loaded.
    /// This is the primary logic trigger for setting the theme.
    /// </summary>
    void Start()
    {
        // On scene start, determine which theme to apply based on the global progression.
        if (PlayerDataManager.Instance != null)
        {
            Debug.Log($"[CorridorThemeManager] Checking progression. Upgrades acquired: {PlayerDataManager.Instance.upgradesAcquired}");
            ApplyThemeBasedOnProgression(PlayerDataManager.Instance.upgradesAcquired);
        }
        else
        {
            Debug.LogError("[CorridorThemeManager] PlayerDataManager.Instance not found! Cannot determine correct theme. Applying default theme.");
            ApplyTheme(FestivalTheme.Default);
        }
    }

    /// <summary>
    /// This section can be uncommented if you need direct keyboard controls for debugging purposes.
    /// Use Function Keys (F1-F4) to simulate different progression levels.
    /// </summary>
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) ApplyThemeBasedOnProgression(0); // Simulates 0 upgrades
        if (Input.GetKeyDown(KeyCode.F2)) ApplyThemeBasedOnProgression(1); // Simulates 1 upgrade
        if (Input.GetKeyDown(KeyCode.F3)) ApplyThemeBasedOnProgression(2); // Simulates 2 upgrades
        if (Input.GetKeyDown(KeyCode.F4)) ApplyThemeBasedOnProgression(3); // Simulates 3 upgrades
    }
    */

    /// <summary>
    /// Chooses the correct FestivalTheme based on the number of upgrades completed by the player.
    /// </summary>
    /// <param name="upgradesCompleted">The value from PlayerDataManager.Instance.upgradesAcquired.</param>
    public void ApplyThemeBasedOnProgression(int upgradesCompleted)
    {
        FestivalTheme themeToApply;
        switch (upgradesCompleted)
        {
            case 0:
                // No upgrades completed, game is at the beginning, preparing for Spring Festival.
                themeToApply = FestivalTheme.Pre_SpringFestival;
                break;

            case 1:
                // 1 upgrade complete (e.g., Spring Festival boss defeated), preparing for Duanwu Festival.
                themeToApply = FestivalTheme.Pre_DuanwuFestival;
                break;

            case 2:
                // 2 upgrades complete (e.g., Duanwu Festival boss defeated), preparing for Zhongqiu Festival.
                themeToApply = FestivalTheme.Pre_ZhongqiuFestival;
                break;

            default: // 3 or more upgrades completed
                // All major festivals are done, can show a default or celebratory theme.
                themeToApply = FestivalTheme.Default;
                break;
        }

        // Apply the determined theme.
        ApplyTheme(themeToApply);
    }

    /// <summary>
    /// Applies the visual changes for a given theme by activating/deactivating decoration GameObjects and changing colors.
    /// </summary>
    /// <param name="newTheme">The FestivalTheme to apply.</param>
    public void ApplyTheme(FestivalTheme newTheme)
    {
        Debug.Log($"[CorridorThemeManager] Applying visual theme: {newTheme}");

        // Deactivate all specific decorations first to reset the state.
        if (defaultDecorations != null) defaultDecorations.SetActive(false);
        if (springFestivalDecorations != null) springFestivalDecorations.SetActive(false);
        if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(false);
        if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(false);

        // Activate the correct one based on the theme.
        switch (newTheme)
        {
            case FestivalTheme.Default:
                if (backgroundSprite != null) backgroundSprite.color = defaultBgColor;
                if (defaultDecorations != null) defaultDecorations.SetActive(true);
                break;
            case FestivalTheme.Pre_SpringFestival:
                if (backgroundSprite != null) backgroundSprite.color = springBgColor;
                if (springFestivalDecorations != null) springFestivalDecorations.SetActive(true);
                break;
            case FestivalTheme.Pre_DuanwuFestival:
                if (backgroundSprite != null) backgroundSprite.color = duanwuBgColor;
                if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(true);
                break;
            case FestivalTheme.Pre_ZhongqiuFestival:
                if (backgroundSprite != null) backgroundSprite.color = zhongqiuBgColor;
                if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(true);
                break;
        }
    }
}

using UnityEngine;

public class CorridorThemeManager : MonoBehaviour
{
    public enum FestivalTheme
    {
        Default,
        Pre_SpringFestival,
        Pre_DuanwuFestival,
        Pre_ZhongqiuFestival
    }

    [Header("Theme Elements")]
    public SpriteRenderer backgroundSprite;
    public GameObject springFestivalDecorations;
    public GameObject duanwuFestivalDecorations;
    public GameObject zhongqiuFestivalDecorations;

    [Header("Theme Colors (Example)")]
    public Color defaultBgColor = Color.gray;
    public Color springBgColor = new Color(1f, 0.8f, 0.8f);
    public Color duanwuBgColor = new Color(0.8f, 1f, 0.8f);
    public Color zhongqiuBgColor = new Color(0.8f, 0.8f, 1f);

    void Start()
    {
        if (PlayerDataManager.Instance != null)
        {
            Debug.Log($"[CorridorThemeManager] Checking progression state: {PlayerDataManager.Instance.currentProgressionState}");
            ApplyThemeBasedOnProgression(PlayerDataManager.Instance.currentProgressionState);
        }
        else
        {
            Debug.LogError("[CorridorThemeManager] PlayerDataManager.Instance not found! Cannot determine correct theme. Applying default theme.");
            ApplyTheme(FestivalTheme.Default);
        }
    }
    
    // You can uncomment this section if you need keyboard controls for debugging purposes.
    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) ApplyThemeBasedOnProgression(GameProgressionState.Start);
        if (Input.GetKeyDown(KeyCode.F2)) ApplyThemeBasedOnProgression(GameProgressionState.SpringFestival_Complete);
        if (Input.GetKeyDown(KeyCode.F3)) ApplyThemeBasedOnProgression(GameProgressionState.DuanwuFestival_Complete);
        if (Input.GetKeyDown(KeyCode.F4)) ApplyThemeBasedOnProgression(GameProgressionState.ZhongqiuFestival_Complete);
    }
    */

    public void ApplyThemeBasedOnProgression(GameProgressionState progressionState)
    {
        FestivalTheme themeToApply;
        switch (progressionState)
        {
            case GameProgressionState.Start:
                themeToApply = FestivalTheme.Pre_SpringFestival;
                break;
            case GameProgressionState.SpringFestival_Complete:
                themeToApply = FestivalTheme.Pre_DuanwuFestival;
                break;
            case GameProgressionState.DuanwuFestival_Complete:
                themeToApply = FestivalTheme.Pre_ZhongqiuFestival;
                break;
            case GameProgressionState.ZhongqiuFestival_Complete:
                themeToApply = FestivalTheme.Default; // Or a special "Post-Game" theme
                break;
            default:
                themeToApply = FestivalTheme.Default;
                break;
        }
        ApplyTheme(themeToApply);
    }

    public void ApplyTheme(FestivalTheme newTheme)
    {
        Debug.Log($"[CorridorThemeManager] Applying visual theme: {newTheme}");

        // Deactivate all specific decorations first
        if (springFestivalDecorations != null) springFestivalDecorations.SetActive(false);
        if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(false);
        if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(false);

        // Activate the correct one
        switch (newTheme)
        {
            case FestivalTheme.Default:
                if (backgroundSprite != null) backgroundSprite.color = defaultBgColor;
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

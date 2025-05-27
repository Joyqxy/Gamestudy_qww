using UnityEngine;
using UnityEngine.UI; // 如果你想用UI按钮切换主题

public class CorridorThemeManager : MonoBehaviour
{
    public enum FestivalTheme
    {
        Default, // 默认/节气过渡时期
        Pre_SpringFestival,
        Pre_DuanwuFestival,
        Pre_ZhongqiuFestival
    }

    public FestivalTheme currentTheme = FestivalTheme.Default;

    // 示例：用于改变氛围的元素引用 (你需要根据实际场景元素来扩展)
    public SpriteRenderer backgroundSprite; // 长廊的背景Sprite
    public GameObject springFestivalDecorations; // 春节装饰物父对象
    public GameObject duanwuFestivalDecorations;   // 端午节装饰物父对象
    public GameObject zhongqiuFestivalDecorations; // 中秋节装饰物父对象

    // 示例颜色 (你可以用Sprite替换、材质替换等更复杂的方式)
    public Color defaultBgColor = Color.gray;
    public Color springBgColor = new Color(1f, 0.8f, 0.8f); // 淡红色
    public Color duanwuBgColor = new Color(0.8f, 1f, 0.8f); // 淡绿色
    public Color zhongqiuBgColor = new Color(0.8f, 0.8f, 1f); // 淡蓝色

    void Start()
    {
        // 初始化时隐藏所有特定节日装饰
        if (springFestivalDecorations != null) springFestivalDecorations.SetActive(false);
        if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(false);
        if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(false);

        ApplyTheme(currentTheme); // 应用初始主题
    }

    public void ApplyTheme(FestivalTheme newTheme)
    {
        currentTheme = newTheme;

        // 先隐藏所有节日特定装饰
        if (springFestivalDecorations != null) springFestivalDecorations.SetActive(false);
        if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(false);
        if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(false);

        switch (currentTheme)
        {
            case FestivalTheme.Default:
                if (backgroundSprite != null) backgroundSprite.color = defaultBgColor;
                // 激活默认装饰（如果有）
                break;
            case FestivalTheme.Pre_SpringFestival:
                if (backgroundSprite != null) backgroundSprite.color = springBgColor;
                if (springFestivalDecorations != null) springFestivalDecorations.SetActive(true);
                Debug.Log("应用春节前夕主题");
                break;
            case FestivalTheme.Pre_DuanwuFestival:
                if (backgroundSprite != null) backgroundSprite.color = duanwuBgColor;
                if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(true);
                Debug.Log("应用端午节前夕主题");
                break;
            case FestivalTheme.Pre_ZhongqiuFestival:
                if (backgroundSprite != null) backgroundSprite.color = zhongqiuBgColor;
                if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(true);
                Debug.Log("应用中秋节前夕主题");
                break;
        }
    }
 //   void Update()
// {
//     if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyTheme(FestivalTheme.Default);
//     if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyTheme(FestivalTheme.Pre_SpringFestival);
//     if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyTheme(FestivalTheme.Pre_DuanwuFestival);
//     if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyTheme(FestivalTheme.Pre_ZhongqiuFestival);
// }
    // --- 示例：用于通过UI按钮切换主题 (可选) ---
    // public void SetThemeToDefault() { ApplyTheme(FestivalTheme.Default); }
    // public void SetThemeToSpring() { ApplyTheme(FestivalTheme.Pre_SpringFestival); }
    // public void SetThemeToDuanwu() { ApplyTheme(FestivalTheme.Pre_DuanwuFestival); }
    // public void SetThemeToZhongqiu() { ApplyTheme(FestivalTheme.Pre_ZhongqiuFestival); }

    // --- 示例：用于通过代码切换主题 (比如游戏主逻辑调用) ---
    // public void ChangeToNextFestivalTheme(string festivalName) // festivalName 可以是 "Spring", "Duanwu", "Zhongqiu"
    // {
    //     if (festivalName == "Spring") ApplyTheme(FestivalTheme.Pre_SpringFestival);
    //     // ...
    // }
}

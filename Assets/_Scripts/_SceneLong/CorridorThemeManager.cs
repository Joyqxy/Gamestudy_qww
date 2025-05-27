using UnityEngine;
using UnityEngine.UI; // ���������UI��ť�л�����

public class CorridorThemeManager : MonoBehaviour
{
    public enum FestivalTheme
    {
        Default, // Ĭ��/��������ʱ��
        Pre_SpringFestival,
        Pre_DuanwuFestival,
        Pre_ZhongqiuFestival
    }

    public FestivalTheme currentTheme = FestivalTheme.Default;

    // ʾ�������ڸı��Χ��Ԫ������ (����Ҫ����ʵ�ʳ���Ԫ������չ)
    public SpriteRenderer backgroundSprite; // ���ȵı���Sprite
    public GameObject springFestivalDecorations; // ����װ���︸����
    public GameObject duanwuFestivalDecorations;   // �����װ���︸����
    public GameObject zhongqiuFestivalDecorations; // �����װ���︸����

    // ʾ����ɫ (�������Sprite�滻�������滻�ȸ����ӵķ�ʽ)
    public Color defaultBgColor = Color.gray;
    public Color springBgColor = new Color(1f, 0.8f, 0.8f); // ����ɫ
    public Color duanwuBgColor = new Color(0.8f, 1f, 0.8f); // ����ɫ
    public Color zhongqiuBgColor = new Color(0.8f, 0.8f, 1f); // ����ɫ

    void Start()
    {
        // ��ʼ��ʱ���������ض�����װ��
        if (springFestivalDecorations != null) springFestivalDecorations.SetActive(false);
        if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(false);
        if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(false);

        ApplyTheme(currentTheme); // Ӧ�ó�ʼ����
    }

    public void ApplyTheme(FestivalTheme newTheme)
    {
        currentTheme = newTheme;

        // ���������н����ض�װ��
        if (springFestivalDecorations != null) springFestivalDecorations.SetActive(false);
        if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(false);
        if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(false);

        switch (currentTheme)
        {
            case FestivalTheme.Default:
                if (backgroundSprite != null) backgroundSprite.color = defaultBgColor;
                // ����Ĭ��װ�Σ�����У�
                break;
            case FestivalTheme.Pre_SpringFestival:
                if (backgroundSprite != null) backgroundSprite.color = springBgColor;
                if (springFestivalDecorations != null) springFestivalDecorations.SetActive(true);
                Debug.Log("Ӧ�ô���ǰϦ����");
                break;
            case FestivalTheme.Pre_DuanwuFestival:
                if (backgroundSprite != null) backgroundSprite.color = duanwuBgColor;
                if (duanwuFestivalDecorations != null) duanwuFestivalDecorations.SetActive(true);
                Debug.Log("Ӧ�ö����ǰϦ����");
                break;
            case FestivalTheme.Pre_ZhongqiuFestival:
                if (backgroundSprite != null) backgroundSprite.color = zhongqiuBgColor;
                if (zhongqiuFestivalDecorations != null) zhongqiuFestivalDecorations.SetActive(true);
                Debug.Log("Ӧ�������ǰϦ����");
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
    // --- ʾ��������ͨ��UI��ť�л����� (��ѡ) ---
    // public void SetThemeToDefault() { ApplyTheme(FestivalTheme.Default); }
    // public void SetThemeToSpring() { ApplyTheme(FestivalTheme.Pre_SpringFestival); }
    // public void SetThemeToDuanwu() { ApplyTheme(FestivalTheme.Pre_DuanwuFestival); }
    // public void SetThemeToZhongqiu() { ApplyTheme(FestivalTheme.Pre_ZhongqiuFestival); }

    // --- ʾ��������ͨ�������л����� (������Ϸ���߼�����) ---
    // public void ChangeToNextFestivalTheme(string festivalName) // festivalName ������ "Spring", "Duanwu", "Zhongqiu"
    // {
    //     if (festivalName == "Spring") ApplyTheme(FestivalTheme.Pre_SpringFestival);
    //     // ...
    // }
}

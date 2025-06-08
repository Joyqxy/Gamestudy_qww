using UnityEngine;
using System.Collections;

public class TitleAnimator : MonoBehaviour
{
    public float shakeAmount = 2.5f; // 抖动的幅度 (像素单位)
    public float shakeSpeed = 10f;   // 抖动的速度 (每秒抖动频率)

    private RectTransform rectTransform;
    private Vector3 initialPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            initialPosition = rectTransform.anchoredPosition3D; // 获取UI元素的初始位置
        }
        else
        {
            Debug.LogError("TitleAnimator需要附加到有RectTransform的UI对象上!");
            enabled = false;
        }
    }

    void Update()
    {
        if (rectTransform != null)
        {
            // 使用Perlin Noise生成平滑的随机抖动偏移
            // Time.time * shakeSpeed 控制抖动的频率
            // shakeAmount 控制抖动的幅度
            float offsetX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0.0f) * 2.0f - 1.0f) * shakeAmount;
            float offsetY = (Mathf.PerlinNoise(0.0f, Time.time * shakeSpeed) * 2.0f - 1.0f) * shakeAmount;

            // 应用抖动偏移到初始位置
            rectTransform.anchoredPosition3D = initialPosition + new Vector3(offsetX, offsetY, 0);
        }
    }

    // 当对象被禁用或销毁时，恢复初始位置（可选，但良好实践）
    void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition3D = initialPosition;
        }
    }
}

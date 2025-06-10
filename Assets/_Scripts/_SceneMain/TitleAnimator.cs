using UnityEngine;
using System.Collections;

public class TitleAnimator : MonoBehaviour
{
    public float shakeAmount = 2.5f; // �����ķ��� (���ص�λ)
    public float shakeSpeed = 10f;   // �������ٶ� (ÿ�붶��Ƶ��)

    private RectTransform rectTransform;
    private Vector3 initialPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            initialPosition = rectTransform.anchoredPosition3D; // ��ȡUIԪ�صĳ�ʼλ��
        }
        else
        {
            Debug.LogError("TitleAnimator��Ҫ���ӵ���RectTransform��UI������!");
            enabled = false;
        }
    }

    void Update()
    {
        if (rectTransform != null)
        {
            // ʹ��Perlin Noise����ƽ�����������ƫ��
            // Time.time * shakeSpeed ���ƶ�����Ƶ��
            // shakeAmount ���ƶ����ķ���
            float offsetX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0.0f) * 2.0f - 1.0f) * shakeAmount;
            float offsetY = (Mathf.PerlinNoise(0.0f, Time.time * shakeSpeed) * 2.0f - 1.0f) * shakeAmount;

            // Ӧ�ö���ƫ�Ƶ���ʼλ��
            rectTransform.anchoredPosition3D = initialPosition + new Vector3(offsetX, offsetY, 0);
        }
    }

    // �����󱻽��û�����ʱ���ָ���ʼλ�ã���ѡ��������ʵ����
    void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition3D = initialPosition;
        }
    }
}

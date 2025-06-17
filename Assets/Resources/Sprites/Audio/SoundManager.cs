using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioClip[] eliminationSounds;
    public float pitchIncrement = 0.1f;
    public float maxPitch = 1.5f;
    public float comboTimeout = 2f;

    private AudioSource audioSource;
    private int comboCount = 0;
    private float comboTimer = 0f;
    private bool isPlayerChain = false;
    private bool isProcessingAutoMatches = false; // ����������Ƿ����ڴ����Զ�����

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.8f;
    }

    void Update()
    {
        if (comboCount > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboTimeout)
            {
                ResetCombo();
            }
        }
    }

    public void PlayEliminationSound(bool isPlayerAction)
    {
        // ��Ҳ�����������������״̬
        if (isPlayerAction)
        {
            ResetCombo();
            isPlayerChain = true;
            isProcessingAutoMatches = false;
            Debug.Log("[��Ч] ��Ҳ�����ʼ��������");
        }
        // �Զ�����������Ƿ���Ҫ��ʼ�µ��Զ�������
        else if (!isProcessingAutoMatches)
        {
            // ��ʼ�µ��Զ��������������������������������������ǣ�
            comboCount = 0;
            isProcessingAutoMatches = true;
            Debug.Log("[��Ч] ��ʼ�µ��Զ�����������");
        }

        comboCount++;
        comboTimer = 0f;

        float pitch = 1.0f + Mathf.Min(comboCount - 1, (maxPitch - 1f) / pitchIncrement) * pitchIncrement;
        audioSource.pitch = pitch;

        if (eliminationSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, eliminationSounds.Length);
            audioSource.PlayOneShot(eliminationSounds[randomIndex]);
            Debug.Log($"[��Ч] ������: {comboCount} | ����: {pitch:F2} | ��Ҵ���: {isPlayerAction}");
        }
    }

    // ����������Զ���������
    public void MarkAutoMatchesFinished()
    {
        isProcessingAutoMatches = false;
        Debug.Log("[��Ч] �Զ���������������");
    }

    private void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
        isPlayerChain = false;
        isProcessingAutoMatches = false;
    }
}
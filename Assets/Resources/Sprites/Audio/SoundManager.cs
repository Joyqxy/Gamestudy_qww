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
    private bool isProcessingAutoMatches = false; // 新增：标记是否正在处理自动消除

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
        // 玩家操作：重置所有连击状态
        if (isPlayerAction)
        {
            ResetCombo();
            isPlayerChain = true;
            isProcessingAutoMatches = false;
            Debug.Log("[音效] 玩家操作开始新连击链");
        }
        // 自动消除：检查是否需要开始新的自动连击链
        else if (!isProcessingAutoMatches)
        {
            // 开始新的自动消除连击链（重置音调，但保留玩家连击标记）
            comboCount = 0;
            isProcessingAutoMatches = true;
            Debug.Log("[音效] 开始新的自动消除连击链");
        }

        comboCount++;
        comboTimer = 0f;

        float pitch = 1.0f + Mathf.Min(comboCount - 1, (maxPitch - 1f) / pitchIncrement) * pitchIncrement;
        audioSource.pitch = pitch;

        if (eliminationSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, eliminationSounds.Length);
            audioSource.PlayOneShot(eliminationSounds[randomIndex]);
            Debug.Log($"[音效] 连击数: {comboCount} | 音调: {pitch:F2} | 玩家触发: {isPlayerAction}");
        }
    }

    // 新增：标记自动消除结束
    public void MarkAutoMatchesFinished()
    {
        isProcessingAutoMatches = false;
        Debug.Log("[音效] 自动消除连击链结束");
    }

    private void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
        isPlayerChain = false;
        isProcessingAutoMatches = false;
    }
}
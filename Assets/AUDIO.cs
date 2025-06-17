using UnityEngine;

public class MusicController : MonoBehaviour
{
    // ����һ��������AudioSource����
    public AudioSource musicSource;

    void Start()
    {
        // ��ȡ���
        musicSource = GetComponent<AudioSource>();
    }

    // �������ֵķ���
    public void PlayMusic()
    {
        musicSource.Play();
    }

    // ��ͣ���ֵķ���
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    // ֹͣ���ֵķ���
    public void StopMusic()
    {
        musicSource.Stop();
    }
}
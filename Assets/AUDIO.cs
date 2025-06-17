using UnityEngine;

public class MusicController : MonoBehaviour
{
    // 声明一个公共的AudioSource变量
    public AudioSource musicSource;

    void Start()
    {
        // 获取组件
        musicSource = GetComponent<AudioSource>();
    }

    // 播放音乐的方法
    public void PlayMusic()
    {
        musicSource.Play();
    }

    // 暂停音乐的方法
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    // 停止音乐的方法
    public void StopMusic()
    {
        musicSource.Stop();
    }
}
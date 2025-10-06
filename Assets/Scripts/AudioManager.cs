using UnityEngine;
using System.Collections;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("UI Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Game Sounds")]
    public AudioClip buySound;
    public AudioClip sellSound;
    public AudioClip eventSound;
    public AudioClip warningSound;

    [Header("Background Music")]
    public AudioClip mainMenuBGM;
    public AudioClip gameBGM;
    public AudioClip victoryBGM;
    public AudioClip defeatBGM;

    private AudioSource sfxSource;   // 用于短音效
    private AudioSource bgmSource;   // 用于背景音乐
    private Coroutine fadeCoroutine; // 用于淡入淡出

    void Awake()
    {
        // --- 单例模式 ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 自动补齐组件
            sfxSource = gameObject.AddComponent<AudioSource>();
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = 0.6f;  // 可调默认BGM音量
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 播放短音效（按钮、事件、购买等）
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // 播放BGM（自动淡入切换）
    public void PlayBGM(AudioClip newClip, float fadeTime = 1.5f)
    {
        if (newClip == null) return;

        if (bgmSource.clip == newClip) return; // 相同音乐不重复切换

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewBGM(newClip, fadeTime));
    }

    private IEnumerator FadeToNewBGM(AudioClip newClip, float fadeTime)
    {
        // 淡出旧音乐
        float startVolume = bgmSource.volume;
        //for (float t = 0; t < fadeTime; t += Time.deltaTime)
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // 淡入新音乐
        //for (float t = 0; t < fadeTime; t += Time.deltaTime)
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = startVolume;
    }

    // 停止BGM（可用于过场或静音）
    public void StopBGM(float fadeTime = 1f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        StartCoroutine(FadeOutBGM(fadeTime));
    }

    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
            yield return null;
        }
        bgmSource.Stop();
    }
}

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

    private AudioSource sfxSource;   // ���ڶ���Ч
    private AudioSource bgmSource;   // ���ڱ�������
    private Coroutine fadeCoroutine; // ���ڵ��뵭��

    void Awake()
    {
        // --- ����ģʽ ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // �Զ��������
            sfxSource = gameObject.AddComponent<AudioSource>();
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = 0.6f;  // �ɵ�Ĭ��BGM����
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ���Ŷ���Ч����ť���¼�������ȣ�
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // ����BGM���Զ������л���
    public void PlayBGM(AudioClip newClip, float fadeTime = 1.5f)
    {
        if (newClip == null) return;

        if (bgmSource.clip == newClip) return; // ��ͬ���ֲ��ظ��л�

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewBGM(newClip, fadeTime));
    }

    private IEnumerator FadeToNewBGM(AudioClip newClip, float fadeTime)
    {
        // ����������
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

        // ����������
        //for (float t = 0; t < fadeTime; t += Time.deltaTime)
        for (float t = 0; t < fadeTime; t += Time.unscaledDeltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, startVolume, t / fadeTime);
            yield return null;
        }
        bgmSource.volume = startVolume;
    }

    // ֹͣBGM�������ڹ���������
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

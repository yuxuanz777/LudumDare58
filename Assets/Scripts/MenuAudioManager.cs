using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    public static MenuAudioManager Instance;

    [Header("UI Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;


    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (!TryGetComponent(out audioSource))
            audioSource = gameObject.AddComponent<AudioSource>();

        // ��ѡ����ֹ�л�����ʱ����
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}

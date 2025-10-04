using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Logos")]
    [SerializeField] GameObject baseLogo;
    [SerializeField] GameObject ultimateLogo; // ¡°Ultimate Game Collector!¡± logo

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;

    void Awake()
    {
        if (ultimateLogo != null)
            ultimateLogo.SetActive(PlayerPrefs.GetInt(PersistentKeys.HasUltimateLogo, 0) == 1);

        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    void OnDestroy()
    {
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitClicked);
    }

    void OnStartClicked() => SceneLoader.LoadGame();
    void OnExitClicked() => SceneLoader.Quit();
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameIconSetter_Auto : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text tagText;  // 可选，如果想按Tag匹配

    [Header("Settings")]
    [Tooltip("在Resources文件夹下的子路径，例如 Resources/GameIcons/")]
    [SerializeField] private string resourceFolder = "GameIcons";

    [Tooltip("是否根据名字的第一个单词匹配图标，否则使用Tag")]
    [SerializeField] private bool useFirstWordOfName = true;

    void Start()
    {
        if (iconImage == null)
        {
            Debug.LogWarning($"{name}: Missing Icon Image reference!");
            return;
        }

        // 取关键字
        string key = "";
        if (useFirstWordOfName && nameText != null)
        {
            string[] parts = nameText.text.Split(' ');
            if (parts.Length > 0)
                key = parts[0];
        }
        else if (tagText != null)
        {
            key = tagText.text;
        }

        if (string.IsNullOrEmpty(key))
        {
            iconImage.enabled = false;
            return;
        }

        // 从 Resources/GameIcons/ 下加载同名 Sprite
        string path = $"{resourceFolder}/{key}";
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
        }
        else
        {
            Debug.LogWarning($"[GameIconSetter_Auto] 图标未找到: {path}");
            iconImage.enabled = false;
        }
    }
}

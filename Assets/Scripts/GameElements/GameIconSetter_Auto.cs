using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameIconSetter_Auto : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text tagText;  // ��ѡ������밴Tagƥ��

    [Header("Settings")]
    [Tooltip("��Resources�ļ����µ���·�������� Resources/GameIcons/")]
    [SerializeField] private string resourceFolder = "GameIcons";

    [Tooltip("�Ƿ�������ֵĵ�һ������ƥ��ͼ�꣬����ʹ��Tag")]
    [SerializeField] private bool useFirstWordOfName = true;

    void Start()
    {
        if (iconImage == null)
        {
            Debug.LogWarning($"{name}: Missing Icon Image reference!");
            return;
        }

        // ȡ�ؼ���
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

        // �� Resources/GameIcons/ �¼���ͬ�� Sprite
        string path = $"{resourceFolder}/{key}";
        Sprite sprite = Resources.Load<Sprite>(path);

        if (sprite != null)
        {
            iconImage.sprite = sprite;
            iconImage.enabled = true;
        }
        else
        {
            Debug.LogWarning($"[GameIconSetter_Auto] ͼ��δ�ҵ�: {path}");
            iconImage.enabled = false;
        }
    }
}

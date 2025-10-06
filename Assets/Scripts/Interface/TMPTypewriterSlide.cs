using System.Collections;
using UnityEngine;
using TMPro;

public class TMPTypewriterSlide : MonoBehaviour
{
    public TMP_Text textMeshPro;
    [Range(0.01f, 0.2f)] public float charInterval = 0.05f;
    public float slideDistance = 30f;  // ÿ����ĸ�������
    public float slideSpeed = 6f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private string fullText;

    void Start()
    {
        if (textMeshPro == null) textMeshPro = GetComponent<TMP_Text>();
        fullText = textMeshPro.text;
        textMeshPro.text = "";
        StartCoroutine(SlideInCharacters());
    }

    IEnumerator SlideInCharacters()
    {
        // ��ʼ��ÿ����ĸ�ĳ�ʼλ��
        textMeshPro.ForceMeshUpdate();
        var textInfo = textMeshPro.textInfo;

        textMeshPro.text = fullText;
        textMeshPro.ForceMeshUpdate();
        int totalChars = textInfo.characterCount;

        // ����������ĸ
        textMeshPro.maxVisibleCharacters = 0;

        for (int i = 0; i < totalChars; i++)
        {
            textMeshPro.maxVisibleCharacters = i + 1;
            yield return StartCoroutine(SlideLetter(i));
            yield return new WaitForSeconds(charInterval);
        }
    }

    IEnumerator SlideLetter(int index)
    {
        textMeshPro.ForceMeshUpdate();
        var textInfo = textMeshPro.textInfo;
        int matIndex = textInfo.characterInfo[index].materialReferenceIndex;
        int vertIndex = textInfo.characterInfo[index].vertexIndex;

        if (!textInfo.characterInfo[index].isVisible)
            yield break;

        Vector3[] vertices = textInfo.meshInfo[matIndex].vertices;
        Vector3 offset = new Vector3(0, -slideDistance, 0); // ���·�����
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            float eased = easeCurve.Evaluate(t);
            Vector3 currentOffset = Vector3.Lerp(offset, Vector3.zero, eased);

            for (int j = 0; j < 4; j++)
                vertices[vertIndex + j] = textInfo.characterInfo[index].bottomLeft + textInfo.characterInfo[index].vertex_BL.position + currentOffset;

            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }
    }
}

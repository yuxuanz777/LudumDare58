using UnityEngine;

// ���� ��ȫ��������壿 ���� //
public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(t.GetChild(i).gameObject);
        }
    }
}
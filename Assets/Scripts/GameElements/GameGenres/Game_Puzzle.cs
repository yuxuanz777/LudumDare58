using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Puzzle : GameItems
{
    public bool isRevealed;       // �Ƿ��ѽ���
    public int revealedTurn;      // �����Ļغ�
    public int finalValue;        // ȷ�������ʵ��ʼֵ

    public Game_Puzzle(string id, string name, int price, int baseValue)
        : base(id, name, price, baseValue, "Puzzle")
    {
        this.value = baseValue;
        this.isRevealed = false;
        this.finalValue = 0;
    }

    public override int GetCurrentValue(List<GameItems> library, int n = 0)
    {
        // ����δ��������ʾ���������������ֵ
        if (!isRevealed)
        {
            // ��ʾ����ʱ����α���ֵ���������Ӿ������ı�״̬��
            return Random.Range(0, 999);
        }

        // �������������ع̶�ֵ
        return finalValue;
    }

    // �� EndTurn ʱ���ã�ִ�н����߼�
    public void RevealValue()
    {
        if (isRevealed) return;

        isRevealed = true;
        revealedTurn = GameController.Instance._progress.CurrentTurn;

        // �����һ�� 10~1000 ��ֵ����ֵ���ʸ��ͣ�
        finalValue = GetWeightedRandomValue();
        value = finalValue; // ������������ֵ
        value = finalValue;
    }

    int GetWeightedRandomValue()
    {
        // ��Ȩ�ظ���ģ�ͣ�С���࣬����ϡ��
        float r = Random.value;
        if (r < 0.5f) return Random.Range(10, 200);
        if (r < 0.8f) return Random.Range(200, 500);
        if (r < 0.95f) return Random.Range(500, 800);
        return Random.Range(800, 1001); // ���ٳ��ָ߼�
    }
}

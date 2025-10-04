using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Simulation : GameItems
{
    public Game_Simulation(string id, string name, int price, int baseValue)
        : base(id, name, price, baseValue, "Simulation")
    {
        // Ԥ��ֵ���� LibraryService �� GameController ��ʼ����������������
    }

    // ����ͨ��Ϸһ�����ص�ǰʵ�� value
    public override int GetCurrentValue(List<GameItems> library, int n = 0)
    {
        return value;
    }

    // ��ѡ���ṩһ��ר�ýӿڹ� UI ��������ʾԤ��
    //public string GetPredictionDisplay()
    //{
    //    int predict = predictedDelta;
    //    string sign = predict >= 0 ? "+" : "";
    //    return $"{sign}{predict}";
    //}
}

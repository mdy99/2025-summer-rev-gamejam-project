using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarReinforceTracker : MonoBehaviour
{
    public static BarReinforceTracker Instance { get; private set; }

    private Dictionary<string, int> reinforceCounts = new Dictionary<string, int>();

    public TMP_Text reinforceText; // UI에 강화 횟수를 표시할 텍스트

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateReinforceInfo()
    {
        if (reinforceText == null) return;

        string info = "[강화 현황]\n";

        info += FormatLine("HP", new int[] { 20, 15, 10, 5, 5 });
        info += FormatLine("MP", new int[] { 15, 10, 8, 5, 3 });
        info += FormatLine("Regen", new int[] { 5, 4, 2, 1, 1 });

        reinforceText.text = info;
    }
    
    private string FormatLine(string type, int[] curve)
    {
        int count = GetReinforceCount(type);
        bool maxed = count >= curve.Length;
        string valuePart = maxed ? "→ 최대 강화 도달" : $"→ 다음 강화 시 +{curve[count]}";
        return $"{type}: 횟수 {count}회 {valuePart}\n";
    }

    public int GetReinforceCount(string type)
    {
        return reinforceCounts.TryGetValue(type, out int count) ? count : 0;
    }


    public (int hpUp, int mpUp, int mpRegenUp) ReinforceBar(string type)
    {
        if (!reinforceCounts.ContainsKey(type))
            reinforceCounts[type] = 0;

        int count = reinforceCounts[type];
        reinforceCounts[type]++;

        // 커브 테이블 예시
        int[] hpCurve = { 20, 15, 10, 5, 5 };
        int[] mpCurve = { 15, 10, 8, 5, 3 };
        int[] regenCurve = { 5, 4, 2, 1, 1 };

        int hp = type == "HP" ? GetValue(hpCurve, count) : 0;
        int mp = type == "MP" ? GetValue(mpCurve, count) : 0;
        int regen = type == "Regen" ? GetValue(regenCurve, count) : 0;

        return (hp, mp, regen);
    }

    private int GetValue(int[] curve, int count)
    {
        return count < curve.Length ? curve[count] : curve[curve.Length - 1];
    }

    public void ResetTracker()
    {
        reinforceCounts.Clear();
    }
}

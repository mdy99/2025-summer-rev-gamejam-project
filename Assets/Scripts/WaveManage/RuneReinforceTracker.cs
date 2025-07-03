using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuneReinforceTracker : MonoBehaviour
{
    public static RuneReinforceTracker Instance { get; private set; }

    [SerializeField] private TMP_Text reinforceText; // UI에 강화 횟수를 표시할 텍스트

    private Dictionary<string, int> runeReinforceCounts = new Dictionary<string, int>();

    public void UpdateReinforceInfo()
{
    if (reinforceText == null) return;

    string info = "[룬 강화 현황]\n";

    int[] damageCurve = { 10, 8, 6, 5, 4 };
    int[] mpCostCurve = { -5, -4, -3, -2, -1 };

    foreach (var kvp in runeReinforceCounts)
    {
        string rune = kvp.Key;
        int count = kvp.Value;

        bool isMaxed = count >= damageCurve.Length;
        string line;

        if (isMaxed)
        {
            line = $"{rune}: 횟수 {count}회 → 최대 강화 도달";
        }
        else
        {
            int damage = damageCurve[count];
            int mpCost = mpCostCurve[count];
            line = $"{rune}: 횟수 {count}회 → 다음 강화 시 +{damage} / 마나 {mpCost}";
        }

        info += line + "\n";
    }

    reinforceText.text = info;
}

public (int count, int? nextDamage, int? nextMpCost, bool isMaxed) GetNextReinforceInfo(string runeCode)
{
    int[] damageCurve = { 10, 8, 6, 5, 4 };
    int[] mpCostCurve = { -5, -4, -3, -2, -1 };

    int count = GetReinforceCount(runeCode);
    bool isMaxed = count >= damageCurve.Length;

    int? nextDamage = isMaxed ? null : damageCurve[count];
    int? nextMpCost = isMaxed ? null : mpCostCurve[count];

    return (count, nextDamage, nextMpCost, isMaxed);
}



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 해당 룬을 몇 번 강화했는지 반환
    /// </summary>
    public int GetReinforceCount(string runeCode)
    {
        return runeReinforceCounts.TryGetValue(runeCode, out int count) ? count : 0;
    }

    /// <summary>
    /// 해당 룬을 강화하고 강화 수치(damage/mpCost)를 반환
    /// </summary>
    public (int damage, int mpCost) ReinforceRune(string runeCode)
    {
        int count = GetReinforceCount(runeCode);

        int[] damageCurve = { 10, 8, 6, 5, 4 };
        int damage = count < damageCurve.Length ? damageCurve[count] : 0;
        int[] mpCostCurve = { -5, -4, -3, -2, -1 };
        int mpCost = count < mpCostCurve.Length ? mpCostCurve[count] : 0;

        // 강화 횟수 증가
        runeReinforceCounts[runeCode] = count + 1;

        return (damage, mpCost);
    }

    /// <summary>
    /// 전체 초기화 (ex. 웨이브 시작 시)
    /// </summary>
    public void ResetTracker()
    {
        runeReinforceCounts.Clear();
    }
}

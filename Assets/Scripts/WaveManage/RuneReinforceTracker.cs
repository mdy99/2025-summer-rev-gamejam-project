using System.Collections.Generic;
using UnityEngine;

public class RuneReinforceTracker : MonoBehaviour
{
    public static RuneReinforceTracker Instance { get; private set; }

    private Dictionary<string, int> runeReinforceCounts = new Dictionary<string, int>();

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    [Header("Rune Database")]
    [SerializeField] private RuneInfoDatabase runeInfoDatabase; // 룬 정보 데이터베이스

    private Dictionary<string, RuneInfo> runeDictionary; // 룬 정보를 저장할 딕셔너리
    private void Awake()
    {
        runeDictionary = new Dictionary<string, RuneInfo>();
        foreach(var rune in runeInfoDatabase.runes)
        {
            runeDictionary[rune.runeCode] = rune; // 룬 코드로 룬 정보를 매핑
        }
    }

    public void ReinforceRune(string runeCode, int damageValue, int mpCostValue)
    {
        if (runeDictionary.TryGetValue(runeCode, out RuneInfo runeInfo))
        {
            runeInfo.AddRuneInfo(damageValue, mpCostValue); // 룬 정보 강화
            Debug.Log($"Reinforced {runeCode}: Damage +{damageValue}, MP Cost {mpCostValue}");
        }
        else
        {
            Debug.LogWarning($"Rune {runeCode} not found in the database.");
        }
    }

    public (int totalDamage, int totalMpCost) GetRuneStats(List<string> runeCodes)
    {
        int totalDamage = 0;
        int totalMpCost = 0;

        foreach (var runeCode in runeCodes)
        {
            if (runeDictionary.TryGetValue(runeCode, out RuneInfo runeInfo))
            {
                totalDamage += runeInfo.damage; // 룬의 데미지를 총 데미지에 더함
                totalMpCost += runeInfo.mpCost; // 룬의 마나 소모를 총 마나 소모에 더함
            }
            else
            {
                Debug.LogWarning($"Rune {runeCode} not found in the database.");
            }
        }
        return (totalDamage, totalMpCost);
    }
}

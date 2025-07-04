using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneManager : MonoBehaviour
{
    public static RuneManager Instance { get; private set; } // 싱글톤 인스턴스

    public delegate void RuneChanged();
    public event RuneChanged OnRuneChanged;

    public void NotifyRuneChanged()
{
    OnRuneChanged?.Invoke(); // 내부에서만 호출 가능
}


    [Header("Rune Database")]
    [SerializeField] private RuneInfoDatabase runeInfoDatabase; // 룬 정보 데이터베이스

    private Dictionary<string, RuneInfo> runeDictionary; // 룬 정보를 저장할 딕셔너리

    public int GetDamage(string runeCode){
        if (runeDictionary.TryGetValue(runeCode, out RuneInfo runeInfo))
        {
            return runeInfo.damage; // 룬의 데미지 반환
        }
        else
        {
            Debug.LogWarning($"Rune {runeCode} not found in the database.");
            return 0; // 룬이 없으면 0 반환
        }
    }

    public int GetMpCost(string runeCode){
        if (runeDictionary.TryGetValue(runeCode, out RuneInfo runeInfo))
        {
            return runeInfo.mpCost; // 룬의 마나 소모 반환
        }
        else
        {
            Debug.LogWarning($"Rune {runeCode} not found in the database.");
            return 0; // 룬이 없으면 0 반환
        }
    }

    private void Awake()
    {
            if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
    }
    else
    {
        Destroy(gameObject); // 중복 방지
    }
        runeDictionary = new Dictionary<string, RuneInfo>();
        foreach(var rune in runeInfoDatabase.runes)
        {
            rune.CacheInitialValues();
            rune.ResetToInitialValues();
            runeDictionary[rune.runeCode] = rune; // 룬 코드로 룬 정보를 매핑
        }
    }

    public void ResetRuneDatabase()
{
    runeInfoDatabase.ResetDatabase(); // ScriptableObject 상태 복원
    NotifyRuneChanged(); // UI 갱신 트리거
}


    public void ReinforceRune(string runeCode, int damageValue, int mpCostValue)
    {
        if (runeDictionary.TryGetValue(runeCode, out RuneInfo runeInfo))
        {
            runeInfo.AddRuneInfo(damageValue, mpCostValue); // 룬 정보 강화
            OnRuneChanged?.Invoke(); // UI 갱신 알림 추가
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

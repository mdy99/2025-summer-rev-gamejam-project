using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 스킬 타입을 정의하는 열거형
public enum SkillType
{
    None, // 스킬 없음
    NOVICE, // 기초 스킬
    APPRENTICE, // 중급 스킬
    ADEPT // 고급 스킬
}

public enum SkillCategory
{
    None, // 카테고리 없음
    Projectile, // 투사체 스킬
    InstantRectangle, // 직선형 즉시 스킬
    InstantCircle, // 원형 즉시 스킬
}

[CreateAssetMenu(menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string runeCode; // 룬 코드
    public GameObject prefab; // 스킬 프리팹
    public SkillType skillType; // 스킬 타입
    public SkillCategory skillCategory; // 스킬 카테고리
    public string SkillName; // 스킬 이름
    public string Description; // 스킬 설명

    [HideInInspector] public int mpCost; // 마나 소모량
    [HideInInspector] public int damage; // 스킬 피해량

    public float speed; // 스킬 속도
    public float aoeRadius; // 범위 공격 반경

    public void InitializeFromRunes(Dictionary<string, RuneInfo> runeInfoMap)
    {
        mpCost = 0; // 초기화
        damage = 0; // 초기화

        string[] runes = runeCode.Select(c => c.ToString()).ToArray(); // 룬 코드를 문자열 배열로 변환
        foreach (string rune in runes){
            if(runeInfoMap.TryGetValue(rune, out RuneInfo info)){
                mpCost += info.mpCost; // 룬의 마나 소모량을 누적
                damage += info.damage; // 룬의 피해량을 누적
            } else {
                Debug.LogWarning($"Rune {rune} not found in runeInfoMap."); // 룬 정보가 없을 경우 경고 로그 출력
            }
        }
    }

    public int GetTotalMpCost(){
        List<string> runeList = new List<string>();
        foreach(char rune in runeCode){
            runeList.Add(rune.ToString());
        }
        return RuneManager.Instance.GetRuneStats(runeList).totalMpCost;
    }

    public int GetTotalDamage(){
        List<string> runeList = runeCode.Select(c=>c.ToString()).ToList();
        return RuneManager.Instance.GetRuneStats(runeList).totalDamage;
    }
}

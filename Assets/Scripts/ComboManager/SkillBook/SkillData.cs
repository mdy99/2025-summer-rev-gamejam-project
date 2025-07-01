using System.Collections;
using System.Collections.Generic;
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
    Projectile // 투사체 스킬
    // 추가 카테고리 정의
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
    public int mpCost; // 마나 소모량
    public float speed; // 스킬 속도
    public int damage; // 스킬 피해량
    public float aoeRadius; // 범위 공격 반경
}

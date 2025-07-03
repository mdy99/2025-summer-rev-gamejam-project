using System;
using UnityEngine;

public class InstantCircleSkill : ISkill
{
    private SkillData skillData; // 스킬 데이터
    private Func<Vector3> firePointProvider; // 스킬 발사 위치 제공 함수

    public InstantCircleSkill(SkillData data, Func<Vector3> firePointProvider)
    {
        this.skillData = data; // 스킬 데이터 초기화
        this.firePointProvider = firePointProvider; // 스킬 발사 위치 제공 함수 초기화
    }

    public void Execute()
    {
        if (skillData.mpCost > BarManager.Instance.Mp) // 현재 마나가 부족한 경우
        {
            Debug.LogWarning("Not enough MP to execute skill: " + skillData.runeCode);
            NarrationText.Instance.UpdateNarration("마나가 부족합니다.",Color.gray); // 마나 부족 메시지 표시
            return; // 마나가 부족하면 스킬 실행 중단
        }
        BarManager.Instance.UpdateMpBar(-skillData.mpCost); // 마나 UI 바 업데이트
        Debug.Log($"Instant Circle Skill Executed: {skillData.runeCode}"); // 스킬

        Vector3 center = firePointProvider(); // 스킬 발사 위치 가져오기
        GameObject aoeObject = GameObject.Instantiate(skillData.prefab, center, Quaternion.identity); // 스킬 프리팹 생성
        aoeObject.GetComponent<InstantCircleAOE>().Initialize(skillData.damage, skillData.aoeRadius); // 스킬 초기화
    }   
}
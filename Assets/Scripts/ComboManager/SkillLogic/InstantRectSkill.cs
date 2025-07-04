using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRectSkill : ISkill
{
    private SkillData skillData; // 스킬 데이터
    private Func<Vector3> firePointProvider; // 스킬 발사 위치 제공 함수

    public InstantRectSkill(SkillData data , Func<Vector3> firePointProvider)
    {
        this.skillData = data; // 스킬 데이터 초기화
        this.firePointProvider = firePointProvider; // 스킬 발사 위치 제공 함수 초기화
    }

    public void Execute(){
        int totalMp = skillData.GetTotalMpCost();
        if (totalMp > BarManager.Instance.Mp) // 현재 마나가 부족한 경우
        {
            Debug.LogWarning("Not enough MP to execute skill: " + skillData.runeCode);
            NarrationText.Instance.UpdateNarration("마나가 부족합니다.",Color.gray); // 마나 부족 메시지 표시
            return; // 마나가 부족하면 스킬 실행 중단
        }

        BarManager.Instance.UpdateMpBar(-totalMp); // 마나 UI 바 업데이트
        Debug.Log($"Instant Rect Skill Executed: {skillData.runeCode}"); // 스킬 실행 로그 출력

        Vector3 center = firePointProvider(); // 스킬 발사 위치 가져오기
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 월드 좌표로 변환
        mouse.z = 0f; // z축 위치를 0으로 설정 (2D 게임에서 z축은 사용하지 않음)
        Vector2 dir = (mouse - center).normalized; // 마우스 위치를 기준으로 방향 계산

        float spawnOffset = 11f; // 스킬 발사 위치 오프셋
        Vector3 spawnPos = center + (Vector3)(dir * spawnOffset); // 방향에 맞게 오프셋 적용

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // 방향을 각도로 변환
        Quaternion rotation = Quaternion.Euler(0, 0, angle+90f); // 방향에 맞게 회전 (90도 회전)

        GameObject aoeObject = GameObject.Instantiate(skillData.prefab, spawnPos, rotation); // 스킬 프리팹 생성        
        int totalDamage = skillData.GetTotalDamage();
        aoeObject.GetComponent<InstantRectAOE>().Initialize(totalDamage); // 스킬 초기화
    }
}
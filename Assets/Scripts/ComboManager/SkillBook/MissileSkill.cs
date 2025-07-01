using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSkill : ISkill
{
    private SkillData data; // 스킬 데이터
    private Func<Vector3> firePointProvider; // 미사일 발사 위치

    public MissileSkill(SkillData data,Func<Vector3> firePointProvider)
    {
        this.data = data; // 스킬 데이터 초기화
        this.firePointProvider = firePointProvider; // 미사일 발사 위치 초기화
    }

    public void Execute()
    {
        Debug.Log($"Missile Skill Executed: {data.runeCode}"); // 스킬 실행 로그 출력
        // 미사일 스킬 실행 로직
        Vector3 firePoisiton = firePointProvider(); // 현재 플레이어 위치를 기준으로 미사일 발사 위치를 가져옴
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - firePoisiton).normalized; // 마우스 위치를 기준으로 방향 계산

        GameObject missile = GameObject.Instantiate(data.prefab, firePoisiton, Quaternion.identity); // 미사일 생성
        missile.GetComponent<MissileSkillProjectile>().Initialize(dir); // 미사일 초기화
        BarManager.Instance.UpdateMpBar(-data.mpCost); // 마나 UI 바 업데이트
    }
}

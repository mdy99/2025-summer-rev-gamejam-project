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
        firePoisiton.z = 0f; // z축 위치를 0으로 설정 (2D 게임에서 z축은 사용하지 않음)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 월드 좌표로 변환
        mousePosition.z = 0f; // z축 위치를 0으로 설정 (2D 게임에서 z축은 사용하지 않음)
        Vector2 dir = (mousePosition - firePoisiton).normalized; // 마우스 위치를 기준으로 방향 계산

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // 방향을 각도로 변환
        //Quaternion rotation = (dir.x < 0) ? Quaternion.Euler(0, 0, angle + 180f) : Quaternion.Euler(0, 0, angle); // 방향에 맞게 회전
        Quaternion rotation = Quaternion.Euler(0, 0, angle + 180f); // 방향에 맞게 회전

        GameObject missile = GameObject.Instantiate(data.prefab, firePoisiton, rotation); // 미사일 생성

        missile.GetComponent<MissileSkillProjectile>().Initialize(dir); // 미사일 초기화
        BarManager.Instance.UpdateMpBar(-data.mpCost); // 마나 UI 바 업데이트
    }
}

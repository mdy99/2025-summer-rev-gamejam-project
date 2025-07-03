using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
        public static BarManager Instance { get; private set; }

        public GameObject hpBar; // 플레이어의 체력을 표시하는 UI 바 오브젝트
        public GameObject mpBar; // 플레이어의 마나를 표시하는 UI 바 오브젝트
        public int Hp => hpBar.GetComponent<BarUpdater>().CurBarValue; // 현재 플레이어의 체력 값
        public int Mp => mpBar.GetComponent<BarUpdater>().CurBarValue; // 현재
        public int MaxHp => hpBar.GetComponent<BarUpdater>().MaxBarValue; // 플레이어의 최대 체력 값
        public int MaxMp => mpBar.GetComponent<BarUpdater>().MaxBarValue; //
        public int MpRegen => mpBar.GetComponent<BarUpdater>().getMpRegenValue(); // 현재 플레이어의 마나 재생 속도 값

    void Awake()
    {
        if(Instance ==null) Instance = this; // 싱글톤 인스턴스 설정
        else Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 오브젝트 제거
    }

    void Start()
    {
        if (hpBar == null || mpBar == null)
        {
            Debug.LogError("BarManager: hpBar or mpBar is not assigned!"); // hpBar 또는 mpBar가 할당되지 않은 경우 에러 메시지 출력
            return;
        }
    }

    public void UpdateHpBar(int value){
        hpBar.GetComponent<BarUpdater>().UpdateBar(value); // hpBar의 현재 값을 10만큼 증가
    }

    public void UpdateMpBar(int value){
        mpBar.GetComponent<BarUpdater>().UpdateBar(value); // mpBar의 현재 값을 10만큼 증가
    }

    public void UpdateMaxHpBar(int value){
        hpBar.GetComponent<BarUpdater>().UpdateMaxBar(value); // hpBar의 최대 값을 10만큼 증가
        hpBar.GetComponent<BarUpdater>().SmoothIncreaseBackBar(); // hpBar의 배경 바를 부드럽게 증가
    }
    public void UpdateMaxMpBar(int value){
        mpBar.GetComponent<BarUpdater>().UpdateMaxBar(value); // mpBar의 최대 값을 10만큼 증가
        mpBar.GetComponent<BarUpdater>().SmoothIncreaseBackBar(); // mpBar의 배경 바를 부드럽게 증가
    }

    public void UpdateMpRegenBar(int value){
        mpBar.GetComponent<BarUpdater>().UpdateMpRegenBar(value); // mpBar의 재생 속도를 5만큼 증가
    }

    public int GetHpBarValue()
    {
        return hpBar.GetComponent<BarUpdater>().CurBarValue; // 현재 hpBar의 값을 반환
    }
    public int GetMpBarValue()
    {
        return mpBar.GetComponent<BarUpdater>().CurBarValue; // 현재 mpBar의 값을 반환
    }

}
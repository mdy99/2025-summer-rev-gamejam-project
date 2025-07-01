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

}
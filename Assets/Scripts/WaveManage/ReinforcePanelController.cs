using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// hp, mp, mpRegen 버튼을 클릭하여 강화 선택하는 패널 컨트롤러
// 이걸 골라야 이후 스킬 강화가 가능해짐
public class ReinforcePanelController : MonoBehaviour
{
    [SerializeField] private Button hpButton; // HP 증가 버튼
    [SerializeField] private Button mpButton; // MP 증가 버튼
    [SerializeField] private Button mpRegenButton; // MP 재생 속도 증가 버튼
    
    public event Action OnReinforceSelected; // 강화 선택 이벤트

    private bool alreadySelected = false; // 이미 강화 선택 여부

    private void Start()
    {
        hpButton.onClick.AddListener(()=> Select("Hp")); // HP 버튼 클릭 이벤트 등록
        mpButton.onClick.AddListener(()=> Select("Mp")); // MP 버튼 클릭 이벤트 등록
        mpRegenButton.onClick.AddListener(()=> Select("MpRegen")); // MP 재생 속도 버튼 클릭 이벤트 등록
    }


    private void Select(string type){
        if(alreadySelected) return; // 이미 선택된 경우 아무 작업도 하지 않음
        alreadySelected = true; // 강화 선택 상태로 변경
        hpButton.interactable = false; // HP 버튼 비활성화
        mpButton.interactable = false; // MP 버튼 비활성화
        mpRegenButton.interactable = false; // MP 재생 속도 버튼 비활성화

        switch (type)
        {
            case "Hp":
                // HP 증가 로직
                HpButtonClicked(); // HP 버튼 클릭 이벤트 호출
                Debug.Log("HP 강화 선택됨");
                break;
            case "Mp":
                // MP 증가 로직
                MpButtonClicked(); // MP 버튼 클릭 이벤트 호출
                Debug.Log("MP 강화 선택됨");
                break;
            case "MpRegen":
                MpRegenButtonClicked(); // MP 재생 속도 버튼 클릭 이벤트 호출
                Debug.Log("MP 재생 속도 강화 선택됨");
                break;
            default:
                Debug.LogError("알 수 없는 강화 타입: " + type);
                return;
        }

        gameObject.SetActive(false); // 패널 비활성화
        OnReinforceSelected?.Invoke(); // 강화 선택 이벤트 호출
    }

    public void ResetPanel(){
        alreadySelected = false; // 강화 선택 상태 초기화
        gameObject.SetActive(true); // 패널 활성화
        // 버튼 활성화 로직 추가 (필요시)
        hpButton.interactable = true;
        mpButton.interactable = true;
        mpRegenButton.interactable = true;
    }

    void HpButtonClicked()
    {
        BarManager.Instance.UpdateMaxHpBar(10); // 예시로 HP 최대치 10 증가
        BarManager.Instance.UpdateHpBar(10); // HP 현재치도 10 증가
    }

    void MpButtonClicked()
    {
        BarManager.Instance.UpdateMaxMpBar(10); // 예시로 MP 최대치 10 증가
        BarManager.Instance.UpdateMpBar(10); // MP 현재치도 10 증가
    }

    void MpRegenButtonClicked()
    {
        // MP 재생 속도 증가 로직
    }
}

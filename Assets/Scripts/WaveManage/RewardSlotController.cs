using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // TextMeshPro 네임스페이스 추가

public class RewardSlotController : MonoBehaviour
{
    // 임시 변수명
    [SerializeField] private List<Button> runeButtons; // 룬 버튼 리스트

    [SerializeField] private TMP_Text nameText; // 보상 이름 텍스트
    [SerializeField] private TMP_Text typeText; // 보상 타입 텍스트
    [SerializeField] private TMP_Text descriptionText; // 보상 설명 텍스트

    public event Action OnAnyRuneSelected; // 룬 선택 이벤트

    private bool alreadySelected = false; // 룬 선택 여부

    private void Start()
    {
        foreach(var button in runeButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button)); // 버튼 클릭 시 룬 선택
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        if (alreadySelected) return; // 이미 선택된 경우 아무 작업도 하지 않음

        alreadySelected = true; // 룬 선택 상태로 변경

        // 룬 선택 이벤트 호출---
        // 룬 데미지 증가냐. 자원 감소냐
        //----------------------
        OnAnyRuneSelected?.Invoke();
    }

    public void Disable(){
        foreach(var button in runeButtons)
        {
            button.interactable = false; // 룬 버튼 비활성화
        }
    }

    public void Enable()
    {
        if(!alreadySelected){
            foreach(var button in runeButtons)
            {
                button.interactable = true; // 룬 버튼 활성화
            }
        }
    }

    public void ResetSlot(){
        alreadySelected = false; // 룬 선택 상태 초기화
        Disable(); // 룬 버튼 비활성화
    }

}

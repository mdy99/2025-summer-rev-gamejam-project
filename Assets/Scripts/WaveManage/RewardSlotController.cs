using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting; // TextMeshPro 네임스페이스 추가

public class RewardSlotController : MonoBehaviour
{

    [SerializeField] private SkillType slotSkillType; // Novice, Apprentice, Adept 중 하나의 스킬 타입
    // 임시 변수명
    [SerializeField] private List<Button> runeButtons; // 룬 버튼 리스트
    [SerializeField] private List<TMP_Text> runeButtonLabels; // 룬 버튼 레이블 리스트 

    [SerializeField] private TMP_Text nameText; // 보상 이름 텍스트
    [SerializeField] private TMP_Text typeText; // 보상 타입 텍스트
    [SerializeField] private TMP_Text descriptionText; // 보상 설명 텍스트

    private List<string> assignedRuneCodes = new List<string>(); // 버튼에 할당된 룬 코드 리스트
    [SerializeField]private RuneManager runeManager; // 룬 강화 처리 담당

    public event Action OnAnyRuneSelected; // 룬 선택 이벤트

    private bool alreadySelected = false; // 룬 선택 여부

    private int ReinforceDamage = 10;
    private int ReinforceMpCost = -5;

    [SerializeField] private TMP_Text reinforceInfoText; // 룬 강화 횟수를 표시할 텍스트

    public void ShowReinforceInfo()
    {
        string info = "[강화 정보]\n";

        foreach (string runeCode in assignedRuneCodes)
        {
            var (count, damage, mpCost, isMaxed) = RuneReinforceTracker.Instance.GetNextReinforceInfo(runeCode);

            if (isMaxed)
            {
                info += $"{runeCode}: {count}회 → 최대 강화 도달\n";
            }
            else
            {
                info += $"{runeCode}: {count}회 → 다음 +{damage} / 마나 {mpCost}\n";
            }
        }

        reinforceInfoText.text = info;
    }

    public void Initialize(SkillData skillData){
        assignedRuneCodes = SplitRuneCodes(skillData.runeCode); // 룬 코드 분할 및 할당
        for(int i=0;i<runeButtons.Count;i++)
        {
            if(i < assignedRuneCodes.Count)
            {
                string runeCode = assignedRuneCodes[i];
                runeButtonLabels[i].text = runeCode; // 룬 코드 레이블 설정

                runeButtons[i].gameObject.SetActive(true); // 버튼 활성화
                int buttonIndex = i; // 버튼 인덱스 저장 (람다식에서 사용하기 위해)
                runeButtons[i].onClick.RemoveAllListeners(); // 기존 리스너 제거
                runeButtons[i].onClick.AddListener(() => OnButtonClicked(buttonIndex)); // 버튼 클릭 이벤트 등록
            }
            else
            {
                runeButtons[i].gameObject.SetActive(false); // 버튼 비활성화
                runeButtonLabels[i].text = ""; // 레이블 초기화
            }
        }
    }

    void Awake()
{
    if (runeManager == null)
    {
        runeManager = FindObjectOfType<RuneManager>();
        if (runeManager == null)
            Debug.LogError("RuneManager not found in scene!");
    }
}

    public SkillType GetSkillType()
    {
        return slotSkillType; // 슬롯의 스킬 타입 반환
    }

    private List<string> SplitRuneCodes(string fullCode)
    {
        var list = new List<string>();
        foreach (char c in fullCode){
            list.Add(c.ToString()); // 각 문자를 문자열로 변환하여 리스트에 추가
        }
        return list; // 최종 리스트 반환
    }

    private void OnButtonClicked(int index)
    {
        if (alreadySelected) return; // 이미 선택된 경우 아무 작업도 하지 않음

        alreadySelected = true; // 룬 선택 상태로 변경

        // 룬 선택 이벤트 호출
        string runeCode = assignedRuneCodes[index];

        // 강화 수치 외부 Tracker에 위임
        var (damage, mpCost) = RuneReinforceTracker.Instance.ReinforceRune(runeCode);

        if (runeManager == null)
        {
            Debug.LogError("RuneManager is not assigned!");
        }
        runeManager.ReinforceRune(runeCode, damage, mpCost);

        RuneReinforceTracker.Instance.UpdateReinforceInfo(); // 룬 강화 정보 업데이트
        NarrationText.Instance.UpdateNarration($"{runeCode} 룬이 강화되었습니다! {damage}의 데미지 증가와 {mpCost}의 마나 감소가 적용됩니다.", Color.magenta);
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

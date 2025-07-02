using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 밤 전체 관리 패널
public class RewardPanelController : MonoBehaviour
{
    [SerializeField] private GameObject rewardPanel; // 보상 패널 오브젝트

    [SerializeField] private ReinforcePanelController reinforcePanel; // 강화 패널 컨트롤러
    [SerializeField] private List<RewardSlotController> rewardSlots; // 보상 슬롯 리스트
    [SerializeField] private Button nextWaveButton; // 다음 웨이브 버튼

    private bool rewardSelected = false; // 보상 선택 여부

    void Start()
    {
        nextWaveButton.interactable = false; // 초기에는 다음 웨이브 버튼 비활성화
        reinforcePanel.OnReinforceSelected += HandleReinforceSelected; // 강화 선택 이벤트에 리스너 등록

        foreach (var slot in rewardSlots)
        {
            slot.OnAnyRuneSelected += HandleAnyRuneSelected; // 룬 선택 이벤트에 리스너 등록
            slot.Disable(); // 초기에는 모든 보상 슬롯 비활성화
        }

        nextWaveButton.onClick.AddListener(OnClickNextWave); // 다음 웨이브 버튼 클릭 이벤트 등록
    }

    public void SetupRewardSlots(List<SkillData> memorizedSkills)
    {
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            var skill = memorizedSkills[i]; // memorizedSkills에서 스킬 데이터 가져오기
            rewardSlots[i].Initialize(skill); // 슬롯 초기화
        }
    }

    private void HandleReinforceSelected()
    {
        foreach (var slot in rewardSlots)
        {
            slot.Enable(); // 모든 보상 슬롯 활성화
        }
    }

    private void HandleAnyRuneSelected()
    {
        if(rewardSelected) return; // 이미 보상이 선택된 경우 아무 작업도 하지 않음
        rewardSelected = true; // 보상 선택 상태로 변경

        foreach(var slot in rewardSlots)
        {
            slot.Disable(); // 모든 보상 슬롯 비활성화
        }
        nextWaveButton.interactable = true; // 다음 웨이브 버튼 활성화
    }

    public void OnClickNextWave(){
        WaveManager.Instance.OnRewardComplete(); // 웨이브 매니저에 다음 웨이브 요청
    }

    public void ResetPanel(){
        rewardSelected = false; // 보상 선택 상태 초기화
        nextWaveButton.interactable = false; // 다음 웨이브 버튼 비활성화
        reinforcePanel.ResetPanel(); // 강화 패널 초기화

        foreach (var slot in rewardSlots)
        {
            slot.ResetSlot(); // 슬롯 초기화 (필요한 경우)
            slot.Disable(); // 모든 보상 슬롯 비활성화
        }
    }

    public void OpenPanel(){
        rewardPanel.SetActive(true); // 보상 패널 활성화
        Time.timeScale = 0f; // 게임 일시 정지
        reinforcePanel.ResetPanel(); // 강화 패널 초기화
    }

    public void ClosePanel(){
        //rewardSlots.ForEach(slot => slot.ResetSlot()); // 모든 보상 슬롯 초기화
        rewardPanel.SetActive(false); // 보상 패널 비활성화
        Time.timeScale = 1f; // 게임 재개
    }

    public bool GetActive(){
        return rewardPanel.activeSelf;
    }
}

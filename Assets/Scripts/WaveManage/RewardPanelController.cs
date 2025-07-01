using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RewardPanelController : MonoBehaviour
{
    [SerializeField] private GameObject rewardPanel; // 보상 패널 오브젝트

    public void OpenPanel(){
        rewardPanel.SetActive(true); // 보상 패널 활성화
        Time.timeScale = 0f; // 게임 일시 정지
    }

    public void ClosePanel(){
        rewardPanel.SetActive(false); // 보상 패널 비활성화
        Time.timeScale = 1f; // 게임 재개
    }

    public void IncreaseHPLimit(int amount){
        // 플레이어의 HP를 증가시키는 로직
    }

    public void IncreaseMpRegen(int amount){
        // 플레이어의 MP 재생 속도를 증가시키는 로직
    }

    public void IncreaseMpLimit(int amount){
        // 플레이어의 MP를 증가시키는 로직
    }

    public bool GetActive(){
        return rewardPanel.activeSelf;
    }
}

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
    [SerializeField] private Button rewardButton1; // 보상 슬롯 버튼
    [SerializeField] private Button rewardButton2; // 보상 슬롯 버튼
    [SerializeField] private Button rewardButton3; // 보상 슬롯 버튼

    [SerializeField] private TMP_Text nameText; // 보상 이름 텍스트
    [SerializeField] private TMP_Text typeText; // 보상 타입 텍스트
    [SerializeField] private TMP_Text descriptionText; // 보상 설명 텍스트

    public event Action OnRuneSelected; // 룬 선택 이벤트

    private bool isSeleted = false; // 룬 선택 여부

    private void Start()
    {
        rewardButton1.onClick.AddListener(() => SelectRune("Rune1")); // 룬 1 선택 이벤트 등록
        rewardButton2.onClick.AddListener(() => SelectRune("Rune2")); // 룬 2 선택 이벤트 등록
        rewardButton3.onClick.AddListener(() => SelectRune("Rune3")); // 룬 3 선택 이벤트 등록
    }

    public void Disable(){
        rewardButton1.interactable = false;
        rewardButton2.interactable = false;
        rewardButton3.interactable = false;
    }

    public void Enable()
    {
        if(!isSeleted) // 룬이 선택되지 않은 경우에만 버튼 활성화
        {
            rewardButton1.interactable = true;
            rewardButton2.interactable = true;
            rewardButton3.interactable = true;
        }
    }


    private void SelectRune(string runeName)
    {
        if (isSeleted) return;

        isSeleted = true;
        rewardButton1.interactable = false; // 룬 선택 후 버튼 비활성화
        rewardButton2.interactable = false; // 다른 룬 버튼 비활성화
        rewardButton3.interactable = false; // 다른 룬 버튼 비활성화

        // 보상 적용 로직----

        //-----------------
        OnRuneSelected?.Invoke();
    }

}

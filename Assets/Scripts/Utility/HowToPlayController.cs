using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text howToPlayText; // 표시될 텍스트
    [SerializeField] private Button prevButton; // 이전 버튼
    [SerializeField] private Button nextButton; // 다음 버튼

    [Header("설명 페이지")]
    [SerializeField][TextArea(3,10)] private List<string> pages = new List<string>(); // 설명 텍스트들

    private int currentPage = 0;

    private void Start()
    {
        UpdateText();
        prevButton.onClick.AddListener(GoToPreviousPage);
        nextButton.onClick.AddListener(GoToNextPage);
    }

    private void UpdateText()
    {
        if (pages == null || pages.Count == 0)
        {
            howToPlayText.text = "내용이 없습니다.";
            return;
        }

        howToPlayText.text = pages[currentPage];

        // 버튼 활성화 조건 설정
        prevButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < pages.Count - 1;
    }

    private void GoToPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateText();
        }
    }

    private void GoToNextPage()
    {
        if (currentPage < pages.Count - 1)
        {
            currentPage++;
            UpdateText();
        }
    }
}

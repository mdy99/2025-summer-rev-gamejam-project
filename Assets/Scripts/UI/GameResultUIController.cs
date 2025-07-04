using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;


public class GameResultUIController : MonoBehaviour
{
    [Header("패널 & 이미지")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Image resultImage;

    [Header("스프라이트")]
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [SerializeField] private TMP_Text resultText; // 결과 텍스트

    private bool resultShown = false;

    string winText = "마왕을 무찔렀지만, 그가 남기고 간 상처는 깊었습니다. 지금 우리가 취해야 할 일은 그 상처를 치유하고, 다시 일어서는 것입니다.";
    string loseText = "패배!ㅋㅋ";

    void Start()
    {
        resultPanel.SetActive(false); // 시작 시 비활성화

        BarUpdater.OnPlayerDead += ShowLosePanel;
        WaveManager.OnWaveFinished += CheckVictory; // 웨이브가 끝날 때 승리 체크
        StartCoroutine(CheckVictoryCoroutine());
    }

    void CheckVictory()
    {
        if (resultShown) return;

        if (WaveManager.Instance != null && WaveManager.Instance.CurrentState == WaveState.Finished)
        {
            resultImage.sprite = winSprite;
            resultPanel.SetActive(true);
            resultText.text = winText; // 결과 텍스트 설정
            resultShown = true;
        }
    }

    void ShowLosePanel()
    {
        if (resultShown) return;

        resultImage.sprite = loseSprite;
        resultPanel.SetActive(true);
        resultText.text = loseText; // 결과 텍스트 설정
        resultShown = true;
    }

    IEnumerator CheckVictoryCoroutine()
    {
        while (!resultShown)
        {
            if (WaveManager.Instance != null && WaveManager.Instance.CurrentState == WaveState.Finished)
            {
                resultImage.sprite = winSprite;
                resultPanel.SetActive(true);
                resultText.text = winText; // 결과 텍스트 설정
                resultShown = true;
            }
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 체크
        }
    }

    private void OnDestroy()
    {
        BarUpdater.OnPlayerDead -= ShowLosePanel;
        WaveManager.OnWaveFinished -= CheckVictory;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class GameResultUIController : MonoBehaviour
{
    [Header("패널 & 이미지")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Image resultImage;

    [Header("스프라이트")]
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    private bool resultShown = false;

    void Start()
    {
        resultPanel.SetActive(false); // 시작 시 비활성화

        BarUpdater.OnPlayerDead += ShowLosePanel;
        StartCoroutine(CheckVictoryCoroutine());
    }

    void ShowLosePanel()
    {
        if (resultShown) return;

        resultImage.sprite = loseSprite;
        resultPanel.SetActive(true);
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
                resultShown = true;
            }
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 체크
        }
    }

    private void OnDestroy()
    {
        BarUpdater.OnPlayerDead -= ShowLosePanel;
    }
}

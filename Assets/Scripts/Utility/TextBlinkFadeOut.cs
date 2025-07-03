using System.Collections;
using TMPro;
using UnityEngine;

public class TextBlinkFadeOut : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private float blinkDuration = 13f; // 전체 깜빡이는 시간
    [SerializeField] private float blinkSpeed = 5f; 
    private void Awake()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TMP_Text>();
            if (targetText == null)
            {
                Debug.LogError("TMP_Text component is not assigned or found on the GameObject.");
            }
        }
    }

    void Start()
    {
        ShowAndBlink(); // 초기 메시지 표시
    }

    public void ShowAndBlink()
    {
        targetText.alpha = 1f;
        StartCoroutine(SmoothBlinkAndFadeOut());
    }

    private IEnumerator SmoothBlinkAndFadeOut()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            targetText.text = ""; // Escape 키를 누르면 텍스트 제거
            yield break;
        }
        float elapsed = 0f;
        Color originalColor = targetText.color;

        while (elapsed < blinkDuration)
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f; // 0 ~ 1 사이 부드러운 반복
            Color newColor = originalColor;
            newColor.a = alpha;
            targetText.color = newColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 종료 시 텍스트 제거 및 알파 복원
        targetText.text = "";
        targetText.color = originalColor;
    }
}

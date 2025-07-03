using UnityEngine;

public class BackgroundDimmer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer; // 배경 스프라이트
    [SerializeField] private int maxWave = 7;

    // 웨이브마다 불러줌
    public void ApplyWaveEffect(int currentWave)
    {
        float t = Mathf.Clamp01((float)(currentWave - 1) / (maxWave - 1)); // 0~1 비율

        // 채도 줄이기 (원본 색에서 회색으로 선형 보간)
        Color originalColor = Color.white; // 원래 배경색 (변경 가능)
        Color gray = Color.Lerp(originalColor, Color.gray, t * 0.8f); // 채도 감소

        // 명도 줄이기
        float brightnessFactor = Mathf.Lerp(1f, 0.6f, t); // 밝기 감소

        // 최종 색 계산
        Color finalColor = gray * brightnessFactor;
        finalColor.a = 1f; // 알파는 그대로 유지

        backgroundRenderer.color = finalColor;
    }
}

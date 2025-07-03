using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleZoomFixer : MonoBehaviour
{
    private Camera mainCam;
    private ParticleSystem ps;
    private float initialZoom;
    private float initialScale;

    void Start()
    {
        mainCam = Camera.main;
        ps = GetComponent<ParticleSystem>();

        // 초기 카메라 사이즈 및 스케일 저장
        initialZoom = mainCam.orthographicSize;
        initialScale = transform.localScale.x;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        // 현재 줌 비율에 반비례하는 보정값
        float scaleFactor = initialZoom / mainCam.orthographicSize;

        // 파티클 오브젝트 스케일 보정
        transform.localScale = Vector3.one * (initialScale * scaleFactor);
    }
}

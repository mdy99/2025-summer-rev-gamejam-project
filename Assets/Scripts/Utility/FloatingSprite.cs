using UnityEngine;

public class FloatingSprite : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float floatAmplitude = 0.25f; // 위아래 이동 거리
    [SerializeField] private float floatSpeed = 1f;         // 부유 속도
    [SerializeField] private bool useLocalPosition = true;  // 로컬 위치 기준으로 움직일지

    private Vector3 startPos;

    void Start()
    {
        startPos = useLocalPosition ? transform.localPosition : transform.position;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        Vector3 offset = new Vector3(0f, newY, 0f);

        if (useLocalPosition)
            transform.localPosition = startPos + offset;
        else
            transform.position = startPos + offset;
    }
}

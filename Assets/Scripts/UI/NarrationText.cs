using System.Collections;
using TMPro;
using UnityEngine;

public class NarrationText : MonoBehaviour
{
    public static NarrationText Instance { get; private set; }

    [SerializeField] private TMP_Text UpperNarrationText;
    [SerializeField] private TMP_Text MiddleNarrationText;
    [SerializeField] private TMP_Text LowerNarrationText;

    private Coroutine upperCoroutine;
    private Coroutine middleCoroutine;
    private Coroutine lowerCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // 싱글톤 인스턴스 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 오브젝트 제거
        }
    }

    void Start()
    {
        UpperNarrationText.text = "";
        MiddleNarrationText.text = "";
        LowerNarrationText.text = "";
    }

    public void UpdateNarration(string newNarration)
    {
        // 위로 밀기
        UpperNarrationText.text = MiddleNarrationText.text;
        if (upperCoroutine != null) StopCoroutine(upperCoroutine);
        upperCoroutine = StartCoroutine(ClearAfterDelay(UpperNarrationText, 3f));

        MiddleNarrationText.text = LowerNarrationText.text;
        if (middleCoroutine != null) StopCoroutine(middleCoroutine);
        middleCoroutine = StartCoroutine(ClearAfterDelay(MiddleNarrationText, 3f));

        LowerNarrationText.text = newNarration;
        if (lowerCoroutine != null) StopCoroutine(lowerCoroutine);
        lowerCoroutine = StartCoroutine(ClearAfterDelay(LowerNarrationText, 3f));
    }

    private IEnumerator ClearAfterDelay(TMP_Text text, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.text = "";
    }
}

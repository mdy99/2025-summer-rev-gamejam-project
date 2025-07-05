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
        UpdateNarration($"{KeySettingManager.Instance.morseKey}를 눌러 마법진 소환", Color.black); // 게임 시작 시 내레이션 업데이트
        UpdateNarration($"{KeySettingManager.Instance.enterKey}를 눌러 룬 새김", Color.black); // 게임 시작 시 내레이션 업데이트
    }

public void UpdateNarration(string newNarration, Color color)
{
    // 위로 밀기
    UpperNarrationText.text = MiddleNarrationText.text;
    UpperNarrationText.color = MiddleNarrationText.color;
    if (upperCoroutine != null) StopCoroutine(upperCoroutine);
    upperCoroutine = StartCoroutine(ClearAfterDelay(UpperNarrationText, 5f));

    MiddleNarrationText.text = LowerNarrationText.text;
    MiddleNarrationText.color = LowerNarrationText.color;
    if (middleCoroutine != null) StopCoroutine(middleCoroutine);
    middleCoroutine = StartCoroutine(ClearAfterDelay(MiddleNarrationText, 5f));

    LowerNarrationText.text = newNarration;
    LowerNarrationText.color = color;
    if (lowerCoroutine != null) StopCoroutine(lowerCoroutine);
    lowerCoroutine = StartCoroutine(ClearAfterDelay(LowerNarrationText, 5f));
}

    private IEnumerator ClearAfterDelay(TMP_Text text, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.text = "";
    }
}

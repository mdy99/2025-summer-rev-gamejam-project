using System.Collections;
using TMPro;
using UnityEngine;

public class NarrationText : MonoBehaviour
{
    [SerializeField] private TMP_Text UpperNarrationText;
    [SerializeField] private TMP_Text MiddleNarrationText;
    [SerializeField] private TMP_Text LowerNarrationText;

    public static NarrationText Instance { get; private set; }

    private Coroutine upperCoroutine;
    private Coroutine middleCoroutine;
    private Coroutine lowerCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
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

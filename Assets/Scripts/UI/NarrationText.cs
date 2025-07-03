using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrationText : MonoBehaviour
{
    private TMP_Text narrationText; // 내레이션 텍스트 컴포넌트

    public static NarrationText Instance { get; private set; } // 싱글톤 인스턴스

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
        narrationText = GetComponent<TMP_Text>(); // 내레이션 텍스트 컴포넌트 가져오기
    }

    public void UpdateNarration(string newNarration)
    {
        narrationText.text = newNarration; // 내레이션 텍스트 업데이트
    }
}
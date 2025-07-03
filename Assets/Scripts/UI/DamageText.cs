using System.Collections;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private TMP_Text damageText; // 피해 텍스트를 표시하는 TextMeshPro 컴포넌트

    public float floatUpSpeed = 1.5f; // 텍스트가 위로 떠오르는 속도
    public float duration = 1f; // 텍스트가 사라지는 시간

    void Awake()
    {
        damageText = GetComponent<TMP_Text>(); // TextMeshPro 컴포넌트 가져오기
        if (damageText == null)
        {
            Debug.LogError("DamageText 컴포넌트에 TMP_Text가 할당되지 않았습니다.");
        }
    }

    public void Init(int damage)
    {
        damageText.text = damage.ToString(); // 피해 값을 텍스트로 설정
        StartCoroutine(FloatingText());
    }

    IEnumerator FloatingText()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = startPos + Vector3.up * floatUpSpeed * t;
            elapsed += Time.deltaTime;
            yield return null;
        }

        DamageTextPool.Instance.ReturnText(gameObject); // 텍스트가 사라진 후 풀에 반환
    }
}



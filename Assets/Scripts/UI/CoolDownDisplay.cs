using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CoolDownDisplay : MonoBehaviour
{
    private Image coolDownImage; // 쿨타임 이미지
    private float coolDownTime; // 쿨타임 시간
    private float lastUsedTime; // 마지막 그래플링 훅 사용 시간

    void Awake()
    {
        coolDownImage = GetComponent<Image>(); // 쿨타임 이미지 컴포넌트 가져오기
        coolDownImage.fillAmount = 1f; // 초기화 시 이미지 채움 비율을 0으로 설정
    }

    public float CoolDownTime
    {
        get { return coolDownTime; }
        set { coolDownTime = value; }
    }

    public float LastUsedTime
    {
        get { return lastUsedTime; }
        set { lastUsedTime = value; }
    }

    public bool IsAvailable
    {
        get { return Time.time - lastUsedTime >= coolDownTime; }
    }

    public void StartCoolDown(float time)
    {
        coolDownTime = time; // 쿨타임 시간 설정
        lastUsedTime = Time.time; // 현재 시간을 마지막 사용 시간으로 설정
        StartCoroutine(CoolDownCoroutine()); // 쿨타임 코루틴 시작
    }

    public void ResetCoolDown()
    {
        StopAllCoroutines(); // 모든 코루틴 중지
        coolDownImage.fillAmount = 1f; // 이미지 채움 비율을 0으로 설정
    }

    private void Start()
    {
        if (coolDownImage == null)
        {
            Debug.LogError("CoolDownImage is not assigned in the inspector.");
        }
        ResetCoolDown(); // 초기화 시 쿨타임 이미지 비활성화
    }
private IEnumerator CoolDownCoroutine()
{
    float elapsedTime = 0f; // 경과 시간 초기화
    while (elapsedTime < coolDownTime)
    {
        elapsedTime = Time.time - lastUsedTime; // 경과 시간 계산
        coolDownImage.fillAmount = Mathf.Clamp01(elapsedTime / coolDownTime); // 0 → 1로 차오르게 설정
        yield return null; // 다음 프레임까지 대기
    }
    coolDownImage.fillAmount = 1f; // 쿨타임이 끝나면 완전히 채움
}

}

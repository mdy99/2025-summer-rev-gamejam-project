using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarUpdater : MonoBehaviour
{

    // UI에서 바를 나타내는 이미지 컴포넌트
    public Image backBarImage; // 배경 바 이미지 
    public Image slowBarImage;  // 느리게 업데이트되는 바 이미지
    public Image fastBarImage;  // 빠르게 업데이트되는 바 이미지
    public TMP_Text barText; // 현재 바 값을 표시하는 텍스트

    public static event Action OnPlayerDead; // 플레이어가 죽었을 때 호출되는 이벤트

    public int CurBarValue{
        get => curBarValue;
        set{
            curBarValue = Mathf.Clamp(value, MINBARVALUE, maxBarValue);
            ApplyBarUI(); // 바의 UI를 업데이트
        }
    }

    public int MaxBarValue{
        get => maxBarValue;
        set{
            maxBarValue = Mathf.Clamp(value, MINBARVALUE,LIMITBARVALUE); // 최대 값은 최소 값 이상이어야 함

            // 현재 값이 최대 값을 초과하면 최대 값으로 설정
            if (curBarValue > maxBarValue)
            {
                CurBarValue = maxBarValue; 
            }

            ApplyBarUI(); // 바의 UI를 업데이트
        }
    }

    public int debugValue=0; // 디버그용 바 값 변경 변수

    public void UpdateBar(int delta) => CurBarValue += delta; // 현재 바 값을 delta만큼 업데이트
    public void UpdateMaxBar(int delta) => MaxBarValue += delta; // 최대 바 값을 delta만큼 업데이트

    private Coroutine regenCoroutine; // 재생성 코루틴

    private const int LIMITBARVALUE = 200; // 최대 바의 제한 값
    private const int MINBARVALUE =0;    // 최소 바의 값
    
    // BackBar의 너비를 부드럽게 업데이트하기 위한 변수
    private float baseBackWidth; // 기본 바의 너비 (픽셀 단위)
    private float curBackWidth; // 현재 바의 너비
    private float targetBackWidth; // 목표 바의 너비 (픽셀 단위)
    private float backBarUpdateSpeed =5.6f; // 바의 너비 업데이트 속도

    // slowBarImage와 fastBarImage의 fillAmount를 부드럽게 업데이트하기 위한 변수
    private int curBarValue = 100; // 현재 바의 값
    private int maxBarValue = 100; // 최대 바의 값
    private float barUpdateSpeed =1.8f; // 바의 채움 비율 업데이트 속도
    private float targetFillAmount = 1f; // 목표 채움 비율

    private int regenValue = 7; // 자동 재생성 시 증가하는 바의 값

    // Start is called before the first frame update
    void Start()
    {
        InitSetting(); // 초기 설정
        if(gameObject.tag == "MpBar"){
            regenCoroutine = StartCoroutine(AutoRegenBar()); // 자동 회복 시작
        }
    }

    public void UpdateMpRegenBar(int value)
    {
        regenValue += value; // 재생성 속도 업데이트
        StopRegen(); // 기존 재생성 코루틴 중지
        regenCoroutine = StartCoroutine(AutoRegenBar()); // 새로운 재생성 코루틴 시작
    }

    // Update is called once per frame
    void Update()
    {
        //디버그용 확인 코드----------------------------
        if(Input.GetKeyDown(KeyCode.F1))
        {
            // 예시로 Space 키를 누르면 바의 값을 감소시키는 동작
            UpdateBar(debugValue);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UpdateMaxBar(debugValue);
        }
        // --------------------------------------------

        // 바의 채움 비율을 부드럽게 업데이트
        slowBarImage.fillAmount = Mathf.Lerp(slowBarImage.fillAmount, targetFillAmount, barUpdateSpeed * Time.deltaTime);
        
        if(Time.timeScale<0.1f) return;// 게임이 일시 정지 상태일 때

        curBackWidth = Mathf.Lerp(curBackWidth, targetBackWidth, backBarUpdateSpeed * Time.deltaTime);
        backBarImage.rectTransform.sizeDelta = new Vector2(curBackWidth, backBarImage.rectTransform.sizeDelta.y);
        if( curBarValue <= MINBARVALUE) // 현재 바 값이 최소 값 이하일 때
        {
            if(gameObject.tag == "MpBar") return; // MP 바일 경우 플레이어가 죽었을 때 이벤트를 발생시키지 않음 
            OnPlayerDead?.Invoke(); // 플레이어가 죽었음을 알리는 이벤트 호출
            CurBarValue = MINBARVALUE; // 현재 바 값을 최소 값으로 설정
        }
    }

    public void SmoothIncreaseBackBar(){
        StartCoroutine(SmoothBackBarCoroutine(0.1f, targetBackWidth)); // 부드럽게 바의 너비를 증가시키는 코루틴 시작
    }

    private IEnumerator SmoothBackBarCoroutine(float duration, float targetWidth){
        float startWidth = backBarImage.rectTransform.sizeDelta.x; // 시작 너비 저장
        float elapsedTime = 0f; // 경과 시간 초기화

        while(elapsedTime < duration){
            elapsedTime += Time.unscaledDeltaTime; // 경과 시간 업데이트
            float t = elapsedTime / duration; // 0과 1 사이의 비율 계산
            backBarImage.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(startWidth, targetWidth, t), backBarImage.rectTransform.sizeDelta.y);
            yield return null; // 다음 프레임까지 대기
        }

        backBarImage.rectTransform.sizeDelta = new Vector2(targetWidth, backBarImage.rectTransform.sizeDelta.y); // 최종 너비 설정
    }


    private void InitSetting(){
        baseBackWidth = backBarImage.rectTransform.sizeDelta.x; // 초기 너비 저장
        ApplyBarUI(); // 바의 UI를 초기화
    }

    private void ApplyBarUI(){
        targetFillAmount = (float)curBarValue / maxBarValue;
        fastBarImage.fillAmount = targetFillAmount;

        targetBackWidth = baseBackWidth *(maxBarValue/100f); // 목표 바의 너비 계산

        Vector2 newSize = new Vector2(targetBackWidth-15.0f, fastBarImage.rectTransform.sizeDelta.y);
        fastBarImage.rectTransform.sizeDelta = newSize; // fastBarImage의 크기 업데이트
        slowBarImage.rectTransform.sizeDelta = newSize; // slowBarImage의 크기 업데이트

        // 현재 바 값을 텍스트로 표시
        if (barText != null)
        {
            barText.text = $"{curBarValue}/{maxBarValue}";
        }
        else
        {
            Debug.LogWarning("Bar text component is not assigned.");
        }
    }

    private IEnumerator AutoRegenBar(){
        WaitForSeconds waitTime = new WaitForSeconds(1f); // 1초 대기 시간
        while(true){
            if(curBarValue < maxBarValue) // 현재 바 값이 최대 바 값보다 작을 때
            {
                CurBarValue += regenValue; // 현재 바 값을 regenValue만큼 증가
            }
            yield return waitTime; // 대기 시간 동안 대기
        }
    }

    private void StopRegen(){
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine); // 자동 재생성 코루틴 중지
            regenCoroutine = null; // 코루틴 변수 초기화
        }
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressedDetector : MonoBehaviour
{
    public KeyCode keyToDetect = KeyCode.A; // 탐지할 키
    public float threshold = 0.01f; // 키 입력 간격 임계값
    
    private float keyDownTime = 0f; // 키가 눌린 시간
    private bool isKeyPressed = false; // 키가 눌렸는지 여부

    public event Action<string> OnSymbolDetected; // 심볼이 감지되었을 때 호출되는 이벤트

    public void KeySetting(KeyCode keyToDetect= KeyCode.A, float threshold = 0.5f)
    {
        this.keyToDetect = keyToDetect; // 탐지할 키 설정
        this.threshold = threshold; // 임계값 설정
    }

    // Update is called once per frame
    void Update()
    {
        if(WaveManager.Instance.CurrentState != WaveState.InWave) // 현재 웨이브 상태가 InWave가 아니면
        {
            return; // 함수 종료
        }
        if(Input.GetKeyDown(keyToDetect))
        {
            if (!isKeyPressed)
            {
                isKeyPressed = true;
                keyDownTime = Time.time; // 현재 시간을 기록
            }
        }

        if(Input.GetKeyUp(keyToDetect))
        {
            if (isKeyPressed)
            {
                isKeyPressed = false;
                float pressDuration = Time.time - keyDownTime; // 키가 떼어진 시간을 기록

                // 키 입력 간격이 임계값보다 작으면 심볼 감지
                if (pressDuration < threshold)
                {
                    OnSymbolDetected?.Invoke("."); // 심볼 감지 이벤트 호출
                }
                else{
                    OnSymbolDetected?.Invoke("ㅡ"); // 심볼 감지 이벤트 호출
                }
            }
        }
    }
}

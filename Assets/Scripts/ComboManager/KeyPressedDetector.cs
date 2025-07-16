using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressedDetector : MonoBehaviour
{
    public KeyCode keyToDetect = KeyCode.A; // 탐지할 키

    private bool isKeyPressed = false; // 키가 눌렸는지 여부

    public event Action<string> OnSymbolDetected; // 심볼이 감지되었을 때 호출되는 이벤트


    public void KeySetting(KeyCode keyToDetect= KeyCode.A)
    {
        this.keyToDetect = keyToDetect; // 탐지할 키 설정
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
                NativeTimer.StartTimer(); // 네이티브 타이머 시작
            }
        }

        if(Input.GetKeyUp(keyToDetect))
        {
            if (isKeyPressed)
            {
                isKeyPressed = false;
                char pressedChar = NativeTimer.GetInputType(); // 키가 떼어진 시간을 기록

                // 키 입력 간격이 임계값보다 작으면 심볼 감지
                if (pressedChar =='.')
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

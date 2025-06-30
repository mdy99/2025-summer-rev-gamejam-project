using UnityEngine;

public class DoubleClickFunc
{
    private static float doubleClickThreshold = 0.3f; // 더블 클릭 간격 (초 단위)
    private static float lastClickTime = -1f; // 마지막 클릭 시간 저장
    
    public static bool IsDoubleClick(int mouseButton)
    {
        if (Input.GetMouseButtonDown(mouseButton)) // 마우스 버튼이 눌렸을 때
        {
            float currentTime = Time.time;
            if (currentTime - lastClickTime < doubleClickThreshold)
            {
                lastClickTime = -1f; // 더블 클릭이 발생했으므로 초기화
                return true; // 더블 클릭 감지
            }
            lastClickTime = currentTime; // 마지막 클릭 시간 업데이트
        }
        return false; // 더블 클릭이 아님
    }
}
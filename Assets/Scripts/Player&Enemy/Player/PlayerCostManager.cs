using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCostManager : MonoBehaviour
{
    void OnEnable()
    {
        BarUpdater.OnPlayerDead += HandlePlayerDeath; // 플레이어가 죽었을 때 이벤트 핸들러 등록
    }

    void OnDisable()
    {
        BarUpdater.OnPlayerDead -= HandlePlayerDeath; // 플레이어가 죽었을 때 이벤트 핸들러 등록        
    }

    void HandlePlayerDeath()
    {
        // 플레이어가 죽었을 때 실행할 로직
        Debug.Log("Player has died. Handling player death logic.");
        
        // 예: UI 업데이트, 게임 오버 처리 등
    }
}

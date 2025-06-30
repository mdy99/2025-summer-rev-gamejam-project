using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // 일시정지 패널 오브젝트
    private bool isPaused = false; // 게임이 일시정지 상태인지 여부

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // 게임 일시정지
            pausePanel.SetActive(true); // 패널 활성화
        }
        else
        {
            Time.timeScale = 1f; // 게임 재개
            pausePanel.SetActive(false); // 패널 비활성화
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동에 필요

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel; // 게임 방법 패널
    [SerializeField] private GameObject keySettingPanel; // 키 설정 패널
    [SerializeField] private GameObject volumeSettingsPanel; // 볼륨 설정 패널

    // 게임 시작 (씬 이동)
    public void StartGame(string sceneName)
    {

        // 1. 일시정지 상태 해제
        Time.timeScale = 1f;

        if (RuneManager.Instance != null)
        {
            // 이미 Destroy된 오브젝트인지도 확인
            if (RuneManager.Instance.gameObject != null)
            {
                RuneManager.Instance?.NotifyRuneChanged(); // 룬 정보 변경 알림
                RuneManager.Instance?.GetComponent<RuneInfoDatabase>()?.ResetDatabase();
            }
        }

        // 강화 수치 초기화
        BarReinforceTracker.Instance?.ResetTracker();
        RuneReinforceTracker.Instance?.ResetTracker();

        SceneManager.LoadScene(sceneName);
    }
public void ToMain()
{
    Time.timeScale = 1f;

    // 🔁 Rune 초기화
    RuneManager.Instance?.ResetRuneDatabase(); // 내부적으로 ResetDatabase + Notify

    // 강화 정보 초기화도 같이
    BarReinforceTracker.Instance?.ResetTracker();
    RuneReinforceTracker.Instance?.ResetTracker();

    SceneManager.LoadScene("StartScene");
}


    // 게임 종료
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 실행 중지
#else
        Application.Quit(); // 빌드된 게임 종료
#endif
    }

    public void OpenVolumeSettings()
    {
        if (volumeSettingsPanel != null)
            volumeSettingsPanel.SetActive(true);
    }

    public void CloseVolumeSettings()
    {
        if (volumeSettingsPanel != null)
            volumeSettingsPanel.SetActive(false);
    }

    // 게임 방법 패널 열기
    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    // 게임 방법 패널 닫기
    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }

     // 게임 방법 패널 열기
    public void OpenKeySetting()
    {
        if (keySettingPanel != null)
            keySettingPanel.SetActive(true);
    }

    // 게임 방법 패널 닫기
    public void CloseKeySetting()
    {
        if (keySettingPanel != null)
            keySettingPanel.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동에 필요

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] private GameObject howToPlayPanel; // 게임 방법 패널
    [SerializeField] private GameObject keySettingPanel; // 키 설정 패널

    // 게임 시작 (씬 이동)
    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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

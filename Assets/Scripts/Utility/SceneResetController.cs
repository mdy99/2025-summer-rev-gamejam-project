using UnityEngine;

public class SceneResetController : MonoBehaviour
{
    private Color32 resetBackgroundColor = new Color32(53, 102, 33, 255); // #4D7241

    [SerializeField] private GameObject backbackground; // 게임 오브젝트의 스프라이트 렌더러
    [SerializeField] private bool disableSprites = true;

    void Start()
    {
        ResetVisualState();
    }

    public void ResetVisualState()
    {
        // ✅ 카메라 배경색 변경
        if (Camera.main != null)
            Camera.main.backgroundColor = resetBackgroundColor;

        backbackground.SetActive(false); // 게임 오브젝트 비활성화
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUIRenderer : MonoBehaviour
{
    public Dictionary<string, Sprite> runeSprites; // 룬 프리팹 딕셔너리

    [SerializeField] private Transform comboPanel; // 콤보 UI 패널
    [SerializeField] private GameObject dotPrefab; // 콤보 UI에 표시할 . 프리팹
    [SerializeField] private GameObject dashPrefab; // 콤보 UI에 표시할 ㅡ 프리팹
    [SerializeField] private GameObject runePrefab; // 하나의 공통 룬 UI 프리팹

    private List<GameObject> spawnedCombo = new List<GameObject>(); // 생성된 콤보 UI 오브젝트를 저장하는 리스트

    [SerializeField] private CanvasGroup canvasGroup; // 콤보 UI 패널의 캔버스 그룹
    private Vector2 hiddenPosition;// 콤보 UI 패널을 숨길 위치
    private Vector2 visiblePosition; // 콤보 UI 패널을 표시할 위치
    private bool isAnimating = false; // 애니메이션 중인지 여부

    void Awake()
    {
        InitSpriteDictionary(); // 룬 스프라이트 딕셔너리 초기화
        InitCanvasGroup(); // 콤보 UI 패널의 캔버스 그룹 초기화
    }

    public void SetPanelActive(bool isActive){
        if(!isAnimating){
            StartCoroutine(AnimatePanel(isActive)); // 애니메이션 코루틴 시작
        }
    }

    // 콤보 UI 패널이 활성화되어 있는지 여부 반환
    public bool IsPanelActive(){ return canvasGroup.alpha > 0.01f; }

    // 콤보 UI에 모스부호 이미지 추가
    public void AddSymbolImage(string symbol){
        GameObject prefab = (symbol ==".") ? dotPrefab : dashPrefab; // 심볼에 따라 프리팹 선택
        GameObject img = Instantiate(prefab, comboPanel); // 콤보 UI 패널에 프리팹 인스턴스 생성
        spawnedCombo.Add(img); // 생성된 오브젝트를 리스트에 추가
    }

    // 콤보 UI에 표시된 모스부호 이미지 전체 삭제
    public void RemoveMorseImage(){
        for(int i = spawnedCombo.Count - 1; i >= 0; i--){
            GameObject lastImage = spawnedCombo[i];
            if(lastImage.GetComponent<MorseSymbol>() != null){ // 모스 부호 이미지인지 확인
                spawnedCombo.RemoveAt(i); // 리스트에서 마지막 콤보 UI 오브젝트 제거
                Destroy(lastImage); // 리스트에 있는 콤보 UI 오브젝트 삭제
            }
        }
    }

    // 콤보 UI에 표시된 모든 콤보(룬, 모스) 이미지 삭제
    public void ClearComboImage(){
        foreach(var img in spawnedCombo){Destroy(img);}// 생성된 콤보 UI 오브젝트 삭제
        spawnedCombo.Clear(); // 리스트 초기화
    }

    public void AddRuneImage(string rune){
        if (!runeSprites.ContainsKey(rune)) {
            Debug.LogError($"Rune prefab for {rune} 찾을 수 없습니당.");
            return;
        }
        Debug.Log($"룬 이미지 추가: {rune}"); // 디버그 로그 출력
        GameObject runeObj = Instantiate(runePrefab, comboPanel);
        Image img = runeObj.GetComponent<Image>();
        if(img != null){
            Debug.Log($"룬 스프라이트 설정: {rune}"); // 디버그 로그 출력
            img.sprite = runeSprites[rune]; // 룬 스프라이트 설정
        } else {
            Debug.LogError("룬 오브젝트에 Image 컴포넌트가 없습니다.");
        }
        spawnedCombo.Add(runeObj); // 생성된 룬 오브젝트를 리스트에
    }


    private IEnumerator AnimatePanel(bool show)
    {
        isAnimating = true; // 애니메이션 시작

        float duration = 0.3f; // 애니메이션 지속 시간
        float time = 0f; // 애니메이션 진행 시간

        RectTransform rectTransform = comboPanel.GetComponent<RectTransform>();
        Vector2 startPosition = show ? hiddenPosition : visiblePosition; // 시작 위치 설정
        Vector2 endPosition = show ? visiblePosition : hiddenPosition; // 끝 위치 설정

        float startAlpha = show ? 0f : 1f; // 시작 투명도 설정
        float endAlpha = show ? 1f : 0f; // 끝 투명

        if(show){
            canvasGroup.interactable = true; // 상호작용 가능 설정
            canvasGroup.blocksRaycasts = true; // 레이캐스트 차단 설정
        }

        while(time<duration){
            float t = time / duration; // 애니메이션 진행 비율 계산
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t); // 위치 보간
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t); // 투명도
            time += Time.deltaTime; // 시간 증가
            yield return null; // 다음 프레임까지 대기
        }

        rectTransform.anchoredPosition = endPosition; // 최종 위치 설정
        canvasGroup.alpha = endAlpha; // 최종 투명도 설정

        if(!show){
            canvasGroup.interactable = false; // 상호작용 불가능 설정
            canvasGroup.blocksRaycasts = false; // 레이캐스트 차단 해제
        }
        isAnimating = false; // 애니메이션 종료
    }

    void InitSpriteDictionary()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("RuneSprites/rune_sheet"); // 룬 스프라이트 로드
        runeSprites = new Dictionary<string, Sprite>(); // 룬 스프라이트 딕셔너리 초기화
        // 룬 프리팹을 딕셔너리에 추가
        foreach (Sprite sprite in sprites) {
            runeSprites[sprite.name] = sprite; // 룬 스프라이트 이름을 키로 사용
        }
    }

    void InitCanvasGroup()
    {
        canvasGroup = comboPanel.GetComponent<CanvasGroup>(); // 콤보 UI 패널의 캔버스 그룹 가져오기
        if (canvasGroup == null) {
            Debug.LogError("콤보 UI 패널에 CanvasGroup 컴포넌트가 없습니다.");
            return;
        }
        hiddenPosition = new Vector2(comboPanel.position.x, -3f); // 숨길 위치 설정
        visiblePosition = comboPanel.position; // 표시할 위치 설정
        comboPanel.position = hiddenPosition; // 초기 위치를 숨길 위치로 설정
        canvasGroup.alpha = 0f; // 초기 투명도 설정
        canvasGroup.interactable = false; // 초기 상호작용 불가능 설정
        canvasGroup.blocksRaycasts = false; // 초기 레이캐스트 차단 설정
    }
}
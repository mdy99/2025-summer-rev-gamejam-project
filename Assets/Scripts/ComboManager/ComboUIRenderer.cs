using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.Rendering;

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

    [SerializeField] private Animator animator; // 플레이어 애니메이터
    [SerializeField] private Volume globalVolume; // 전역 볼륨

    private IObjectPool<GameObject> dotPool;
    private IObjectPool<GameObject> dashPool;

    void Awake()
    {
        InitSpriteDictionary(); // 룬 스프라이트 딕셔너리 초기화
        InitCanvasGroup(); // 콤보 UI 패널의 캔버스 그룹 초기화
        InitObjectPools();
    }

    private void InitObjectPools()
    {
        // Dot 풀 설정
        dotPool = new UnityEngine.Pool.ObjectPool<GameObject>(
            createFunc: () => Instantiate(dotPrefab, comboPanel),            // 풀에 없을 때 새로 생성할 점
            actionOnGet: (obj) => obj.SetActive(true),                             // 풀에서 꺼낼 때 활성화
            actionOnRelease: (obj) => obj.SetActive(false),                        // 풀에 반환할 때 비활성화
            actionOnDestroy: (obj) => Destroy(obj),                                // 풀이 꽉 찼거나 터질 때 물리적 삭제
            collectionCheck: true, defaultCapacity: 5, maxSize: 10
        );

        // Dash 풀 설정
        dashPool = new UnityEngine.Pool.ObjectPool<GameObject>(
            createFunc: () => Instantiate(dashPrefab, comboPanel),           // 풀에 없을 때 새로 생성할 대시
            actionOnGet: (obj) => obj.SetActive(true),                             // 풀에서 꺼낼 때 활성화
            actionOnRelease: (obj) => obj.SetActive(false),                        // 풀에 반환할 때 비활성화
            actionOnDestroy: (obj) => Destroy(obj),                                // 풀이 꽉 찼거나 터질 때 물리적 삭제
            collectionCheck: true, defaultCapacity: 5, maxSize: 10
        );
    }

    public void SetPanelActive(bool isActive){
        if(!isAnimating){
            StartCoroutine(AnimatePanel(isActive)); // 애니메이션 코루틴 시작
        }
    }

    // 콤보 UI 패널이 활성화되어 있는지 여부 반환
    public bool IsPanelActive(){ return canvasGroup.alpha > 0.01f; }

    // 콤보 UI에 모스부호 이미지 추가
    public void AddSymbolImage(string symbol)
    {
        GameObject img = null;
        if (symbol == ".")
        {
            SoundManager.Instance.PlaySFX("Dot");
            animator.SetTrigger("TriggerDot"); // 점 추가 애니메이션 트리거
            img = dotPool.Get();
        }
        else
        {
            SoundManager.Instance.PlaySFX("Dash");
            animator.SetTrigger("TriggerDash"); // 대시 추가 애니메이션 트리거
            img = dashPool.Get();
        }

        // 1. 오브젝트 풀에서 꺼낸 뒤 부모 위치를 재정렬합니다.
        img.transform.SetParent(comboPanel, false);

        // 이 코드가 들어가야 Layout Group에서 정상적으로 맨 오른쪽에 새 기호가 추가됩니다.
        img.transform.SetAsLastSibling();

        // 2. 추적 리스트에 등록
        spawnedCombo.Add(img);
    }

    // 콤보 UI에 표시된 모스부호 이미지 전체 삭제 (풀로 반환)
    public void RemoveMorseImage()
    {
        for (int i = spawnedCombo.Count - 1; i >= 0; i--)
        {
            GameObject lastImage = spawnedCombo[i];

            // 모스 부호 이미지인지 확인
            if (lastImage != null && lastImage.GetComponent<MorseSymbol>() != null)
            {
                spawnedCombo.RemoveAt(i); // 1. 추적 리스트에서 먼저 제거 (참조 끊기)

                if (lastImage.name.Contains("Dot") || lastImage.name.StartsWith(dotPrefab.name))
                {
                    dotPool.Release(lastImage); // 2. 풀로 반환 (만약 풀이 20개 꽉 찼으면 내부에서 Destroy 발동)
                }
                else
                {
                    dashPool.Release(lastImage);
                }
            }
        }
    }

    // 콤보 UI에 표시된 모든 콤보(룬, 모스) 이미지 삭제 및 풀 반환
    public void ClearComboImage()
    {
        // 💡 역순 루프를 돌거나 복사본을 이용해 참조 동기화 오류 방지
        for (int i = spawnedCombo.Count - 1; i >= 0; i--)
        {
            GameObject img = spawnedCombo[i];
            if (img == null) continue;

            if (img.GetComponent<MorseSymbol>() != null)
            {
                spawnedCombo.RemoveAt(i); // 추적 리스트에서 제거

                // 모스부호는 오브젝트 풀로 돌려보냄
                if (img.name.Contains("Dot") || img.name.StartsWith(dotPrefab.name))
                {
                    dotPool.Release(img);
                }
                else
                {
                    dashPool.Release(img);
                }
            }
            else
            {
                spawnedCombo.RemoveAt(i); // 룬 UI도 리스트에서 제거 후 파괴
                Destroy(img);
            }
        }

        spawnedCombo.Clear(); // 혹시 남은 찌꺼기 리스트 초기화
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

        if (show)
        {
            canvasGroup.interactable = true; // 상호작용 가능 설정
            canvasGroup.blocksRaycasts = true; // 레이캐스트 차단 설정
        }

        // 💡 [최적화 1] 무거운 Bloom 가져오기는 루프 시작 전 딱 "한 번"만 수행
        globalVolume.profile.TryGet<UnityEngine.Rendering.Universal.Bloom>(out var bloom);

        // 💡 [최적화 2] BGM 피치 조절 코루틴도 루프 시작 전 딱 "한 번"만 실행
        if (show)
        {
            SoundManager.Instance.SetBGMPitch(0.8f); // BGM 피치 다운
        }
        else
        {
            SoundManager.Instance.SetBGMPitch(1f); // BGM 피치 복구
        }

        // 💡 이제 while문 내부에는 순수 보간 연산만 남아서 엄청나게 가벼워집니다.
        while (time < duration)
        {
            float t = time / duration; // 애니메이션 진행 비율 계산
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t); // 위치 보간
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t); // 투명도

            // Bloom Tint 보간만 매 프레임 처리 (가비지 생성 없음)
            if (bloom != null)
            {
                bloom.tint.value = show ? Color.Lerp(Color.white, Color.red, t)
                                        : Color.Lerp(Color.red, Color.white, t);
            }

            time += Time.unscaledDeltaTime; // 시간 증가
            yield return null; // 다음 프레임까지 대기
        }

        rectTransform.anchoredPosition = endPosition; // 최종 위치 설정
        canvasGroup.alpha = endAlpha; // 최종 투명도 설정

        // 최종 프레임에서 한 번 더 확실하게 색상 고정
        if (bloom != null)
        {
            bloom.tint.value = show ? Color.red : Color.white;
        }

        if (!show)
        {
            canvasGroup.interactable = false; // 상호작용 불가능 설정
            canvasGroup.blocksRaycasts = false; // 레이캐스트 차단 해제
        }
        isAnimating = false; // 애니메이션 종료
    }

    public void AddRuneImage(string rune)
    {
        if (!runeSprites.ContainsKey(rune))
        {
            Debug.LogError($"Rune prefab for {rune} 찾을 수 없습니당.");
            return;
        }
        Debug.Log($"룬 이미지 추가: {rune}"); // 디버그 로그 출력

        GameObject runeObj = Instantiate(runePrefab, comboPanel);

        // 💡 [레이아웃 정렬 안전성 확보] 
        // 모스 부호들을 풀에서 release/get 하면서 Sibling Index가 꼬이는 현상을 완전히 방지하기 위해,
        // 새로 생성된 룬 오브젝트도 확실하게 UI 레이아웃 맨 뒤(오른쪽)로 밀어줍니다.
        runeObj.transform.SetAsLastSibling();

        Image img = runeObj.GetComponent<Image>();
        if (img != null)
        {
            Debug.Log($"룬 스프라이트 설정: {rune}"); // 디버그 로그 출력
            img.sprite = runeSprites[rune]; // 룬 스프라이트 설정
        }
        else
        {
            Debug.LogError("룬 오브젝트에 Image 컴포넌트가 없습니다.");
        }

        spawnedCombo.Add(runeObj); // 생성된 룬 오브젝트를 리스트에 추가
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

        RectTransform rectTransform = comboPanel.GetComponent<RectTransform>();

        visiblePosition = rectTransform.anchoredPosition; // 표시할 위치를 현재 위치로 설정
        hiddenPosition = new Vector2(visiblePosition.x, -3f); // 숨길 위치를 현재 위치에서 y 좌표만 변경

        rectTransform.anchoredPosition = hiddenPosition; // 초기 위치를 숨길 위치로 설정
        canvasGroup.alpha = 0f; // 초기 투명도 설정
        canvasGroup.interactable = false; // 초기 상호작용 불가능 설정
        canvasGroup.blocksRaycasts = false; // 초기 레이캐스트 차단 설정
    }
}
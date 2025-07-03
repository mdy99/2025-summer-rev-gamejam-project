using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [Header("스킬 정의 목록")]
    [SerializeField] private List<SkillData> skillDatas; // 스킬 데이터 리스트 (룬과 스킬 매핑)
    [Header("스킬 발사 위치")]
    [SerializeField] private Transform playerTransform; // 스킬 데이터 리스트 (룬과 스킬 매핑)

    private SkillBook skillBook; // 스킬북 (룬과 스킬 매핑)
    [SerializeField] private RuneInfoDatabase runeInfoDatabase; // 룬 정보 데이터베이스

    private KeyCode MORSE_KEY = KeyCode.A; // 콤보 입력 키 (모스 부호 입력 키)
    private KeyCode ENTER_KEY = KeyCode.W; // 콤보 제출 키 (엔터키)

    private ComboInputHandler comboInputHandler; // 콤보 입력 핸들러
    private MorseTranslator morseTranslator; // 모스 부호 변환기
    private ComboResultHandler comboResultHandler; // 콤보 결과 핸들러

    private ComboUIRenderer comboUIRenderer; // 콤보 UI 렌더러
    private KeyPressedDetector keyPressedDetector; // 키 입력 탐지기

    private List<string> activeRunes = new List<string>(); // 현재 활성화된 룬 리스트

    [SerializeField] private ParticleSystem magicCircleEffect; // 마법진 이펙트

    void Start()
    {
        magicCircleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        if(Input.GetKeyDown(ENTER_KEY)) // 스페이스바를 눌렀을 때
        {
            SubmitCombo(); // 현재 콤보 제출
        }

        if(Input.GetMouseButtonUp(0)&& comboUIRenderer.IsPanelActive()) // 마우스 왼쪽 버튼을 눌렀을 때
        {
            Debug.Log("Left mouse button pressed, 현재 콤보를 취소합니다."); // 디버그 로그 출력
            EndCombo(); // 스킬 사용 종료
        }

        if(Input.GetMouseButtonUp(1)&& comboUIRenderer.IsPanelActive()) // 마우스 오른쪽 버튼을 떼었을 때
        {
            if(comboInputHandler.IsEmpty()) // 현재 콤보가 비어있으면
            {
                TryCastSkill(); // 스킬 사용 시도
            }
            Debug.Log("Right mouse button released, 콤보에 맞는 스킬을 사용합니다."); // 디버그 로그 출력
            EndCombo(); // 스킬 사용 종료
        }
    }

    void TryCastSkill(){
        string runeKey = string.Join("", activeRunes); // 현재 활성화된 룬들을 문자열로 결합
        Debug.Log("Trying to cast skill with rune key: " + runeKey); // 디버그 로그 출력

        int totalDamage = 0; // 총 데미지 초기화
        int totalMpCose = 0; // 총 마나 소모 초기화

        foreach(string rune in activeRunes) // 활성화된 룬들에 대해
        {
            if(runeInfoDatabase.ToDictionary().TryGetValue(rune, out RuneInfo runeInfo)) // 룬 정보 데이터베이스에서 룬 정보를 찾음
            {
                totalDamage += runeInfo.damage; // 룬의 데미지를 총 데미지에 더함
                totalMpCose += runeInfo.mpCost; // 룬의 마나 소모를 총 마나 소모에 더함
            }
            else
            {
                Debug.LogWarning("Rune info not found for rune: " + rune); // 경고 로그 출력
            }
        }
        Debug.Log($"Total Damage: {totalDamage}, Total MP Cost: {totalMpCose}"); // 총 데미지와 마나 소모 로그 출력

        if(skillBook.TryGetSkill(runeKey, out ISkill skill)) // 스킬북에서 룬에 해당하는 스킬을 찾음
        {
            skill.Execute(); // 스킬 사용
        }
        else
        {
            Debug.LogWarning("No skill found for rune key: " + runeKey); // 경고 로그 출력
            NarrationText.Instance.UpdateNarration("해당 룬 조합을 메모라이즈하지 않았습니다.",Color.white); // 룬에 대한 스킬이 없다는 메시지 표시
        }
    }

    void EndCombo(){
        ClearCombo(); // 현재 쌓여있는 콤보들 다 제거
        comboUIRenderer.SetPanelActive(false); // 콤보 UI 패널 비활성화
        magicCircleEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnEnable()
    {
        keyPressedDetector.OnSymbolDetected += AddSymbol; // 심볼 감지 이벤트에 콜백 등록
    }
    void OnDisable()
    {
        keyPressedDetector.OnSymbolDetected -= AddSymbol; // 심볼 감지 이벤트에서 콜백 제거
    }

    // Start is called before the first frame update
    void Awake()
    {
        comboInputHandler = new ComboInputHandler(); // 콤보 입력 핸들러 초기화
        comboUIRenderer = GetComponent<ComboUIRenderer>();
        morseTranslator = new MorseTranslator(); // 모스 부호 변환기 초기화 
        comboResultHandler = new ComboResultHandler(comboUIRenderer); // 콤보 결과 핸들러 초기화
        keyPressedDetector = GetComponent<KeyPressedDetector>(); // 키 입력 탐지기 초기화
        keyPressedDetector.KeySetting(MORSE_KEY); // 키 입력 탐지기에 모스 부호 입력 키 설정
        skillBook = new SkillBook(skillDatas, ()=> playerTransform.position, runeInfoDatabase.ToDictionary()); // 스킬북 초기화
    }

    void AddSymbol(string symbol)
    {
        Debug.Log("AddSymbol called with symbol: " + symbol); // 디버그 로그 출력
        ActivePanel(); // 콤보 UI 패널 활성화
        comboInputHandler.Add(symbol); // 콤보 입력 핸들러에 심볼 추가
        comboUIRenderer.AddSymbolImage(symbol); // 콤보 UI 렌더러에 심볼 이미지 추가
    }

    void ActivePanel(){
        if(comboUIRenderer.IsPanelActive()==false) // 콤보 UI 패널이 활성화되어 있지 않으면
        {
            comboUIRenderer.SetPanelActive(true); // 콤보 UI 패널 활성화
            magicCircleEffect.Play(); // 마법진 이펙트 재생
        }
    }

    void RemoveMorse(){
        comboInputHandler.Clear(); // 콤보 입력 핸들러의 현재 콤보 제거
        comboUIRenderer.RemoveMorseImage(); // 콤보 UI 렌더러의
    }

    void ClearCombo(){
        comboInputHandler.Clear(); // 콤보 입력 핸들러의 현재 콤보 제거
        activeRunes.Clear(); // 활성화된 룬 리스트 초기화
        comboUIRenderer.ClearComboImage(); // 콤보 UI 렌더러의 모든 콤보 이미지 제거
    }

    void SubmitCombo(){
        string morse = comboInputHandler.GetCurrentCombo(); // 현재 입력된 콤보 문자열 가져오기
        string rune = morseTranslator.TranslateToRune(morse); // 모스 부호를 룬으로 변환

        if(rune != null)
        {
            comboResultHandler.AddRune(rune); // 룬 이미지 콤보 UI에 추가   
            activeRunes.Add(rune); // 활성화된 룬 리스트에 추가
        }
        else
        {
            // 유효하지 않은 모스 부호 처리 공간
            Debug.LogWarning("Invalid morse code: " + morse); // 유효하지 않은 모스 부호 경고
        }
        RemoveMorse(); // 모스부호 입력만 제거
    }
}

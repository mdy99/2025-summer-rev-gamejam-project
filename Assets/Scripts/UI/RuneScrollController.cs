using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class RuneScrollController : MonoBehaviour
{
    [SerializeField] private Transform runeLine1; // 룬 스크롤 컨텐츠
    [SerializeField] private Transform runeLine2; // 룬 스크롤 컨텐츠
    [SerializeField] private Transform runeLine3; // 룬 스크롤 컨텐츠
    
    private Dictionary<SkillType,Transform> skillLines;
    private MorseTranslator morseTranslator = new MorseTranslator();

    private Coroutine updateCoroutine;

    private void Awake()
    {
        skillLines = new Dictionary<SkillType, Transform>
        {
            { SkillType.NOVICE, runeLine1 },
            { SkillType.APPRENTICE, runeLine2 },
            { SkillType.ADEPT, runeLine3 }
        };
    }

    IEnumerator Start()
    {
        yield return null; // 다음 프레임까지 대기
        if(RuneManager.Instance != null){
            RuneManager.Instance.OnRuneChanged+= UpdateRuneScroll;
        }
        if(WaveManager.Instance != null){
            WaveManager.Instance.OnMemorizedSkillsChanged += UpdateRuneScroll; // 기억된 스킬이 변경될 때 룬 스크롤 업데이트
        }

        while(WaveManager.Instance == null || WaveManager.Instance.MemorizedSkills == null)
        {
            yield return null;
        }
        UpdateRuneScroll();
    }

    private void OnEnable()
    {
        if(RuneManager.Instance != null){
            RuneManager.Instance.OnRuneChanged+= UpdateRuneScroll;
        }
        if(WaveManager.Instance != null){
            WaveManager.Instance.OnMemorizedSkillsChanged += UpdateRuneScroll; // 기억된 스킬이 변경될 때 룬 스크롤 업데이트
        }
        
        StartCoroutine(DelayedUpdate()); // 룬 스크롤 업데이트를 지연시킴
    }

    private void OnDisable()
    {
        if(RuneManager.Instance != null){
            RuneManager.Instance.OnRuneChanged-= UpdateRuneScroll;
        }
        if(WaveManager.Instance != null){
            WaveManager.Instance.OnMemorizedSkillsChanged -= UpdateRuneScroll; // 기억된 스킬이 변경될 때 룬 스크롤 업데이트
        }
    }

    void OnDestroy()
    {
        if(RuneManager.Instance != null){
            RuneManager.Instance.OnRuneChanged-= UpdateRuneScroll;
        }
        if(WaveManager.Instance != null){
            WaveManager.Instance.OnMemorizedSkillsChanged -= UpdateRuneScroll; // 기억된 스킬이 변경될 때 룬 스크롤 업데이트
        }
    }

    public void UpdateRuneScroll(){
        Debug.Log("Updating Rune Scroll");
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        if (WaveManager.Instance == null || WaveManager.Instance.MemorizedSkills == null)
        {
            Debug.LogWarning("WaveManager.Instance or MemorizedSkills is null.");
            updateCoroutine = StartCoroutine(DelayedUpdate()); // 룬 스크롤 업데이트를 지연시킴
            return;
        }

        List<SkillData> skills = WaveManager.Instance.MemorizedSkills;
        foreach (SkillData skill in skills)
        {
            if(!skillLines.TryGetValue(skill.skillType, out Transform line)) continue;

            int runeCount = skill.runeCode.Length;
            int panelCount = line.childCount -1;

            for(int i=0;i<panelCount;i++)
            {
                Transform runePanel = line.GetChild(i+1); // 현재 라인의 i번째 자식 오브젝트
                if(i<runeCount){
                    string runeChar = skill.runeCode[i].ToString();
                    runePanel.gameObject.SetActive(true);
                    UpdateRunePanel(runePanel, runeChar);
                }
                else{
                    runePanel.gameObject.SetActive(false);
                }
            }
        }
    }

    private IEnumerator DelayedUpdate(){
        yield return null; // 다음 프레임까지 대기
        UpdateRuneScroll(); // 룬 스크롤 업데이트
    }

    private void UpdateRunePanel(Transform panel, string runeName){
        Sprite runeSprite = LoadRuneSprite(runeName); // 룬 스프라이트 로드
        if(runeSprite == null) return; // 스프라이트가 없으면 중단

        var imgTr = panel.Find("RuneImage");
        var txtTr = panel.Find("RuneText");
        var dmgTr = panel.Find("RuneDamage");
        var costTr = panel.Find("RuneCost");

        if(imgTr == null || txtTr == null || dmgTr == null || costTr == null){
            Debug.LogError($"Missing child in panel: {panel.name}");
            return;
        }

        imgTr.GetComponent<Image>().sprite = runeSprite;

        string morse = morseTranslator.TranslateToRuneReverse(runeName);
        txtTr.GetComponent<TMP_Text>().text = morse ?? "?";

        int damage = RuneManager.Instance != null ? RuneManager.Instance.GetDamage(runeName) : 0;
        int mpCost = RuneManager.Instance != null ? RuneManager.Instance.GetMpCost(runeName) : 0;

        panel.Find("RuneDamage").GetComponent<TMP_Text>().text = damage.ToString();
        panel.Find("RuneCost").GetComponent<TMP_Text>().text = mpCost.ToString();
    }

    private Sprite LoadRuneSprite(string runeName)
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("RuneSprites/rune_sheet"); // 룬 스프라이트 리소스 폴더에서 로드
        foreach(var sprite in allSprites){
            if(sprite.name == runeName) return sprite; // 이름이 일치하는 스프라이트 반환
        }
        Debug.LogWarning($"Rune sprite not found: {runeName}"); // 스프라이트가 없는 경우 경고 메시지 출력
        return null; // 스프라이트가 없으면 null 반환
    }
}
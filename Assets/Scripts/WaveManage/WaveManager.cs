using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WaveState
{
    InWave, // 적 스폰 중
    RewardTime, // 보상 시간
    Finished // 스폰 완료
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; } // 싱글톤 인스턴스

    [SerializeField] private int totalWaves = 7; // 총 웨이브 수
    [SerializeField] private List<WaveData> waveSettings = new List<WaveData>();

    [SerializeField] private List<SkillData> memorizedSkills = new List<SkillData>(); // 기억된 스킬 데이터

    public int currentWave=0; // 현재 웨이브 번호
    private float timer = 0f; // 웨이브 스폰 타이머

    public WaveState CurrentState { get; private set; } // 현재 웨이브 상태

    private RewardPanelController rewardPanelController; // 보상 패널 컨트롤러

    public EnemySpawner enemySpawner;

    public int killCount=0; // 현재 웨이브에서 처치한 적의 수

    public string ReinforceRandomSkillDamage(int amount){
        if(memorizedSkills.Count == 0) return null; // 기억된 스킬이 없으면 함수 종료

        SkillData randomSkill = memorizedSkills[Random.Range(0, memorizedSkills.Count)]; // 랜덤으로 스킬 선택
        if(randomSkill.runeCode.Count() == 0) return null; // 선택된 스킬에 룬 코드가 없으면 함수 종료

        string randomRuneCode = randomSkill.runeCode[Random.Range(0, randomSkill.runeCode.Length)].ToString(); // 랜덤으로 룬 코드 선택

        RuneManager runeManager = FindObjectOfType<RuneManager>(); // 룬 매니저 찾기
        if(runeManager != null){
            runeManager.ReinforceRune(randomRuneCode, amount, 0); // 룬 강화
        }
        return randomRuneCode; // 강화된 룬 코드 반환
    }

    public void OnEnemyKilled(){
        killCount++;
        Debug.Log($"Enemy killed! Current kill count: {killCount}");
        if(killCount >= waveSettings[currentWave-1].enemyGoalTotal){
            EndCurrentWave();
        }
    }

    void Awake()
    {
        if(Instance == null) Instance = this; // 싱글톤 인스턴스 설정
        else Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 오브젝트 제거
    }

    void Start(){
        rewardPanelController = GetComponent<RewardPanelController>();
        StartNextWave(); // 첫 번째 웨이브 시작
    }

    void Update()
    {
        if(CurrentState != WaveState.InWave) return; // 현재 상태가 웨이브 스폰 중이 아닐 경우 업데이트 중지

        // 웨이브 중일 때,
        SpawnEnemyInWave(); // 적 스폰 함수 호출        
    }

    void SpawnEnemyInWave(){
        timer += Time.deltaTime;
        if (timer >= enemySpawner.SpawnInterval)
        {
            WaveData currentWaveData = waveSettings[currentWave - 1]; // 현재 웨이브 데이터 가져오기
            EnemyType selectedType = GetRandomEnemyType(currentWaveData.enemySpawnRatios); // 현재 웨이브에 맞는 적 타입 랜덤 선택
            enemySpawner.SpawnEnemyByType(selectedType); // 적 스폰 함수 호출
            timer = 0f; // 타이머 초기화
        }

    }

    EnemyType GetRandomEnemyType(List<EnemySpawnRatio> ratios)
    {
        float rand = UnityEngine.Random.value; // 0~1 사이의 랜덤 값 생성
        float cumulative =0f; // 누적 확률 초기화
        
        foreach(var ratio in ratios){
            cumulative += ratio.spawnRatio;
            if(rand <= cumulative){
                return ratio.enemyType;
            }
        }
        return ratios[ratios.Count - 1].enemyType; // 모든 확률을 합쳐도 랜덤 값이 범위를 벗어날 경우 마지막 타입 반환
    }

    private void StartNextWave()
    {
        killCount = 0; // 웨이브 시작 시 처치 카운트 초기화
        enemySpawner.DisableAllEnemies(); // 모든 적 비활성화
        
        if (CurrentState == WaveState.InWave && rewardPanelController.GetActive())
        {
            rewardPanelController.ClosePanel(); // 보상 패널 닫기
        }
        if (currentWave < totalWaves)
        {
            currentWave++;
            if(currentWave > totalWaves){
                CurrentState = WaveState.Finished; // 모든 웨이브가 끝났을 때
                Debug.Log("All waves completed!");
                return;
            }

            Debug.Log($"Starting Wave {currentWave}");
            CurrentState = WaveState.InWave; // 웨이브 스폰 상태로 변경

            // TODO: 웨이브 시작 로직 추가
            enemySpawner.AdjustPoolSizeForWave(waveSettings[currentWave - 1]); // 적 풀 사이즈 조정
        }
    }

    private void EndCurrentWave()
    {
        CurrentState = WaveState.RewardTime; // 현재 웨이브 종료 상태로 변경
        rewardPanelController.SetupRewardSlots(memorizedSkills); // 보상 슬롯 설정
        rewardPanelController.ResetPanel(); // 보상 패널 초기화
        rewardPanelController.OpenPanel(); // 보상 패널 열기
        Debug.Log($"Wave {currentWave} completed!");
    }

    public void OnRewardComplete(){
        Debug.Log($"Rewards for Wave {currentWave} 종료 다음 웨이브 진행");
        rewardPanelController.ClosePanel(); // 보상 패널 닫기
        CurrentState = WaveState.InWave; // 웨이브 스폰 상태로 변경
        StartNextWave(); // 다음 웨이브 시작
    }

}

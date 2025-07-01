using System.Collections;
using System.Collections.Generic;
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
    
    public int currentWave=0; // 현재 웨이브 번호
    private float timer = 0f; // 웨이브 스폰 타이머

    public WaveState CurrentState { get; private set; } // 현재 웨이브 상태

    private RewardPanelController rewardPanelController; // 보상 패널 컨트롤러

    public EnemySpawner enemySpawner;

    public int killCount=0; // 현재 웨이브에서 처치한 적의 수

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
        // TODO: 현재 웨이브 종료 조건 추가 
        
    }

    void SpawnEnemyInWave(){
        timer += Time.deltaTime;
        if (timer >= enemySpawner.SpawnInterval)
        {
            enemySpawner.SpawnEnemy();
            timer = 0f; // 타이머 초기화
        }

    }

    private void StartNextWave()
    {
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
        }
    }

    private void EndCurrentWave()
    {
        CurrentState = WaveState.RewardTime; // 현재 웨이브 종료 상태로 변경
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

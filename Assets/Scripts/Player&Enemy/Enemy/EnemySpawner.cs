using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 참조하기 위한 변수
    public float spawnRadius = 5f; // 적 생성 반경

    [Header("몬스터 프리팹들")]
    public EnemyData[] enemyDatas; // 몬스터 데이터 배열
    [SerializeField]private int poolSize = 10; // 풀의 크기

    private List<GameObject> enemyPool = new List<GameObject>(); // 적 풀
    private Dictionary<EnemyType, Queue<GameObject>> enemyPools = new Dictionary<EnemyType, Queue<GameObject>>(); // 적 타입별 풀

    [Header("보스 프리팹")]
    public GameObject midBossPrefab; // 중간 보스 프리팹
    public GameObject finalBossPrefab; // 최종 보스 프리팹

    private float spawnInterval = 1.2f; // 적 생성 간격

    public float SpawnInterval{
        get => spawnInterval;
        set{
            if(value <= 0) {
                Debug.LogWarning("Spawn interval must be greater than 0. Setting to default value of 2.0f.");
                return;
            }
            spawnInterval = value; // 적 생성 간격 설정
        }
    }

    private float timer;

    [Header("맵 경계")]
    private float minX = -29f; // 생성 위치의 최소 x 좌표
    private float maxX = 29f; // 생성 위치의 최대 x 좌표
    private float minY = -17f; // 생성 위치의 최소 y 좌표
    private float maxY = 17f; // 생성 위치의 최대 y 좌표



    public void AdjustPoolSizeForWave(WaveData waveData){
        foreach(var ratio in waveData.enemySpawnRatios){
            EnemyType type = ratio.enemyType; // 적 타입
            int desiredCount = Mathf.CeilToInt(waveData.enemyCount * ratio.spawnRatio); // 해당 타입의 적 수

            if(!enemyPools.ContainsKey(type)){
                enemyPools[type] = new Queue<GameObject>(); // 해당 타입의 풀 생성
            }

            Queue<GameObject> pool = enemyPools[type];
            int currentCount = pool.Count; // 현재 풀에 있는 적 수

            int needToAdd = desiredCount - currentCount; // 추가해야 할 적 수
            if(needToAdd > 0){
                EnemyData data = GetEnemyDataByType(type); // 해당 타입의 적 데이터 가져오기
                for(int i=0;i<needToAdd;i++)
                {
                    GameObject enemy = Instantiate(data.enemyPrefab); // 적 프리팹 인스턴스화
                    enemy.SetActive(false); // 비활성화 상태로 설정
                    enemy.GetComponent<IEnemy>().Init(data); // 적 초기화
                    pool.Enqueue(enemy); // 풀에 추가
                    enemyPool.Add(enemy); // 전체 적 풀에도 추가
                }
            }
            else if(needToAdd < 0){
                // 현재 풀에 있는 적이 너무 많으면 일부 제거
                for(int i=0;i<Mathf.Abs(needToAdd);i++)
                {
                    if(pool.Count > 0){
                        GameObject enemy = pool.Dequeue(); // 풀에서 제거
                        enemy.SetActive(false); // 비활성화 상태로 설정
                        enemyPool.Remove(enemy); // 전체 적 풀에서도 제거
                        Destroy(enemy); // 게임 오브젝트 삭제
                    }
                }
            }
        }
    }

    private EnemyData GetEnemyDataByType(EnemyType type){
        foreach(EnemyData data in enemyDatas){
            if(data.enemyType == type){
                return data; // 해당 타입의 적 데이터 반환
            }
        }
        Debug.LogWarning($"Enemy data for type {type} not found.");
        return null; // 해당 타입의 적 데이터가 없으면 null 반환
    }

    public void SpawnEnemyByType(EnemyType type){
        if(!enemyPools.ContainsKey(type)|| enemyPools[type].Count == 0)
        {
            Debug.LogWarning($"Enemy type {type} not found in pools or pool is empty.");
            return; // 해당 타입의 적이 없거나 풀에 비활성화된 적이 없으면 함수 종료
        }

        foreach(GameObject enemy in enemyPools[type])
        {
            if (!enemy.activeInHierarchy)
            {
                Vector2 spawnPosition = GetValidSpawnPosition(); // 유효한 생성 위치를 가져옴
                if(spawnPosition == Vector2.negativeInfinity) return; // 유효한 위치가 없으면 함수 종료

                enemy.GetComponent<IEnemy>().Init(GetEnemyDataByType(type)); // 적 초기화
                enemy.transform.position = spawnPosition; // 적의 위치 설정
                enemy.SetActive(true); // 적 활성화

                //(enemy.GetComponent<IEnemy>() as Enemy)?.StartFadeIn(); // 딜레이 후 스프라이트 활성화
                (enemy.GetComponent<IEnemy>() as Enemy)?.DelayedShowSprite(); // 딜레이 후 스프라이트 활성화
                return; // 하나의 적만 생성하고 함수 종료
            }
        }
    }

    public void IncreaseAllEnemiesStatus(){
        foreach(GameObject enemy in enemyPool){
            IEnemy enemyComponent = enemy.GetComponent<IEnemy>();
            if(enemyComponent != null && enemy.activeInHierarchy)
            {
                (enemyComponent as Enemy)?.IncreaseStatus(); // 적의 체력과 공격력 증가
            }
            else
            {
                Debug.LogWarning($"Enemy component not found or enemy is not active: {enemy.name}");
            }
        }
    }

    public void DisableAllEnemies(){
        foreach(var pool in enemyPools.Values)
        {
            foreach(GameObject enemy in pool)
            {
                if(enemy.activeInHierarchy)
                {
                    enemy.SetActive(false); // 모든 적 비활성화
                }
            }
        }
    }

    Vector2 GetValidSpawnPosition(){
        for(int i=0;i<30;i++) // 최대 30번 시도
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            if(Vector2.Distance(spawnPosition, player.position) >= spawnRadius &&
               spawnPosition.x >= minX && spawnPosition.x <= maxX &&
               spawnPosition.y >= minY && spawnPosition.y <= maxY)
            {
                return spawnPosition; // 유효한 위치 반환
            }
        }
        return Vector2.negativeInfinity; // 유효한 위치를 찾지 못하면 무한대 반환
    }

    public void SpawnBoss(GameObject bossPrefab)
    {
        SoundManager.Instance.PlaySFX("SpawnBoss");
        Vector2 bossPosition = new Vector2(0, maxY - 2f);
        GameObject boss = Instantiate(bossPrefab, bossPosition, Quaternion.identity);

        // ✅ EnemyData 초기화
        BossEnemy bossEnemy = boss.GetComponent<BossEnemy>();
        EnemyData bossData = GetEnemyDataByType(EnemyType.Boss); // EnemyType.Boss에 해당하는 데이터
        if (bossEnemy != null && bossData != null)
        {
            bossEnemy.Init(bossData);
            bossEnemy.DelayedShowSprite(); // 딜레이 후 스프라이트 활성화
        
        }

        if(WaveManager.Instance.currentWave == 7){
            bossData.hp += bossData.increaseHp;
            bossData.atk += bossData.increaseAtk;
        }
    }
}
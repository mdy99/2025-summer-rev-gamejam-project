using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 참조하기 위한 변수
    public float spawnRadius = 5f; // 적 생성 반경

    [Header("몬스터 프리팹들")]
    public EnemyData[] enemyDatas; // 몬스터 데이터 배열
    [SerializeField]private int poolSize = 10; // 풀의 크기

    private List<GameObject> enemyPool = new List<GameObject>(); // 적 풀

    [Header("보스 프리팹")]
    public GameObject midBossPrefab; // 중간 보스 프리팹
    public GameObject finalBossPrefab; // 최종 보스 프리팹

    private float spawnInterval = 2.0f; // 적 생성 간격

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

    // Start is called before the first frame update
    void Start()
    {
        AdjustPoolSize();
    }

    void AdjustPoolSize(){
        int currentSize = enemyPool.Count;

        if(currentSize < poolSize){
            int diff = poolSize - currentSize;

            for (int i = 0; i < diff; i++)
            {
                int randIndex = Random.Range(0,enemyDatas.Length); // 랜덤으로 몬스터 프리팹 선택
                EnemyData data = enemyDatas[randIndex]; // 선택한 몬스터 데이터

                GameObject enemy = Instantiate(data.enemyPrefab); // 선택한 프리팹으로 적 생성
                enemy.SetActive(false); // 초기에는 비활성화

                IEnemy enemyComponent = enemy.GetComponent<IEnemy>();
                enemyComponent.Init(data); // 적 초기화
                enemyPool.Add(enemy);
            }
        }
        else if(currentSize > poolSize){
            int diff = currentSize - poolSize;
            
            for(int i = 0; i < diff; i++) {
                GameObject enemyToRemove = enemyPool[enemyPool.Count - 1];
                enemyPool.RemoveAt(enemyPool.Count - 1);
                Destroy(enemyToRemove);
            }
        }
    }

    public void SpawnEnemy()
    {
        // 비활성화된 적 오브젝트를 찾음
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                Vector2 spawnPosition = GetValidSpawnPosition(); // 유효한 생성 위치를 가져옴
                if(spawnPosition == Vector2.negativeInfinity) return; // 유효한 위치가 없으면 함수 종료

                enemy.transform.position = spawnPosition; // 적의 위치 설정
                enemy.GetComponent<IEnemy>().Init(enemyDatas[Random.Range(0, enemyDatas.Length)]); // 적 초기화
                enemy.SetActive(true); // 적 활성화
                return; // 하나의 적만 생성하고 함수 종료
            }
        }
    }

    public void DisableAllEnemies()
    {
        foreach (GameObject enemy in enemyPool)
        {
            if (enemy.activeInHierarchy)
            {
                enemy.SetActive(false); // 모든 적 비활성화
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

    void SpawnBoss(GameObject bossPrefab)
    {
            Vector2 bossPosition = new Vector2(0,maxY-2f);
            Instantiate(bossPrefab, bossPosition, Quaternion.identity); // 보스 생성
    }
}

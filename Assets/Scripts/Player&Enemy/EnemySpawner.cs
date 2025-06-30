using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 참조하기 위한 변수
    public float spawnRadius = 5f; // 적 생성 반경
    public GameObject enemyPrefab; // 적 프리팹
    public int poolSize = 10; // 풀의 크기
    private List<GameObject> enemyPool = new List<GameObject>(); // 적 풀

    public float spawnInterval = 2.0f; // 적 생성 간격
    private float timer;

    private float minX = -30f; // 생성 위치의 최소 x 좌표
    private float maxX = 30f; // 생성 위치의 최대 x 좌표
    private float minY = -20f; // 생성 위치의 최소 y 좌표
    private float maxY = 20f; // 생성 위치의 최대 y 좌표

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false); // 초기에는 비활성화
            enemyPool.Add(enemy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f; // 타이머 초기화
        }
    }

    void SpawnEnemy()
    {
        // 비활성화된 적 오브젝트를 찾음
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                Vector2 spawnPosition;
                int attempts = 0; // 위치 생성 시도 횟수
                do{
                // 랜덤 위치 생성
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);
                spawnPosition = new Vector2(randomX, randomY);
                
                attempts++; // 시도 횟수 증가
                if (attempts>30) return; // 30번 시도했는데도 적절한 위치를 찾지 못하면 함수 종료

                }while(Vector2.Distance(spawnPosition, player.position) < spawnRadius); // 플레이어와의 거리가 spawnRadius보다 작으면 다시 시도

                enemy.transform.position = spawnPosition; // 위치 설정
                enemy.SetActive(true); // 적 활성화
                return; // 하나의 적만 생성하고 함수 종료
            }
        }
    }
}

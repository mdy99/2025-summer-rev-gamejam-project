using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaveState
{
    Waiting, // 대기 중
    Spawning, // 적 스폰 중
    Finished // 스폰 완료
}

public class WaveManager : MonoBehaviour
{

    public static WaveManager Instance { get; private set; } // 싱글톤 인스턴스

    [SerializeField] private int totalWaves = 7; // 총 웨이브 수
    [SerializeField] private List<WaveData> waveSettings = new List<WaveData>();

    private Dictionary<int, int> enemyCountsPerWave = new Dictionary<int, int>(); // 웨이브당 적의 수를 저장하는 딕셔너리


    void Start(){
        InitWaves(); // 웨이브 초기화
    }

    void InitWaves(){
        enemyCountsPerWave.Clear(); // 딕셔너리 초기화
        foreach (var waveData in waveSettings)
        {
            if (!enemyCountsPerWave.ContainsKey(waveData.waveNumber))
            {
                enemyCountsPerWave[waveData.waveNumber] = waveData.enemyCount; // 웨이브 번호와 적의 수를 딕셔너리에 추가
            }
            else
            {
                Debug.LogWarning($"Wave {waveData.waveNumber} already exists. Skipping duplicate entry.");
            }
        }
    }
    
}

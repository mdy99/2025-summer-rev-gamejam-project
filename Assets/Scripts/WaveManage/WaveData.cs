using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public int waveNumber; // 웨이브 번호
    public int enemyCount; // 해당 웨이브에서 오브젝트 풀 최대 스폰될 적의 수
    public int enemyGoalTotal; // 해당 웨이브에서 처치해야 할 적의 총 수
}

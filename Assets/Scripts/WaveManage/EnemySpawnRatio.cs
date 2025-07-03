using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class EnemySpawnRatio
{
    public EnemyType enemyType; // 적의 타입 (슬라임, 문어, 코끼리 등)
    [Range (0f,1f)]public float spawnRatio; // 해당 적이 스폰될 확률 (0~100)
    
    public EnemySpawnRatio(EnemyType type, int ratio)
    {
        enemyType = type;
        spawnRatio = ratio;
    }
}

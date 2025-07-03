using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItemInfo{
    public ItemData itemData; // 드랍할 아이템 데이터
    [Range(0f, 1f)] public float dropChance; // 아이템 드랍 확률 (0~1 사이)
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    
    public string enemyName; // 적의 이름
    public string description; // 적의 설명
    public int hp; // 적의 체력
    public int atk; // 적의 공격력
    public float speed; // 적의 이동 속도
    public float attackCooldown; // 공격 쿨타임
    public int rewardMp; // 처치 시 플레이어에게 지급되는 마나

    public EnemyType enemyType; // 적의 타입 (슬라임, 문어, 코끼리 등)
    public GameObject enemyPrefab; // 적의 프리팹

    public List<DropItemInfo> dropItems; // 적이 드랍할 아이템들
}


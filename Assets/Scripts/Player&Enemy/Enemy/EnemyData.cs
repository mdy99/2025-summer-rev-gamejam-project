using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //public RuntimeAnimatorController animatorController; // 적의 애니메이터 컨트롤러
    public GameObject enemyPrefab; // 적의 프리팹
}


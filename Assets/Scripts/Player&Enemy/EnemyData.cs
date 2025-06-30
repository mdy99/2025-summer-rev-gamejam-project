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
    public int exp; // 적을 처치했을 때 주는 경험치
    public float speed; // 적의 이동 속도
    public float attackCooldown; // 공격 쿨타임
    public Sprite enemySprite; // 적의 스프라이트 이미지
    public GameObject enemyPrefab; // 적의 프리팹
}


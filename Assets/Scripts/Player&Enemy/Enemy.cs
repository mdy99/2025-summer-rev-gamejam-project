using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    [Header("UI 바")]
    public BarUpdater playerHpBarUpdater; // 플레이어의 체력을 표시하는 UI 바

    [Header("컴포넌트")]
    private Rigidbody2D target; // 플레이어 오브젝트
    private SpriteRenderer spriteRenderer; // 적의 스프라이트 렌더러
    private Rigidbody2D enemyRigid; // 적의 Rigidbody2D 컴포넌트

    [Header("적의 정보")]
    private bool isLive=true; // 적이 살아있는지 여부
    private string name; // 적의 이름
    private string description; // 적의 설명
    private int hp=20; // 적의 체력
    private int atk=5; // 적의 공격력
    private int exp=20; // 적을 처치했을 때 주는 경험치
    private float speed=2.0f; // 적의 이동 속도
    private float attackCooldown = 1.0f; // 공격 쿨타임

    [Header("적의 상태")]
    private float lastAttackTime = -999f; // 마지막 공격 시간
    
    public void TakeDamage(int damage)
    {
        if (!isLive) return; // 적이 이미 죽었으면 함수 종료
        hp -= damage; // 적의 체력 감소
        Debug.Log($"Enemy {name} took {damage} damage! Remaining HP: {hp}"); // 디버그 메시지 출력

        if (hp <= 0) Die(); // 체력이 0 이하가 되면 적을 죽임
    }

    void Die()
    {
        isLive = false; // 적을 죽음 상태로 변경
        Debug.Log($"Enemy {name} has died!"); // 디버그 메시지 출력
        Destroy(gameObject); // 적 오브젝트 제거
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyRigid = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player"); // 플레이어 오브젝트를 찾음

        if (player != null) target = player.GetComponent<Rigidbody2D>(); // 플레이어의 Rigidbody2D 컴포넌트를 가져옴
        else Debug.LogWarning("Player not found!"); // 플레이어가 없으면 경고 메시지 출력
    }

    void FixedUpdate()
    {
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        TracePlayer(); // 플레이어를 추적하는 함수 호출
    }

    void LateUpdate()
    {
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        spriteRenderer.flipX = target.transform.position.x > transform.position.x; // 플레이어의 위치에 따라 적의 스프라이트 방향을 뒤집음
    }

    private void TracePlayer(){
        if(Vector2.Distance(transform.position, target.transform.position) < 0.1f) return; // 플레이어와의 거리가 너무 가까우면 이동하지 않음
        Vector2 directVector = target.transform.position - transform.position; // 플레이어 방향 벡터
        Vector2 nextVector = directVector.normalized * speed* Time.fixedDeltaTime; // 플레이어 방향으로 이동 벡터
        enemyRigid.MovePosition(enemyRigid.position + nextVector); // 플레이어 방향으로 이동

        enemyRigid.velocity = Vector2.zero; // Rigidbody2D의 속도를 0으로 설정하여 중력 영향을 받지 않도록 함
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isLive) return; // 적이 살아있지 않으면 함수 종료

        if (collision.gameObject.CompareTag("Player")) // 플레이어와 충돌했을 때
        {
            if(Time.time - lastAttackTime < attackCooldown) return; // 공격 쿨타임이 지나지 않았으면 함수 종료

            DamagePlayer(atk); // 플레이어에게 공격력만큼 피해를 줌
            lastAttackTime = Time.time; // 마지막 공격 시간을 현재 시간으로 갱신
            Debug.Log($"Enemy {name} attacked the player for {atk} damage!"); // 디버그 메시지 출력
        }
    }

    private void DamagePlayer(int damage)
    {
        playerHpBarUpdater.UpdateBar(-damage); // 플레이어의 체력 UI 바를 감소시킴
    }
}

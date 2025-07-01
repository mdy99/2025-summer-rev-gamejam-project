using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;


public class Enemy : MonoBehaviour, IEnemy
{
    [Header("컴포넌트")]
    private Rigidbody2D target; // 플레이어 오브젝트
    private SpriteRenderer spriteRenderer; // 적의 스프라이트 렌더러
    private Rigidbody2D enemyRigid; // 적의 Rigidbody2D 컴포넌트
    private Animator enemyAnimator; // 적의 애니메이터 컴포넌트
    
    [Header("데이터")]
    public EnemyData enemyData; // 적의 데이터 (체력, 공격력 등)
    private int currentHp; // 현재 적의 체력
    private bool isLive=true; // 적이 살아있는지 여부

    private float lastAttackTime = -999f; // 마지막 공격 시간
    
    

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyRigid = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
    }

    public void Init(EnemyData data)
    {
        enemyData = data; // 적의 데이터를 초기화
        currentHp = enemyData.hp; // 현재 체력을 적의 체력으로 설정
        isLive = true; // 적을 살아있는 상태로 초기화
        lastAttackTime = -999f; // 마지막 공격 시간을 초기화
        
        // if(enemyData.animatorController != null && enemyAnimator != null) // 애니메이터 컨트롤러가 설정되어 있고 애니메이터 컴포넌트가 존재하는 경우
        // {
        //     enemyAnimator.runtimeAnimatorController = enemyData.animatorController; // 애니메이터 컨트롤러 설정
        // }
        // else
        // {
        //     Debug.LogWarning("EnemyData animatorController is not set!"); // 애니메이터 컨트롤러가 설정되지 않은 경우 경고 메시지 출력
        // }

        Debug.Log($"Enemy {enemyData.enemyName} initialized with HP: {currentHp}"); // 디버그 메시지 출력
    }

    // Start is called before the first frame update
    void Start()
    {
        target =GameObject.FindWithTag("Player")?.GetComponent<Rigidbody2D>(); // 플레이어의 Rigidbody2D 컴포넌트를 가져옴\
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage; // 적의 체력 감소
        Debug.Log($"Enemy {enemyData.name} took {damage} damage! Remaining HP: {currentHp}"); // 디버그 메시지 출력

        if (currentHp <= 0) Die(); // 체력이 0 이하가 되면 적을 죽임
     //   else enemyAnimator.SetTrigger("hit"); // 적이 공격을 받았을 때 맞는 애니메이션 트리거 설정
    }

    void Die()
    {
        if (!isLive) return; // 적이 이미 죽었으면 함수 종료

        isLive = false; // 적을 죽음 상태로 변경
      //  enemyAnimator.SetTrigger("die"); // 죽음 애니메이션 트리거 설정
        StartCoroutine(DisableAfterAnimation(0.1f)); // 대기 후 적 오브젝트 비활성화
        WaveManager.Instance.OnEnemyKilled(); // 웨이브 매니저에 적 처치 알림
        GiveReward(); // 보상 지급 함수 호출
        Debug.Log($"Enemy {enemyData.name} has died!"); // 디버그 메시지 출력
    }

    void GiveReward()
    {
        BarManager.Instance.UpdateMpBar(enemyData.rewardMp); // 적 처치 시 플레이어에게 체력 보상 지급
    }

    private IEnumerator DisableAfterAnimation(float delay = 0f)
    {
//        yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length); // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(delay); // 추가 딜레이 시간 대기
        gameObject.SetActive(false); // 적 오브젝트를 비활성화
    }

    void FixedUpdate()
    {
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        TraceTarget(); // 플레이어를 추적하는 함수 호출
    }

    void LateUpdate()
    {
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        spriteRenderer.flipX = target.transform.position.x > transform.position.x; // 플레이어의 위치에 따라 적의 스프라이트 방향을 뒤집음
    }

    private void TraceTarget(){
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        if(Vector2.Distance(transform.position, target.transform.position) < 0.01f){
          //  enemyAnimator.SetBool("isMoving", false); // 플레이어와의 거리가 너무 가까우면 이동 애니메이션을 중지
            return; // 플레이어와의 거리가 너무 가까우면 이동하지 않음
        } 

//      enemyAnimator.SetBool("isMoving", true); // 플레이어와의 거리가 멀면 이동 애니메이션을 시작

        Vector2 directVector = target.transform.position - transform.position; // 플레이어 방향 벡터
        Vector2 nextVector = directVector.normalized * enemyData.speed* Time.fixedDeltaTime; // 플레이어 방향으로 이동 벡터
        enemyRigid.MovePosition(enemyRigid.position + nextVector); // 플레이어 방향으로 이동

        enemyRigid.velocity = Vector2.zero; // Rigidbody2D의 속도를 0으로 설정하여 중력 영향을 받지 않도록 함
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (!isLive || !col.gameObject.CompareTag("Player")) return; // 플레이어와 충돌하지 않았거나 적이 죽었으면 함수 종료
        if(Time.time - lastAttackTime < enemyData.attackCooldown) return; // 공격 쿨타임이 지나지 않았으면 함수 종료

        DamagePlayer(enemyData.atk); // 플레이어에게 공격력만큼 피해를 줌
    }

    private void DamagePlayer(int damage)
    {
      //  enemyAnimator.SetTrigger("attack"); // 공격 애니메이션 트리거 설정
        lastAttackTime = Time.time; // 마지막 공격 시간을 현재 시간으로 갱신
    
        BarManager.Instance.UpdateHpBar(-damage); // 플레이어의 체력 UI 바를 감소시킴
        Debug.Log($"Enemy {enemyData.name} attacked the player for {enemyData.atk} damage!"); // 디버그 메시지 출력
    }
}

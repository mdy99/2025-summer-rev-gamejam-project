using System.Collections;
using UnityEngine;

public enum EnemyType
{
    None = 0, // 적 없음
    Slime = 1, // 슬라임
    Octo=2, // 문어
    Elephant=3, // 코끼리
    Boss=4, // 보스
}

public class Enemy : MonoBehaviour, IEnemy
{
    public GameObject itemPrefab; // 아이템 프리팹

    [SerializeField] private bool isBoss = false; // 보스 여부
    
    [Header("컴포넌트")]
    protected Rigidbody2D target; // 플레이어 오브젝트
    private SpriteRenderer spriteRenderer; // 적의 스프라이트 렌더러
    protected Rigidbody2D enemyRigid; // 적의 Rigidbody2D 컴포넌트
    protected Animator enemyAnimator; // 적의 애니메이터 컴포넌트

    [Header("데이터")]
    private EnemyData enemyData; // 적의 데이터 (체력, 공격력 등)
    [SerializeField] private int currentHp; // 현재 적의 체력
    [SerializeField] private float debugSpeed; // 적의 이동 속도
    protected bool isLive=true; // 적이 살아있는지 여부

    private float lastAttackTime = -999f; // 마지막 공격 시간

    private bool isPlayerDead = false; // 플레이어가 죽었는지 여부


    void ShowDamageText(int damage, bool isPlayer = false)
    {
        Vector3 spawnPos = isPlayer ? target.position : transform.position + new Vector3(0, 1f, 0); // 피해 텍스트가 적 위에 나타나도록 위치 설정
        GameObject damageText = DamageTextPool.Instance.GetText(spawnPos, damage); // 피해 텍스트 풀에서 가져오기
        damageText.GetComponent<DamageText>().Init(damage); // 피해 텍스트에 피해량 설정
    }

    void OnEnable()
    {
        BarUpdater.OnPlayerDead += OnPlayerDead; // 플레이어가 죽었을 때 이벤트 핸들러 등록
    }

    void OnPlayerDead(){
        isPlayerDead = true; // 플레이어가 죽었음을 표시
        isLive = false; // 적을 죽음 상태로 변경
    }

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
        debugSpeed = enemyData.speed; // 디버그용 속도 설정
        isLive = true; // 적을 살아있는 상태로 초기화
        lastAttackTime = -999f; // 마지막 공격 시간을 초기화
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        target =GameObject.FindWithTag("Player")?.GetComponent<Rigidbody2D>(); // 플레이어의 Rigidbody2D 컴포넌트를 가져옴\
        if(EnemyType.Boss == enemyData.enemyType) isBoss = true; // 적의 타입이 보스인지 확인
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage; // 적의 체력 감소
        if (!isLive) return; // ✅ 죽었으면 더 이상 데미지 받지 않음
        if(isBoss){
            NarrationText.Instance.UpdateNarration($"보스 체력: {currentHp}/{enemyData.hp}", Color.red); // 보스가 데미지를 받았을 때 내레이션 텍스트 업데이트
        }
        StartCoroutine(HitFlash()); // 적이 맞았을 때 색상 변경 효과 시작
        ShowDamageText(damage); // 피해 텍스트 표시
        Debug.Log($"Enemy {enemyData.name} took {damage} damage! Remaining HP: {currentHp}"); // 디버그 메시지 출력

        if (currentHp <= 0) Die(); // 체력이 0 이하가 되면 적을 죽임
        else enemyAnimator.SetTrigger("isDamaged"); // 적이 공격을 받았을 때 맞는 애니메이션 트리거 설정
    }

    IEnumerator HitFlash(){
        spriteRenderer.color = Color.red; // 적의 색상을 빨간색으로 변경하여 맞았음을 표시
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        spriteRenderer.color = Color.white; // 적의 색상을 원래대로 되돌림
    }

    protected virtual void Die()
    {
        if (!isLive) return; // 적이 이미 죽었으면 함수 종료
        if(isBoss){
            WaveManager.Instance.OnBossKilled(); // 보스 처치 시 웨이브 매니저에 알림
        }
        SoundManager.Instance.PlaySFX("MonsterDie");
        isLive = false; // 적을 죽음 상태로 변경
        enemyAnimator.SetTrigger("isDead"); // 죽음 애니메이션 트리거 설정
        StartCoroutine(DisableAfterAnimation(0.1f)); // 대기 후 적 오브젝트 비활성화
        WaveManager.Instance.OnEnemyKilled(); // 웨이브 매니저에 적 처치 알림
        GiveReward(); // 보상 지급 함수 호출
        TryDropItem(); // 아이템 드랍 시도
        Debug.Log($"Enemy {enemyData.name} has died!"); // 디버그 메시지 출력
    }

    void TryDropItem(){
        foreach(var dropInfo in enemyData.dropItems) // 적이 드랍할 아이템들에 대해
        {
            if(Random.value < dropInfo.dropChance) // 드랍 확률에 따라 아이템 드랍 여부 결정
            {
                Vector2 offset = GetRandomDropPosition(); // 아이템 드랍 위치 오프셋 계산
                Vector2 dropPosition = (Vector2)transform.position + offset; // 드랍 위치
                SoundManager.Instance.PlaySFX("Drop");
                GameObject drop = Instantiate(itemPrefab, dropPosition, Quaternion.identity); // 아이템 프리팹 인스턴스 생성
                drop.GetComponent<ItemObject>().itemData = dropInfo.itemData; // 아이템 데이터 설정
                // break; // 아이템 하나만 드랍하고 루프 종료 지금은 여러개 드롭 가능
            }
        }
    }

    public void IncreaseStatus(){
        enemyData.hp += enemyData.increaseHp; // 적의 체력 증가
        enemyData.atk += enemyData.increaseAtk; // 적의 공격력 증가
    }

    Vector2 GetRandomDropPosition(){
        float radius = 2f;
        return Random.insideUnitCircle * radius;
    }

    void GiveReward()
    {
        BarManager.Instance.UpdateMpBar(enemyData.rewardMp); // 적 처치 시 플레이어에게 체력 보상 지급
    }

    private IEnumerator DisableAfterAnimation(float delay = 0f)
    {
        //yield return null; // 현재 애니메이션이 끝날 때까지 대기
        while(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dead")) // 애니메이션이 "Dead" 상태가 될 때까지 대기
        {
            yield return null; // 다음 프레임까지 대기
        }
        float dieAnimLength = enemyAnimator.GetCurrentAnimatorStateInfo(0).length; // 죽음 애니메이션의 길이 가져오기
        yield return new WaitForSeconds(dieAnimLength); // 애니메이션이 끝날 때까지 대기 후 추가 딜레이 시간 대기

        yield return new WaitForSeconds(delay); // 추가 딜레이 시간 대기
        gameObject.SetActive(false); // 적 오브젝트를 비활성화
    }

    protected virtual void FixedUpdate()
    {
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        TraceTarget(); // 플레이어를 추적하는 함수 호출
    }

    protected virtual void LateUpdate()
    {
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        spriteRenderer.flipX = target.transform.position.x > transform.position.x; // 플레이어의 위치에 따라 적의 스프라이트 방향을 뒤집음
    }

    private void TraceTarget(){
        if (target == null || !isLive) return; // 플레이어가 없거나 적이 죽었으면 함수 종료
        if(Vector2.Distance(transform.position, target.transform.position) < 0.01f){
            enemyAnimator.SetBool("isMoving", false); // 플레이어와의 거리가 너무 가까우면 이동 애니메이션을 중지
            return; // 플레이어와의 거리가 너무 가까우면 이동하지 않음
        } 

        enemyAnimator.SetBool("isMoving", true); // 플레이어와의 거리가 멀면 이동 애니메이션을 시작

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
        if (isPlayerDead) return; // 플레이어가 죽었으면 함수 종료
        lastAttackTime = Time.time; // 마지막 공격 시간을 현재 시간으로 갱신
        SoundManager.Instance.PlaySFX("DamagePlayer");
        BarManager.Instance.UpdateHpBar(-damage); // 플레이어의 체력 UI 바를 감소시킴
        ShowDamageText(damage,true); // 피해 텍스트 표시
        Debug.Log($"Enemy {enemyData.name} attacked the player for {enemyData.atk} damage!"); // 디버그 메시지 출력
    }
}

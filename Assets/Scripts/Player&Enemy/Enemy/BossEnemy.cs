using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("보스 전용 설정")]
    public float chargeSpeed = 18f;
    public float chargeCooldown = 10f;
    public float chargeDuration = 1.5f;
    public float chargeDelay = 1f;

    private bool isCharging = false;
    private Vector2 chargeTargetPos;
    private Coroutine chargeCoroutine;

    protected override void Start()
    {
        base.Start();
        chargeCoroutine = StartCoroutine(ChargeCycle());
    }

    private IEnumerator ChargeCycle()
    {
        while (true)
        {
            if (!isLive || target == null) yield return null;

            yield return new WaitForSeconds(chargeCooldown);

            if (!isLive) yield break;

            yield return StartCoroutine(PerformCharge());
        }
    }

    private IEnumerator PerformCharge()
    {
        isCharging = true;
        chargeTargetPos = target.position;

        // 돌진 준비 애니메이션
//        enemyAnimator.SetTrigger("PrepareCharge");
        SoundManager.Instance.PlaySFX("BossCharge");
        
        yield return new WaitForSeconds(chargeDelay);

        // 돌진 애니메이션
  //      enemyAnimator.SetTrigger("StartCharge");

        SoundManager.Instance.PlaySFX("RunBoss");
        float elapsed = 0f;
        Vector2 direction = (chargeTargetPos - (Vector2)transform.position).normalized;

        while (elapsed < chargeDuration)
        {
            enemyRigid.MovePosition(enemyRigid.position + direction * chargeSpeed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        enemyRigid.velocity = Vector2.zero;
    //    enemyAnimator.SetTrigger("StopCharge");

        isCharging = false;
    }

    protected override void FixedUpdate()
    {
        if (isCharging || !isLive || target == null) return;

        base.FixedUpdate(); // 평소 움직임 유지
    }

    protected override void LateUpdate()
    {
        base.LateUpdate(); // 스프라이트 방향 유지
    }

    protected override void Die()
    {
        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        base.Die(); // 원래 Enemy의 Die 처리
    }
}

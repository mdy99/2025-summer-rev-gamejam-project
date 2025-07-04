using System.Collections;
using UnityEngine;

public class BossCharger : MonoBehaviour
{
    public float chargeSpeed = 10f;
    public float chargeDelay = 1.0f;
    public float chargeCooldown = 10.0f;

    public Rigidbody2D rb;
    public Animator animator;

    public Transform player;

    private Vector2 targetPosition;
    private bool isCharging = false;

    void Start()
    {
        StartCoroutine(AutoChargeRoutine());
    }

    private IEnumerator AutoChargeRoutine()
    {
        while (true)
        {
            if (!isCharging)
            {
                yield return StartCoroutine(ChargeTowardsPlayer());
            }

            yield return new WaitForSeconds(chargeCooldown);
        }
    }

    private IEnumerator ChargeTowardsPlayer()
    {
        isCharging = true;

        // 1. 현재 플레이어 위치 저장
        targetPosition = player.position;
        SoundManager.Instance.PlaySFX("BossCharge");
        // 2. 준비 애니메이션
        //animator.SetTrigger("PrepareCharge");

        yield return new WaitForSeconds(chargeDelay);

        // 3. 돌진 애니메이션
        //animator.SetTrigger("StartCharge");

        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = dir * chargeSpeed;
        SoundManager.Instance.PlaySFX("RunBoss");
        // 4. 돌진 지속 시간
        yield return new WaitForSeconds(2f);

        rb.velocity = Vector2.zero;

        //animator.SetTrigger("StopCharge");

        isCharging = false;
    }
}

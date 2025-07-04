using System.Collections;
using UnityEngine;

public class InstantCircleAOE : MonoBehaviour
{
    private int damage; // 스킬 데미지
    public float radius = 1f; // 스킬 반경

    public float duration = 10f; // 스킬 지속 시간
    public float tickInterval = 1f; // 스킬이 적용되는 간격

    SpriteRenderer spriteRenderer; // 스프라이트 렌더러

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 가져오기
        if (spriteRenderer != null)
        {
            StartCoroutine(BlinkTransparency()); // 투명도 깜빡임 코루틴 시작
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on InstantCircleAOE object."); // 스프라이트 렌더러가 없을 경우 에러 메시지 출력
        }
    }

    IEnumerator BlinkTransparency()
    {
        float blinkSpeed = 2f;
        float minAlpha = 0.3f;
        float maxAlpha = 1f;

        while (true)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 0.5f);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t); // 알파 값 보
            Color color = spriteRenderer.color;
            color.a = alpha; // 스프라이트의 알파 값 설정
            spriteRenderer.color = color; // 스프라이트 색상 업데이트

            yield return null; // 다음 프레임까지 대기
        }
    }

    public void Initialize(int damage, float radius = 1f)
    {
        this.damage = damage; // 스킬 데미지 초기화
        this.radius = radius; // 스킬 반경 초기화

        DoImmediateDamage(); // 즉시 범위 내 적에게 데미지 적용
        StartCoroutine(ApplyDotDamage()); // 스킬 실행 코루틴 시작
    }

    void DoImmediateDamage()
    {
        SoundManager.Instance.PlaySFX("adept_attack");
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius); // 범위 내의 모든 콜라이더 가져오기
        foreach (Collider2D collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>(); // 적 컴포넌트 가져오기
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // 적에게 데미지 입히기
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    IEnumerator ApplyDotDamage()
    {
        float elapsedTime = 0f; // 경과 시간 초기화

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(tickInterval); // 지정된 간격만큼 대기

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius); // 범위 내의 모든 콜라이더 가져오기
            foreach (Collider2D collider in hitColliders)
            {
                if(collider.CompareTag("Enemy")) // 적 태그를 가진 오브젝트만 처리
                {
                    Enemy enemy = collider.GetComponent<Enemy>(); // 적 컴포넌트 가져오기
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage/5); // 적에게 데미지 입히기
                    }
                }
            }
            elapsedTime += tickInterval; // 경과 시간 업데이트
        }
        Destroy(gameObject); // 스킬 오브젝트 제거
    }
}
using System.Collections;
using UnityEngine;

public class InstantRectAOE : MonoBehaviour
{
    public float shrinkDuration = 0.5f; // 스킬이 줄어드는 시간
    private int damage;

    public float duration =2f;
    private Vector2 startSize;// 스킬의 시작 크기 (가로, 세로)
    private Vector2 endSize; // 스킬의 끝 크기 (가로, 세로)

    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러 컴포넌트

    public void Initialize(int damageValue)
    {
        startSize = transform.localScale; // 현재 스킬의 크기를 시작 크기로 설정
        endSize.x = 0.2f;
        endSize.y = transform.localScale.y; // 끝 크기를 설정 (가로, 세로)
        this.damage = damageValue; // 스킬의 피해량 설정
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 스프라이트 렌더러 컴포넌트 가져오기
        if(spriteRenderer != null)
        {
            transform.localScale = startSize; // 스킬의 시작 크기로 설정
            SoundManager.Instance.PlaySFX("Appr_attack");
            StartCoroutine(ShrinkToCenter());
        }
    }

    IEnumerator ShrinkToCenter(){
        float time = 0f; // 시간 초기화
        while(time < duration){
            float t = time / duration; // 시간 비율 계산
            Vector2 newSize = Vector2.Lerp(startSize, endSize, t); // 크기 보간
            transform.localScale = newSize; // 크기 설정

            UpdataColliderSize(newSize); // 콜라이더 크기 업데이트

            time += Time.deltaTime; // 시간 증가
            yield return null; // 다음 프레임까지 대기
        }
        transform.localScale = endSize; // 최종 크기로 설정
        UpdataColliderSize(endSize); // 콜라이더 크기 업데이트
        Destroy(gameObject); // 스킬 오브젝트 제거
    }

    void UpdataColliderSize(Vector2 scale){
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>(); // BoxCollider2D 컴포넌트 가져오기
        if(boxCollider != null)
        {
            boxCollider.size = Vector2.one; // 콜라이더의 크기를 1로 초기화
            boxCollider.offset = Vector2.zero; // 콜라이더의 오프셋
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy")) // 적과 충돌 시
        {
            SoundManager.Instance.PlaySFX("ApprHit");
            collision.GetComponent<Enemy>()?.TakeDamage(damage); // 적의 TakeDamage 메서드를 호출하여 피해를 입힘
        }
    }
void OnDrawGizmos()
{
    BoxCollider2D box = GetComponent<BoxCollider2D>();
    if (box == null) return;

    Gizmos.color = Color.red;
    Gizmos.matrix = transform.localToWorldMatrix;
    Gizmos.DrawWireCube(box.offset, box.size);
}

}
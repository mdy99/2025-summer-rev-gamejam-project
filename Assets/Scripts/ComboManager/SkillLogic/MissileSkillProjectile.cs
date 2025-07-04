using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSkillProjectile : MonoBehaviour
{
    public SkillData data; // 스킬 데이터
    public GameObject aoeEffectPrefab; // 범위 공격 이펙트 프리팹

    private Vector2 direction; // 미사일 이동 방향

    public void Initialize(Vector2 direction)
    {
        SoundManager.Instance.PlaySFX("noviceAttack");
        this.direction = direction.normalized; // 방향 벡터를 정규화
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f); // z축 위치를 0으로 설정 (2D 게임에서 z축은 사용하지 않음)
        Destroy(gameObject, 1f); // 1초 후에 미사일 오브젝트 제거
    }

    void Update()
    {
        transform.position += (Vector3)direction * data.speed * Time.deltaTime;

        // 매 프레임 z축 고정
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy")) // 적과 충돌 시
        {
            SoundManager.Instance.PlaySFX("Hit");
            Vector3 hitPoint = other.transform.position; // 충돌 지점

            GameObject aoeEffect = Instantiate(aoeEffectPrefab, hitPoint, Quaternion.Euler(-90, 0, 0)); // 범위 공격 이펙트 생성
            AOEDamage aOEDamage = aoeEffect.GetComponent<AOEDamage>();
            aOEDamage.Initialize(data.aoeRadius, data.damage); // AOE 범위와 피해량 설정
            Destroy(gameObject); // 미사일 오브젝트 제거
        }
    }
}

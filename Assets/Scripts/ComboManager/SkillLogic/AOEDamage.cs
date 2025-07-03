using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamage : MonoBehaviour
{
    private float radius;
    private int damage;

    public void Initialize(float radius, int damage)
    {
        this.radius = radius; // AOE 범위 설정
        this.damage = damage; // AOE 피해량 설정

        DoAOEDamage(); // AOE 피해를 즉시 실행
    }

    void DoAOEDamage()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, radius); // AOE 범위 내의 모든 적을 찾음
        foreach(var target in targets)
        {
            if(target.CompareTag("Enemy")) // 적 태그를 가진 오브젝트에만 피해를 입힘
            {
                target.GetComponent<Enemy>()?.TakeDamage(damage); // 적의 TakeDamage 메서드를 호출하여 피해를 입힘
            }
        }
        Destroy(gameObject, 0.5f); // AOE 효과가 끝나면 오브젝트 제거
    }

    
}

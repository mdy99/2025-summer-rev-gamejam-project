using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData; // 아이템 데이터 (아이템 타입, 드랍 확률 등)

    private PlayerMoveController playerMoveController; // 플레이어 이동 컨트롤러

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyEffect(); // 플레이어와 충돌 시 아이템 효과 적용
            Destroy(gameObject); // 아이템 오브젝트 제거
        }
    }

    void Start()
    {
        playerMoveController = FindObjectOfType<PlayerMoveController>(); // 플레이어 이동 컨트롤러 찾기

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if(sr != null && itemData != null && itemData.iconSprite != null)
        {
            sr.sprite = itemData.iconSprite; // 아이템 스프라이트 설정
        }
        else
        {
            Debug.LogError("ItemObject: SpriteRenderer or ItemData is not assigned!"); // 스프라이트 렌더러 또는 아이템 데이터가 할당되지 않은 경우 에러 메시지 출력
        }
    }

    void ApplyEffect(){
        switch(itemData.itemType)
        {
            case ItemType.HpUp:
                int randomAmount = Random.Range(10, 20); // 랜덤으로 10~20 사이의 체력 회복량 선택
                BarManager.Instance.UpdateHpBar(randomAmount); // 체력 회복
                NarrationText.Instance.UpdateNarration("체력이 " + randomAmount + "만큼 회복되었습니다!",Color.green); // 내레이션 업데이트
                break;
            case ItemType.MpUp:
                int randomMpAmount = Random.Range(10, 30); // 랜덤으로 10~30 사이의 마나 회복량 선택
                BarManager.Instance.UpdateMpBar(randomMpAmount); // 마나 회복
                NarrationText.Instance.UpdateNarration("마나가 " + randomMpAmount + "만큼 회복되었습니다!",Color.blue); // 내레이션 업데이트
                break;
            case ItemType.DamageUp:
                int randomDamage = Random.Range(1,5); // 랜덤으로 1~5 사이의 공격력 증가량 선택
                string runeCode = WaveManager.Instance.ReinforceRandomSkillDamage(randomDamage); // 랜덤 스킬 공격력 증가
                if (runeCode != null)
                {
                    NarrationText.Instance.UpdateNarration($"{runeCode}의 공격력이 " + randomDamage + "만큼 증가했습니다!",Color.magenta); // 내레이션 업데이트
                }
                break;
            case ItemType.SpeedUp:
                float randomDuration = Random.Range(3f, 7f); // 랜덤으로 3~7초 사이의 지속 시간 선택
                float randomMultiplier = Random.Range(1.2f, 2f); // 랜덤으로 1.2~2배 사이의 속도 증가 배율 선택
                playerMoveController.SpeedUpForDuration(randomDuration, randomMultiplier); // 이동 속도 증가
                NarrationText.Instance.UpdateNarration($"{randomDuration:F1}초 동안 이동 속도가 {randomMultiplier:F1}배 증가했습니다!",Color.cyan); // 내레이션 업데이트
                break;
        }
        Destroy(gameObject); // 아이템 효과 적용 후 아이템 오브젝트 제거
    }
}

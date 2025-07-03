using System.Collections;
using System.Collections.Generic;
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

    private void NarrationUpdate(string narration)
    {
        NarrationText.Instance.UpdateNarration(narration); // 내레이션 텍스트 업데이트
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
                BarManager.Instance.UpdateHpBar(itemData.increaseAmount); // 체력 회복
                NarrationUpdate("체력이 " + itemData.increaseAmount + "만큼 회복되었습니다!"); // 내레이션 업데이트
                break;
            case ItemType.MpUp:
                BarManager.Instance.UpdateMpBar(itemData.increaseAmount); // 마나 회복
                NarrationUpdate("마나가 " + itemData.increaseAmount + "만큼 회복되었습니다!"); // 내레이션 업데이트
                break;
            case ItemType.DamageUp:
                string runeCode = WaveManager.Instance.ReinforceRandomSkillDamage(itemData.increaseAmount); // 랜덤 스킬 공격력 증가
                if (runeCode != null)
                {
                    NarrationUpdate($"{runeCode}의 공격력이 " + itemData.increaseAmount + "만큼 증가했습니다!"); // 내레이션 업데이트
                }
                break;
            case ItemType.SpeedUp:
                playerMoveController.SpeedUpForDuration(itemData.duration, itemData.speedMultiplier); // 이동 속도 증가
                NarrationUpdate($"{itemData.duration}초 동안 이동 속도가 " + itemData.speedMultiplier + "배 증가했습니다!"); // 내레이션 업데이트
                break;
        }
        Destroy(gameObject); // 아이템 효과 적용 후 아이템 오브젝트 제거
    }
}

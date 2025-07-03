using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
    public enum ItemType{
        HpUp, // 체력 회복 아이템
        MpUp, // 마나 회복 아이템
        DamageUp, // 공격력 증가 아이템
        SpeedUp, // 이동 속도 증가 아이템
    }

[CreateAssetMenu(menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType itemType; // 아이템 타입 (체력 회복, 마나 회복 등)
    public Sprite iconSprite; // 아이템 아이콘 (UI에서 사용)
}

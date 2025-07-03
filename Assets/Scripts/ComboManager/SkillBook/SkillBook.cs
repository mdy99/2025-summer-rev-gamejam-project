using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook
{
    Dictionary<string, ISkill> skillMap = new Dictionary<string, ISkill>(); // 스킬 딕셔너리

    public SkillBook(
            List<SkillData> skillDatas,
            Func<Vector3> firePointProvider,
            Dictionary<string, RuneInfo> runeMap
            )
    {
        foreach(var data in skillDatas)
        {
            data.InitializeFromRunes(runeMap); // 룬의 데미지를 더해서 최종 스킬의 데미지를 정함

            if(data.skillCategory == SkillCategory.Projectile){
                skillMap[data.runeCode] = new MissileSkill(data, firePointProvider); // 투사체 스킬 생성
            }
            if(data.skillCategory == SkillCategory.InstantRectangle){
                skillMap[data.runeCode] = new InstantRectSkill(data, firePointProvider); // 직선형 즉시 스킬 생성
            }
            if(data.skillCategory == SkillCategory.InstantCircle){
                skillMap[data.runeCode] = new InstantCircleSkill(data, firePointProvider); // 원형 즉시 스킬 생성
            }
        }
    }

    public bool TryGetSkill(string runeKey, out ISkill skill)
    {
        // runeKey에 해당하는 스킬을 딕셔너리에서 찾습니다.
        return skillMap.TryGetValue(runeKey, out skill);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBook
{
    Dictionary<string, ISkill> skillMap = new Dictionary<string, ISkill>(); // 스킬 딕셔너리

    public SkillBook(List<SkillData> skillDatas, Func<Vector3> firePointProvider)
    {
        foreach(var data in skillDatas)
        {
            if(data.skillCategory == SkillCategory.Projectile){
                skillMap[data.runeCode] = new MissileSkill(data, firePointProvider); // 투사체 스킬 생성
            }
        }
    }

    public bool TryGetSkill(string runeKey, out ISkill skill)
    {
        // runeKey에 해당하는 스킬을 딕셔너리에서 찾습니다.
        return skillMap.TryGetValue(runeKey, out skill);
    }
}

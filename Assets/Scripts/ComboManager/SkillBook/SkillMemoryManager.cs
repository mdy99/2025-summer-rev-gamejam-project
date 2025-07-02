using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMemoryManager: MonoBehaviour
{

    private Dictionary<SkillType, SkillData> memorizedSkills = new();

    public void MemorizeSkill(SkillData skill){
        if(skill == null || skill.skillType == SkillType.None){
            Debug.LogWarning("Skill is null or has None type");
            return;
        }

        memorizedSkills[skill.skillType] = skill;
        Debug.Log($"Skill memorized: {skill.SkillName} ({skill.skillType})");
    }


    // 특정 타입의 스킬을 가져오기
    public SkillData GetMemorizedSkill(SkillType type){
        memorizedSkills.TryGetValue(type, out SkillData skill);
        return skill;
    }

    // public Dictionary<SkillType, SkillBook> GetAllMemorizedSkills(){
    //     return new Dictionary<SkillType, SkillBook>(memorizedSkills);
    // }

    // 초기화(필요 시)
    public void ClearAll(){
        memorizedSkills.Clear();
    }
}

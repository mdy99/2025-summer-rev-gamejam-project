using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class RewardSlot : MonoBehaviour
{
    public TMP_Text runeText;
    public TMP_Text nameText;
    public TMP_Text typeText;
    public TMP_Text descriptionText;

    public void FillWithSkill(SkillData skillData)
    {
        nameText.text = skillData.name;
        typeText.text = skillData.skillType.ToString();
        descriptionText.text = skillData.Description;

        // Rune code 예: Rune:A + Rune:B
        runeText.text = "Rune: " + string.Join(" + ", skillData.runeCode);
    }
}

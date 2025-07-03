using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUIFiller : MonoBehaviour
{
    [Header("Bar Text")]
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text mpRegenText;

    [Header("Reward 슬롯")]
    public List<RewardSlot> rewardSlots; // Reward1,2,3 각각

    void OnEnable()
    {

    }

    void Start()
    {
        BarUpdater.OnHpChanged += UpdateBarTexts;
        BarUpdater.OnMpChanged += UpdateBarTexts;
        BarUpdater.OnMpRegenChanged += UpdateBarTexts;

        WaveManager.Instance.OnMemorizedSkillsChanged += FillRewardSlots;

        UpdateBarTexts();
        FillRewardSlots();
    }

    void OnDisable()
    {
        BarUpdater.OnHpChanged -= UpdateBarTexts;
        BarUpdater.OnMpChanged -= UpdateBarTexts;
        BarUpdater.OnMpRegenChanged -= UpdateBarTexts;

        WaveManager.Instance.OnMemorizedSkillsChanged -= FillRewardSlots;
    }

    public void UpdateBarTexts()
    {
        hpText.text = $"HP: {BarManager.Instance.MaxHp}";
        mpText.text = $"MP: {BarManager.Instance.MaxMp}";
        mpRegenText.text = $"MP Regen: {BarManager.Instance.MpRegen}"; // MpRegen이 존재한다면
    }

    void FillRewardSlots()
    {
        List<SkillData> memorized = WaveManager.Instance.MemorizedSkills;

        for (int i = 0; i < rewardSlots.Count && i < memorized.Count; i++)
        {
            rewardSlots[i].FillWithSkill(memorized[i]);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSlider : MonoBehaviour
{
    [Header("슬라이더")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("텍스트 표시")]
    [SerializeField] private TMP_Text bgmText;
    [SerializeField] private TMP_Text sfxText;

    private void Start()
    {
        // 초기값 설정
        bgmSlider.value = SoundManager.Instance != null ? SoundManager.Instance.BGMVolume : 1f;
        sfxSlider.value = SoundManager.Instance != null ? SoundManager.Instance.SFXVolume : 1f;

        UpdateTexts();

        // 이벤트 등록
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    private void OnBGMChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(value);
        UpdateTexts();
    }

    private void OnSFXChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(value);

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        bgmText.text = $"BGM: {(int)(bgmSlider.value * 100)}%";
        sfxText.text = $"SFX: {(int)(sfxSlider.value * 100)}%";
    }
}

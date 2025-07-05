using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeySettingUIInitializer : MonoBehaviour
{
    [SerializeField] private TMP_Text morseText;
    [SerializeField] private TMP_Text enterText;
    [SerializeField] private Button morseBtn;
    [SerializeField] private Button enterBtn;

    void Start()
    {
        if (KeySettingManager.Instance != null)
        {
            KeySettingManager.Instance.BindUI(morseText, enterText, morseBtn, enterBtn);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeySettingManager : MonoBehaviour
{
    public static KeySettingManager Instance { get; private set; }

    public KeyCode morseKey = KeyCode.A;
    public KeyCode enterKey = KeyCode.W;

    [Header("UI")]
    [SerializeField] private TMP_Text morseKeyText;
    [SerializeField] private TMP_Text enterKeyText;
    [SerializeField] private Button morseKeyButton;
    [SerializeField] private Button enterKeyButton;

    private enum KeySettingType { None, Morse, Enter }
    private KeySettingType waitingForKey = KeySettingType.None;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        morseKeyButton.onClick.AddListener(() => WaitForKey(KeySettingType.Morse));
        enterKeyButton.onClick.AddListener(() => WaitForKey(KeySettingType.Enter));

        UpdateUI();
    }

    void Update()
    {
        if (waitingForKey == KeySettingType.None) return;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                switch (waitingForKey)
                {
                    case KeySettingType.Morse:
                        morseKey = keyCode;
                        break;
                    case KeySettingType.Enter:
                        enterKey = keyCode;
                        break;
                }

                Debug.Log($"입력된 키: {keyCode}");
                waitingForKey = KeySettingType.None;
                UpdateUI();
                break;
            }
        }
    }

    void WaitForKey(KeySettingType type)
    {
        waitingForKey = type;
        switch (type)
        {
            case KeySettingType.Morse:
                morseKeyText.text = "모스 키: 입력 대기 중...";
                break;
            case KeySettingType.Enter:
                enterKeyText.text = "엔터 키: 입력 대기 중...";
                break;
        }
    }

    void UpdateUI()
    {
        morseKeyText.text = $"모스 키: {morseKey}";
        enterKeyText.text = $"엔터 키: {enterKey}";
    }
}

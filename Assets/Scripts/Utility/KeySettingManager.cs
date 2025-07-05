using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


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
    private bool isListeningForKey = false;




void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject); // 중복이면 삭제
    }
}


void Start()
{
    UpdateUI();
}


public void BindUI(TMP_Text morseText, TMP_Text enterText, Button morseBtn, Button enterBtn)
{
    morseKeyText = morseText;
    enterKeyText = enterText;
    morseKeyButton = morseBtn;
    enterKeyButton = enterBtn;

    morseKeyButton.onClick.AddListener(() => StartKeyListening(KeySettingType.Morse));
    enterKeyButton.onClick.AddListener(() => StartKeyListening(KeySettingType.Enter));

    UpdateUI();
}


    void Update()
    {
        if (!isListeningForKey || waitingForKey == KeySettingType.None) return;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                if (waitingForKey == KeySettingType.Morse)
                    morseKey = keyCode;
                else if (waitingForKey == KeySettingType.Enter)
                    enterKey = keyCode;

                Debug.Log($"입력된 키: {keyCode}");

                waitingForKey = KeySettingType.None;
                isListeningForKey = false;

                UpdateUI();
                break;
            }
        }
    }

    void StartKeyListening(KeySettingType type)
    {
        waitingForKey = type;
        isListeningForKey = false;
        StartCoroutine(EnableKeyListeningNextFrame());

        // 즉시 텍스트 갱신
        if (type == KeySettingType.Morse)
            morseKeyText.text = "모스 키: ...입력 대기 중...";
        else if (type == KeySettingType.Enter)
            enterKeyText.text = "엔터 키: ...입력 대기 중...";
    }

    IEnumerator EnableKeyListeningNextFrame()
    {
        yield return new WaitForEndOfFrame(); // 버튼 클릭한 프레임은 지나가게 한다
        isListeningForKey = true;
    }

    void UpdateUI()
    {
        morseKeyText.text = $"모스 키: {morseKey}";
        enterKeyText.text = $"엔터 키: {enterKey}";
    }
}

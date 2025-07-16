using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("스타트 씬 BGM")]
    public AudioClip startSceneBGM;
    [Header("메인 게임 씬 BGM")]
    public AudioClip mainGameBGM;

    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    public float BGMVolume
    {
        get => bgmSource.volume;
        set => bgmSource.volume = value;
    }

    public float SFXVolume
    {
        get => sfxSource.volume;
        set => sfxSource.volume = value;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllSFXClips();
                  SceneManager.sceneLoaded += OnSceneLoaded; // ✅ 씬 로딩 이벤트 연결
            InitializeAudioSources();
            PlayInitialBGM();
        }
        else
        {
            Destroy(gameObject);
        }
    }

private void PlayInitialBGM()
    {
        PlaySceneBGM(SceneManager.GetActiveScene().name);
    }

    private void PlaySceneBGM(string sceneName)
    {
        if (sceneName == "StartScene")
        {
            PlayBGM(startSceneBGM);
        }
        else if (sceneName == "MainGameScene")
        {
            PlayBGM(mainGameBGM);
        }
        else
        {
            Debug.Log($"[SoundManager] No BGM assigned for scene: {sceneName}");
        }
    }

   private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 이벤트 해제
    }
       private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneBGM(scene.name);
    }

    private void InitializeAudioSources()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = 0.5f; // ✅ BGM 기본 볼륨 설정

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = 0.5f; // ✅ SFX 기본 볼륨 설정
    }

    private void LoadAllSFXClips()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/Effect");
        foreach (var clip in clips)
        {
            if (!sfxClips.ContainsKey(clip.name))
                sfxClips[clip.name] = clip;
                //ebug.Log($"[SoundManager] Loaded SFX: {clip.name}");
        }
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp01(volume);
    }
    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
    }

    public void PlaySFX(string name)
    {
        if (sfxClips.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] SFX '{name}' not found in Resources/Audio/Effect");
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;

        bgmSource.Stop(); // 현재 BGM 정지
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}

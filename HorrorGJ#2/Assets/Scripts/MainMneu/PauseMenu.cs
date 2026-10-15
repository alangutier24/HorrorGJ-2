using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance {get; private set;}
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderSfx;
    [SerializeField] GameObject pauseMenu;

    private bool pause = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        sliderMusic.onValueChanged.RemoveListener(OnMusicChanged);
        sliderSfx.onValueChanged.RemoveListener(OnSfxChanged);
    }

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        if(Instance != null && Instance != this) {Destroy(gameObject); return;}
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    //void OnMusicChanged(float value) => AudioManager.Instance.SetMusicVolume(value);
    void OnMusicChanged(float value)
     {
       Debug.Log($"Music slider: {value}");
       AudioManager.Instance.SetMusicVolume(value);
    }
    void OnSfxChanged(float value)   => AudioManager.Instance.SetSFXVolume(value);
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sliderMusic = GameObject.Find("MusicSlider").GetComponent<Slider>();
        sliderSfx = GameObject.Find("SfxSlider").GetComponent<Slider>();

        if(sliderMusic == null || sliderSfx == null)
        {
            Debug.LogError("PauseMenu: MusicSlider or SfxSlider not found in the loaded scene.", this);
            return;
        }

        pauseMenu = GameObject.Find("PauseGUI");

        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager is missing from the loaded scene.", this);
            return;
        }

        sliderMusic.value = AudioManager.Instance.GetMusicVolume();
        sliderSfx.value = AudioManager.Instance.GetSFXVolume();

        sliderMusic.onValueChanged.AddListener(OnMusicChanged);
        sliderSfx.onValueChanged.AddListener(OnSfxChanged);

        pauseMenu.SetActive(false);
    }

    public bool PauseMenuUpdate()
    {
        if(Keyboard.current.pKey.wasPressedThisFrame)
            pause = !pause;

        if(pause)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        return pause;
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager is missing; music volume was not changed.", this);
            return;
        }

        AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager is missing; SFX volume was not changed.", this);
            return;
        }

        AudioManager.Instance.SetSFXVolume(value);
    }
}

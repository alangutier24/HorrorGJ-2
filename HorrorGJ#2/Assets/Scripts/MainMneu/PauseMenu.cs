using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance {get; private set;}
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderSfx;
    [SerializeField] string musicParam = "MusicVol";
    [SerializeField] string sfxParam = "SfxVolume";
    [SerializeField] GameObject pauseMenu;

    private bool pause = false;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        if(Instance != null && Instance != this) {Destroy(gameObject); return;}
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
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
        mixer.SetFloat(musicParam, Mathf.Log10(sliderMusic.value)*20);
    }

    public void SetSfxVolume(float value)
    {
        Debug.Log("Set SFX");
        mixer.SetFloat(sfxParam, Mathf.Log10(sliderSfx.value)*20);
    }
}

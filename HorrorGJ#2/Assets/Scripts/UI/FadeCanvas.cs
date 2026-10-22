using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FadeCanvas : MonoBehaviour
{

    public static FadeCanvas Instance {get; private set;}

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    Animator animator;
    TMP_Text narrativeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance != null && Instance != this) {Destroy(gameObject); return;}
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        animator = GetComponent<Animator>();
        if(animator == null)
            return;

        narrativeText = GetComponentInChildren<TMP_Text>();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        narrativeText = GetComponentInChildren<TMP_Text>();
    }

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }

    public void ResetFadeIn()
    {
        animator.ResetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void ResetFadeOut()
    {
        animator.ResetTrigger("FadeOut");
    }

    public void SetStatesActive(bool active)
    {
        animator.SetBool("SetActive", active);
    }

    public void SetNarrativeText(string text)
    {
        narrativeText.text = text;
    }

    public void RemoveNarrative()
    {
        narrativeText.text = "";
    }


}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance {get; private set;}

    [SerializeField]
    private Dialogue[] story;

    private enum StoryState
    {
        WakeUp,
    }

    private StoryState storyState;

    private FadeCanvas fcanvas;

    private DialoguePanelUI dialoguePanelUI;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dialoguePanelUI = GameObject.FindFirstObjectByType<DialoguePanelUI>();

        if (dialoguePanelUI == null)
        {
            Debug.LogError($"No se encontró DialoguePanelUI en la escena {scene.name}.");
            return;
        }

        fcanvas = GameObject.FindFirstObjectByType<FadeCanvas>();
        if (fcanvas == null)
        {
            Debug.LogError($"No se encontró un FadeCanvas en la escena {scene.name}.");
            return;
        }

        Debug.Log("Everything was found, thank you very much!");
        fcanvas.SetActive(true);
    }

    public void BeginStoryGameState()
    {
        storyState = StoryState.WakeUp;
    }

    private bool storyStarted = false;

    public void StoryUpdate()
    {   
        if (storyState != StoryState.WakeUp || storyStarted)
            return;

        // No iniciar la historia mientras siga activa otra escena
        // o no se haya encontrado el FadeCanvas.
        if (SceneManager.GetActiveScene().name != "Game" ||
            fcanvas == null ||
            dialoguePanelUI == null)
            return;

        Debug.Log("Starting story...");
        DialogueManager.Instance.SetActiveDialoguePanel(true);

        storyStarted = true;
        StartCoroutine(StartStory(story[(int)storyState]));
    }

    private IEnumerator StartStory(Dialogue dialogue)
    {
        yield return StartCoroutine(dialoguePanelUI.ShowDialogue(dialogue));
        fcanvas.FadeIn();
        GameManager.Instance.Play();
    }

    public bool PlayNextStory(int index)
    {

        return true;
    }

}

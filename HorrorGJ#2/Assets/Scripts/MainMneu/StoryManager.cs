using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;



public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance {get; private set;}

    [SerializeField]
    private Dialogue[] story;

    public enum StoryState
    {
        WakingUp,
        WTF,
        WhatHappened,
        FindingRichardsLaptop, 
        TheLight, 
        TheMonster, 
        Radiation, 
        RichardsResearch,
        MyBedroom
    }

    private bool wakingUpCompleted = false;
    private bool wtfCompleted = false;
    private bool whatHappenedCompleted = false;
    private bool findingRichardsLaptopCompleted = false;
    private bool theLightCompleted = false;
    private bool theMonsterCompleted = false;
    private bool radiationCompleted = false;
    private bool richardsResearchCompleted = false;
    private bool myBedroomCompleted = false;

    public StoryState storyState;

    public FadeCanvas fcanvas;

    public DialoguePanelUI dialoguePanelUI;

    public bool storyFinished = false;

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
            return;
        }

        fcanvas = GameObject.FindFirstObjectByType<FadeCanvas>();
        if (fcanvas == null)
        {
            return;
        }

        fcanvas.SetActive(true);
    }

    private bool storyStarted = false;

    public void StoryUpdate()
    {   

        // No iniciar la historia mientras siga activa otra escena
        // o no se haya encontrado el FadeCanvas.
        if (SceneManager.GetActiveScene().name != "Game" ||
            fcanvas == null ||
            dialoguePanelUI == null)
            return;

        switch (storyState)
        {
            case StoryState.WakingUp:
            if(!storyStarted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    storyStarted = true;
                }
                break;
            case StoryState.WTF:
                if(!wtfCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    wtfCompleted = true;
                }
                break;

            case StoryState.WhatHappened:
                if(!whatHappenedCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    whatHappenedCompleted = true; 
                }
                break;
            case StoryState.FindingRichardsLaptop:
                if(!findingRichardsLaptopCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    findingRichardsLaptopCompleted = true;
                }
                break;
            case StoryState.TheLight:
                if(!theLightCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    theLightCompleted = true;
                }
                break;
            case StoryState.TheMonster:
                if(!theMonsterCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    theMonsterCompleted = true;
                }
                break;
            case StoryState.Radiation:
                if(!radiationCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    radiationCompleted = true;
                }
                break;  
            case StoryState.RichardsResearch:
                if(!richardsResearchCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    richardsResearchCompleted = true;
                }
                break;
            case StoryState.MyBedroom:
                if(!myBedroomCompleted)
                {
                    DialogueManager.Instance.SetActiveDialoguePanel(true);
                    StartCoroutine(StartStory(story[(int)storyState]));
                    myBedroomCompleted = true;
                }
                break;
            default:
                Debug.LogWarning("Estado de historia no reconocido.");
                break;
        }

    }

    private IEnumerator StartStory(Dialogue dialogue)
    {
        yield return StartCoroutine(dialoguePanelUI.ShowDialogue(dialogue));
        fcanvas.FadeIn();
        storyFinished = true;
    }

}

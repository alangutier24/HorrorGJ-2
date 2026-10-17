using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance {get; private set;}

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


    [SerializeField] public CharacterMovement player;
    [SerializeField] public SoundDefinition LabMusic; 
    [SerializeField] public SoundDefinition MainMenuMusic;
    [SerializeField] public SoundDefinition startingDialogueSfx;  
    [SerializeField] public WakingUpScene wakingUpScene;
    [SerializeField] public BandagesScene bandagesScene;
    [SerializeField] public WhatHappenedScene whatHappenedScene;
    [SerializeField] public I_Laptop laptop;
    [SerializeField] public HouseScene houseScene;
    CircleCollider2D laptopCollider;


    public enum GameState
    {
        MainMenu,
        StartGame,
        Pause, 
        Play, 
        Episodes,
        End,
        Win, 
        Loose, 
        Game
    }

    public enum Episode
    {
        Episode1,
        Episode2,
        Episode3,
        Episode4,
        Episode5,
        Episode6,
        Episode7,
        Episode8,
        Episode9,
        Episode10
    }

    public GameState gameState;
    public Episode currentEpisode;
    private bool storyStarted = false;
    private bool mainMenuLoaded = false;
    private GameObject ECanvas;
    private Animator eanim;

    private Objective curObjective;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
        if(Instance != null && Instance != this) {Destroy(gameObject); return;}
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //gameState = GameState.MainMenu;
        gameState = GameState.MainMenu;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        GameObject playerGO = GameObject.FindWithTag("Player");
        if(playerGO == null)
            return;
        player = playerGO.GetComponent<CharacterMovement>();
        
        wakingUpScene = GameObject.FindFirstObjectByType<WakingUpScene>();

        bandagesScene = GameObject.FindFirstObjectByType<BandagesScene>();

        whatHappenedScene = GameObject.FindFirstObjectByType<WhatHappenedScene>();

        laptop = GameObject.FindFirstObjectByType<I_Laptop>();
        laptopCollider =  laptop.GetComponent<CircleCollider2D>();
        laptopCollider.enabled = false;
        laptop.enabled = false;

        houseScene = GameObject.FindFirstObjectByType<HouseScene>();
    }

    private bool play = true;

    public bool episode1Started = false;

    public bool episode2Started = false;

    public bool episode3Started = false;

    public bool episode4Started = false;

    void Update()
    {
        switch(gameState)
        {
            case GameState.Play:
            {
                if(play)
                {
                    AudioManager.Instance.PlayMusic(LabMusic, 1.5f);
                    play = false;
                }
                player.PlayerUpdate();
                if(PauseMenu.Instance.PauseMenuUpdate())
                {
                    gameState = GameState.Pause;
                }
            }
            break;
             case GameState.Episodes:
                {
                    // Game scene may not have finished loading yet
                    if(wakingUpScene == null)
                        break;

                    switch(currentEpisode)
                    {
                        case Episode.Episode1:
                            {
                                if(!episode1Started)
                                {
                                    episode1Started = true;
                                    AudioManager.Instance.PlaySFX(wakingUpScene.alarmSfx);
                                    StoryManager.Instance.storyState = StoryManager.StoryState.WakingUp;
                                    curObjective = wakingUpScene.objective;
                                    bandagesScene.enabled = true;
                                }
                            }
                            break;
                        case Episode.Episode2:
                            {
                                if(!episode2Started)
                                {
                                    episode2Started = true;
                                    StoryManager.Instance.storyState = StoryManager.StoryState.WTF;
                                    curObjective = bandagesScene.objective;
                                    whatHappenedScene.enabled = true;
                                    CircleCollider2D cc = laptop.GetComponent<CircleCollider2D>();
                                    laptopCollider.enabled = true;
                                    laptop.enabled = true;
                                }
                            }
                            break;
                        case Episode.Episode3:
                            {
                                if(!episode3Started)
                                {
                                    episode3Started = true;
                                    StoryManager.Instance.storyState = StoryManager.StoryState.WhatHappened;
                                    curObjective = whatHappenedScene.objective;
                                }
                            }
                            break;
                        case Episode.Episode4:
                            {
                                if(!episode4Started)
                                {
                                    episode3Started = true;
                                    StoryManager.Instance.storyState = StoryManager.StoryState.House;
                                    curObjective = houseScene.objective;
                                }
                            }
                            break;
                    }

                    StoryManager.Instance.StoryUpdate();
                    if(StoryManager.Instance.storyFinished)
                    {
                        ObjectiveManager.Instance.ShowObjectiveUI(curObjective);
                        StoryManager.Instance.storyFinished = false;
                    }
                    if(ObjectiveManager.Instance.doneShowingObjective)
                    {
                        gameState = GameState.Play;
                        ObjectiveManager.Instance.doneShowingObjective = false;
                    }
                }
            break;  
            case GameState.MainMenu:
            {
                if(!mainMenuLoaded)
                {
                    AudioManager.Instance.PlayMusic(MainMenuMusic, 1.5f);
                    mainMenuLoaded = true;
                }
            }
            break;

            case GameState.Pause:
            {
                if(!PauseMenu.Instance.PauseMenuUpdate())
                {
                    gameState = GameState.Play;
                }

            }
            break;

            case GameState.Loose:
            {
                //MainMenu();
            }
            break;

            case GameState.Win:
            {
                //MainMenu();
            }
            break;

        }
        
    }

}

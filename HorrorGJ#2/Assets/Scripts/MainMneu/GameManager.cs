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

    [SerializeField] public I_Notes notes;

    CircleCollider2D notesCollider;

    [SerializeField] public DoorGuardianScene doorGuardianScene;


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
        Episode1, //Wake up
        Episode2, // WTF
        Episode3, // What happened
        Episode4, // House
        Episode5, // Richard's Laptop
        Episode6, // The Light
        Episode7, // The Monster
        Episode8, // Radiation
        Episode9, // Richard's Research
        Episode10, // My Bedroom
        Episode11, // Door Guardian
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
        
        wakingUpScene = GameObject.FindAnyObjectByType<WakingUpScene>();

        bandagesScene = GameObject.FindAnyObjectByType<BandagesScene>();

        whatHappenedScene = GameObject.FindAnyObjectByType<WhatHappenedScene>();

        laptop = GameObject.FindAnyObjectByType<I_Laptop>();
        laptopCollider =  laptop.GetComponent<CircleCollider2D>();
        laptopCollider.enabled = false;
        laptop.enabled = false;

        houseScene = GameObject.FindAnyObjectByType<HouseScene>();

        notes = GameObject.FindAnyObjectByType<I_Notes>();
        notesCollider = notes.GetComponent<CircleCollider2D>();
        notesCollider.enabled = false;
        notes.enabled = false;

        doorGuardianScene = GameObject.FindAnyObjectByType<DoorGuardianScene>();
    }

    private bool play = true;

    public bool episode1Started = false;

    public bool episode2Started = false;

    public bool episode3Started = false;

    public bool episode4Started = false;

    public bool episode11Started = false;

    void Update()
    {
        switch(gameState)
        {
            case GameState.Play:
            {
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

                    player.StopMovement();

                    switch(currentEpisode)
                    {
                        case Episode.Episode1:
                            {
                                if(!episode1Started)
                                {
                                    AudioManager.Instance.PlayMusic(LabMusic, 0.75f);
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
                                    notes.enabled = true;
                                    notesCollider.enabled = true;
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
                        case Episode.Episode11:
                            {
                                if(!episode11Started)
                                {
                                    episode11Started = true;
                                    StoryManager.Instance.storyState = StoryManager.StoryState.DoorGuardian;
                                    curObjective = doorGuardianScene.objective;
                                    doorGuardianScene.enabled = true;
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

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

    public enum GameState
    {
        MainMenu,
        StartGame,
        Pause, 
        Play, 
        End,
        Win, 
        Loose, 
        Game
    }

    public GameState gameState;

    private bool storyStarted = false;
    private bool mainMenuLoaded = false;
    private GameObject ECanvas;
    private Animator eanim;

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
        gameState = GameState.Play;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject playerGO = GameObject.FindWithTag("Player");
        if(playerGO == null)
            return;
        player = playerGO.GetComponent<CharacterMovement>();
        if(player == null)
            return;
    }

    private bool play = true;

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

            case GameState.StartGame:
            {
                if(!storyStarted)
                {
                    storyStarted = true;
                    //AudioManager.Instance.PlayMusic(HouseMusic, 1.5f);
                    StoryManager.Instance.BeginStoryGameState();
                }
                StoryManager.Instance.StoryUpdate();
            }
            break;
        }

    }

    public void StartGame()
    {
        gameState = GameState.StartGame;
    }

    public void Play()
    {
        gameState = GameState.Play;
    }
}

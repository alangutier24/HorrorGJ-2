using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {get; private set;}

    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;


    [SerializeField] public CharacterMovement player;
    [SerializeField] public SoundDefinition HouseMusic; 
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
        gameState = GameState.MainMenu;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player").GetComponent<CharacterMovement>();
        if(player == null)
        {
            Debug.LogError($"No se encontró un objeto con el tag 'Player' en la escena {scene.name}.");
            return;
        }
    }

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
                    AudioManager.Instance.PlayMusic(HouseMusic, 1.5f);
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

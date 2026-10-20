using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance {get; private set;}
    [SerializeField]
    private Objective[] objectives;

    [SerializeField] public GameObject objectiveUI;
    [SerializeField] public TMP_Text objectiveText;
    [SerializeField] public TMP_Text objectiveDescription;
    [SerializeField] public Animator anim;

    public bool doneShowingObjective = false;


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        objectiveUI = GameObject.Find("ObjectiveCanvas");
        if(objectiveUI == null)
            return;

        anim = GameObject.Find("ObjectivePanel").GetComponent<Animator>();
        objectiveText = GameObject.Find("Objective").GetComponent<TMP_Text>();
        GameObject objectiveDescriptionGO = GameObject.Find("ObjectiveDescription");
        objectiveDescription = objectiveDescriptionGO.GetComponent<TMP_Text>();
        objectiveDescriptionGO.SetActive(false);
        objectiveUI.SetActive(false);
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

    public void ShowObjectiveUI(Objective objective)
    {
        objectiveUI.SetActive(true);
        anim.SetBool("BigObjective", true);
        anim.SetBool("SmallObjective", false);
        objectiveText.text = objective.objectiveName;
        objectiveDescription.text = objective.objectiveDescription;
        StartCoroutine(CloseObjective(5.0f));
    }

    private IEnumerator CloseObjective(float wait)
    {
        float timeElapsed = 0;
        while(wait > timeElapsed)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        HideObjectiveUI();
    }

    public void HideObjectiveUI()
    {
        anim.SetBool("BigObjective", false);
        anim.SetBool("SmallObjective", true);
        doneShowingObjective = true;
    }

}

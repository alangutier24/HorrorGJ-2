using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{ 

    public DialoguePanelUI dialoguePanelUI;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField]
    public GameObject DialoguePanel;

    public void SetActiveDialoguePanel(bool active)
    {
        DialoguePanel.SetActive(active);
    }





}

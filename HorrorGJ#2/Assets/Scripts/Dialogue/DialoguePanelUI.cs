using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using System.Diagnostics;
using System.Collections.Generic;

public class DialoguePanelUI : MonoBehaviour
{

    [SerializeField] 
    private TMP_Text textLabel;
    [SerializeField]
    private GameObject dialogueBox;

    public bool IsOpen{get; private set;}

    private ResponseHandler responseHandler;
    private TypeWritterEffect typeWritterEffect;

    void Awake()
    {
        dialogueBox = GameObject.Find("DialogueCanvas");
        textLabel = GameObject.Find("Dialogue").GetComponent<TMP_Text>();
        typeWritterEffect = FindFirstObjectByType<TypeWritterEffect>();
        responseHandler = FindFirstObjectByType<ResponseHandler>();
        dialogueBox.SetActive(false);
    }

    void Start()
    {
        IsOpen = true;
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }
    
    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        for(int i=0; i<dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            if(i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        }

        if(dialogueObject.HasResponses)
        {
            responseHandler.ShowRespones(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }

    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typeWritterEffect.Run(dialogue, textLabel);

        while(typeWritterEffect.isRunning)
        {
            yield return null;

            if(Keyboard.current.spaceKey.wasPressedThisFrame)
                typeWritterEffect.Stop();
        }
    }

    private void CloseDialogueBox()
    {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }

}

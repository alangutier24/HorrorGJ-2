using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

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
            yield return typeWritterEffect.Run(dialogue, textLabel);

            if(i== dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        }

        if(dialogueObject.HasResponses)
        {
            Debug.Log("Has responses");
            responseHandler.ShowRespones(dialogueObject.Responses);
        }
        else
        {
            Debug.Log("Does not have responses");
            CloseDialogueBox();
        }

    }

    private void CloseDialogueBox()
    {
        Debug.Log("Close box");
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }

}

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialoguePanelUI : MonoBehaviour
{

    [SerializeField] 
    private TMP_Text textLabel;
    [SerializeField]
    private DialogueObject testDialogue;
    [SerializeField]
    private GameObject dialogueBox;

    private ResponseHandler responseHandler;
    private TypeWritterEffect typeWritterEffect;

    void Start()
    {
        typeWritterEffect = GetComponent<TypeWritterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        ShowDialogue(testDialogue);
    
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
            responseHandler.ShowRespones(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
        }

    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }

}

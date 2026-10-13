using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] 
    private RectTransform responseBox;

    [SerializeField] 
    private RectTransform responseButtonTemplate;

    [SerializeField] 
    private RectTransform responseContainer;

    private DialoguePanelUI dialoguePanelUI;

    private List<GameObject> tempResponseButtons = new List<GameObject>();

    void Start()
    {
        dialoguePanelUI = GetComponent<DialoguePanelUI>();
    }

    public void ShowRespones(Response[] responses)
    {
        float responseBoxHeight = 0f;

        float maxWidth = 0;

        foreach(Response response in responses)
        {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));
            float width = responseButton.GetComponent<TMP_Text>().GetPreferredValues(responseButton.GetComponent<TMP_Text>().text).x;
            if(width > maxWidth)
                maxWidth = width;

            tempResponseButtons.Add(responseButton);

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        
        responseBox.sizeDelta = new Vector2(maxWidth + 20f, responseBoxHeight);
        //responseBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response)
    {
        responseBox.gameObject.SetActive(false);

        foreach(GameObject button in tempResponseButtons)
        {
            Destroy(button);
        }

        tempResponseButtons.Clear();

        dialoguePanelUI.ShowDialogue(response.DialogueObject);
    }


}

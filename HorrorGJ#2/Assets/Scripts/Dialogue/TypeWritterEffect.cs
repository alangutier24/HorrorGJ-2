using UnityEngine;
using TMPro;
using System.Collections;

public class TypeWritterEffect : MonoBehaviour
{

    [SerializeField]
    private float typeWritterSpeed = 50f;

    public Coroutine Run(string textToType, TMP_Text textLabel)
    {
        return StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {

        textLabel.text = string.Empty;
    
        float elapsed = 0;
        int charIndex = 0;

        while(charIndex < textToType.Length)
        {
            elapsed += Time.deltaTime * typeWritterSpeed;
            charIndex = Mathf.FloorToInt(elapsed);

            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);
            textLabel.text = textToType.Substring(0, charIndex);

            yield return null;
        }

        textLabel.text = textToType;

    }

}

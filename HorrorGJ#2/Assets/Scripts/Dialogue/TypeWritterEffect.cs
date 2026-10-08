using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class TypeWritterEffect : MonoBehaviour
{

    [SerializeField]
    private float typeWritterSpeed = 50f;

    Dictionary<char, float> punctuations = new Dictionary<char, float>()
    {
        {'.', 0.3f}, {',', 0.3f}, 
        {'?', 0.6f}, {'!', 0.6f}
    };

    public Coroutine Run(string textToType, TMP_Text textLabel)
    {
        return StartCoroutine(TypeText(textToType, textLabel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {

        textLabel.text = string.Empty;
    
        float elapsed = 0;
        int charIndex = 0;

        while(charIndex < textToType.Length - 2)
        {
            elapsed += Time.deltaTime * typeWritterSpeed;
            charIndex = Mathf.FloorToInt(elapsed);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            float waitTime = 0;

            bool isLast = charIndex >= textToType.Length - 1;
            
            textLabel.text = textToType.Substring(0, charIndex + 1);

            if(punctuations.ContainsKey(textToType[charIndex]) && !isLast && !punctuations.ContainsKey(textToType[charIndex + 1]))
            {
                UnityEngine.Debug.Log(elapsed);
                waitTime = punctuations[textToType[charIndex]];
                elapsed += waitTime;
                yield return new WaitForSeconds(waitTime);
            }


            yield return null;
        }

        textLabel.text = textToType;

    }

}

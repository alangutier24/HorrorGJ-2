using UnityEngine;

public class FadeCanvas : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FadeIn()
    {
        animator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        animator.SetTrigger("FadeOut");
    }

    public void SetActive(bool active)
    {
        //animator.SetBool("SetActive", active);
    }

}

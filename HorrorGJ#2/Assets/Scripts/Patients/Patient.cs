using UnityEngine;

public class Patient : MonoBehaviour
{

    private enum PatientState
    {
        healty, 
        detiriorating, 
        troublesum
    }

    [SerializeField] private float health;
    [SerializeField] private float sanity;
    [SerializeField] private PatientData data;

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;



    void TakeMeds(Medication meds)
    {
        if (data.prescription.medication.TryGetValue(meds.med, out var value))
        {
            //Patient sanity goes up
            Debug.Log("Patient sanity went up");
        }
        else
        {
            Debug.Log("Patient sanity went down");
            //patient sanity goes down
        }
    }
}

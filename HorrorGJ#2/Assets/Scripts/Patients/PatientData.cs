using UnityEngine;

[CreateAssetMenu(menuName = "Patient/PatientData")]

public class PatientData : ScriptableObject
{
    public string Name;

    public float height;

    public float weight;

    public string diagnosis;

    public Prescription prescription;
}

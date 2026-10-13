using System.Collections.Generic;

[System.Serializable]
public class Prescription
{
   public string diagnosis;
   public Dictionary<string, float> medication;
}

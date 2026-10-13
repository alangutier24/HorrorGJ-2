using UnityEngine;

   public class AnimEventReceiver : MonoBehaviour
   {
       public event System.Action OnSwitchCamera;   // others subscribe to this

       public void SwitchCamera()                   // called by the animation
       {
           OnSwitchCamera?.Invoke();
       }
   }

using UnityEngine;

public class TargetObject : MonoBehaviour
{
    private void Start()
    {
        UIController ui = FindObjectOfType<UIController>();
        
        if (ui == null) Debug.LogError("No UIController component found");

        ui.AddTargetIndicator(this.gameObject);
    }
}

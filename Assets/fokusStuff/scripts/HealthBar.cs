using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform healthValue;
    
    public void SetHealthValue(float value)
    {
        healthValue.localScale = new Vector3(value, 1f, 1f);
    }
}

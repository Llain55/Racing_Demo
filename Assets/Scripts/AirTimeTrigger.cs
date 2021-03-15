using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirTimeTrigger : MonoBehaviour
{
    public CarController car;
    private void OnTriggerEnter(Collider other) 
    {
        
        if(other.tag == "Car Rear")
        {
            Debug.Log("TEST");
            car.airbourne = true;
        }
    }
}

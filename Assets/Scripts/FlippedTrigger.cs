using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlippedTrigger : MonoBehaviour
{
    public CarController carController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            carController.flipped = true;
            carController.flickOff = true;
        }
    }
}

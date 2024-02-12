using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpType : MonoBehaviour
{
    // type of the object
    public enum PickUp {
        heal, coin
    }
    // variables
    public PickUp type;
    public int value = 20;
   
    // OnTriggerEnter is called when the Collider other enters the trigger.
    protected void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<Character>().PickUpItem(this);
            Destroy(this.gameObject);
        }
    }
   
}

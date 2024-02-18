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
    public ParticleSystem collectedVFX;
    
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    protected void Start() {
        if (type != PickUp.coin) {
            Invoke("DestroyGameObject", 10f);
        }
    }
    
    private void DestroyGameObject() {
        Destroy(this.gameObject);
    }
   
    // OnTriggerEnter is called when the Collider other enters the trigger.
    protected void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            if (collectedVFX != null) {
                Instantiate(collectedVFX, transform.position, Quaternion.identity);
            }
            // play SFX
            SFXManager.instance.PlayAudio(2);
            other.gameObject.GetComponent<Character>().PickUpItem(this);
            Destroy(this.gameObject);
        }
    }
   
}

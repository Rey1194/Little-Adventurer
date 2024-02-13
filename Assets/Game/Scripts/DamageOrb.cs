using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOrb : MonoBehaviour
{
    public float speed = 2f;
    public int Damage = 10;
    public ParticleSystem hitVFX;
    private Rigidbody _rb;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _rb = GetComponent<Rigidbody>();
    }
    
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    protected void FixedUpdate() {
        _rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }
    
    // OnTriggerEnter is called when the Collider other enters the trigger.
    protected void OnTriggerEnter(Collider other) {
        Character cc = other.gameObject.GetComponent<Character>();
        
        if (cc != null && cc.isPlayer) {
            cc.ApplyDamage(Damage, transform.position);
        }
        
        Instantiate(hitVFX, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}

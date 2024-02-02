using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    public int damage = 30;
    public string targetTag;
    private Collider _damageCasterCollider;
    private List<Collider> _damageTargetList;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _damageCasterCollider = GetComponent<Collider>();
        _damageTargetList = new List<Collider>();
        _damageCasterCollider.enabled = true;
    }
    
    // OnTriggerEnter is called when the Collider other enters the trigger.
    protected void OnTriggerEnter(Collider other) {
        if (other.tag == targetTag && !_damageTargetList.Contains(other)) {
            Character targetCC = other.GetComponent<Character>();
            if (targetCC != null) {
                targetCC.ApplyDamage(damage);
            }
            _damageTargetList.Add(other);
        }
    }
    
    public void EnableDamageCaster() {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }
    
    public void DisableDamageCaster() {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }
}

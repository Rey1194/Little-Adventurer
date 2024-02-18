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
        _damageCasterCollider.enabled = false;
    }
    
    // OnTriggerEnter is called when the Collider other enters the trigger.
    protected void OnTriggerEnter(Collider other) {
        if (other.tag == targetTag && !_damageTargetList.Contains(other)) {
            Character targetCC = other.GetComponent<Character>();
            if (targetCC != null) {
                targetCC.ApplyDamage(damage, transform.parent.position);
                // llamar al sfx
                SFXManager.instance.PlayAudio(4);
                // instanciar el VFX en el punto de colisión contra el enemigo
                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();
                if (playerVFXManager != null) {
                    RaycastHit hit;
                    Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;
                    bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);                    
                    if (isHit) {
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }
                }
            }
            _damageTargetList.Add(other);
        }
    }
    // función llamada en la animación de ataque
    public void EnableDamageCaster() {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }
    // función llamada en la animación de ataque
    public void DisableDamageCaster() {
        _damageTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }
    
    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
    private void OnDrawGizmos() {
        if (_damageCasterCollider == null) {
            _damageCasterCollider = GetComponent<Collider>();
        }
        
        RaycastHit hit;
        Vector3 originalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;
        bool isHit = Physics.BoxCast(originalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1 << 6);
        
        if (isHit) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
    }
}

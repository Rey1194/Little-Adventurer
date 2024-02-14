using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootDamageOrb : MonoBehaviour
{
    public Transform  shootingPoint;
    public GameObject damageOrb;
    public Character _cc;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _cc = GetComponent<Character>();
    }
    
    // Update is called every frame, if the MonoBehaviour is enabled.
    protected void Update() {
        _cc.RotateToTarget();
    }
    
    public void ShootTheDamageOrb() {
        Instantiate(damageOrb, shootingPoint.transform.position, Quaternion.LookRotation(shootingPoint.forward));
    }
}

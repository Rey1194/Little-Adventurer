using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootDamageOrb : MonoBehaviour
{
    public Transform  shootingPoint;
    public GameObject damageOrb;
    
    public void ShootTheDamageOrb() {
        Instantiate(damageOrb, shootingPoint.transform.position, Quaternion.LookRotation(shootingPoint.forward));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWeapons : MonoBehaviour
{
    public List<GameObject> weapons;
    
    public void DropSwords(){
        foreach (GameObject weapon in weapons) {
            weapon.AddComponent(typeof(Rigidbody));
            weapon.AddComponent(typeof(BoxCollider));
            weapon.transform.parent = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Variables
    public float currentHealth;
    public float maxHealth;
    private Character _cc;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        currentHealth = maxHealth;
        _cc = GetComponent<Character>();
    }

    public void ApplyDamage(float damage) {
        currentHealth -= damage;
        Debug.Log(gameObject.name + "took Damage" + damage);
        Debug.Log(gameObject.name + "current health" + currentHealth);
        CheckHealth();
    }
    
    public void CheckHealth() {
        if (currentHealth <= 0){
            _cc.SwitchStateTo(Character.CharacterState.dead);
        }
    }
}

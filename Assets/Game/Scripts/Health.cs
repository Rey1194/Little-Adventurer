using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Variables
    private Character _cc;
    public float maxHealth;
    public float currentHealth;
    public float currentHealthPercentage {
        get {
            return (float)currentHealth / (float) maxHealth;
        }
    }
    
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
    
    public void AddHealth (int healh) {
        currentHealth += healh;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }
}

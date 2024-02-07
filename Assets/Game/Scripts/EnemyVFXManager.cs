using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect Enemy_FootStep;
    public VisualEffect Enemy_Smash;
    public ParticleSystem beingHit;
    public VisualEffect beingHitSplashVFX;
    
    public void BurstFootStep() {
        Enemy_FootStep.SendEvent("OnPlay");
    }
    
    public void PlayAttackVFX() {
        Enemy_Smash.SendEvent("OnPlay");
    }
    
    public void PlayBeingHitVFX (Vector3 attackerPos) {
        // efecto de golpe
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y = 0;
        beingHit.transform.rotation = Quaternion.LookRotation(forceForward);
        beingHit.Play();
        // efecto de aceite
        Vector3 splashPos = transform.position;
        splashPos.y += 0.2f;
        VisualEffect newSplashVFX = Instantiate(beingHitSplashVFX, splashPos, Quaternion.identity);
        Destroy(newSplashVFX.gameObject, 10f);
    }
    
}

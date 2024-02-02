using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect Enemy_FootStep;
    public VisualEffect Enemy_Smash;
    
    public void BurstFootStep() {
        Enemy_FootStep.SendEvent("OnPlay");
    }
    public void PlayAttackVFX(){
        Enemy_Smash.SendEvent("OnPlay");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect Player_FootStep;
    public VisualEffect slashVFX;
    public ParticleSystem Blade01;
    
    public void Update_FootStep(bool state) {
        if (state) {
            Player_FootStep.Play();
        }
        else{
            Player_FootStep.Stop();
        }
    }
    
    public void PlayBlade01() {
        Blade01.Play();
    }
    
    public void PlaySlash(Vector3 pos) {
        slashVFX.transform.position = pos;
        slashVFX.Play();
    }
}

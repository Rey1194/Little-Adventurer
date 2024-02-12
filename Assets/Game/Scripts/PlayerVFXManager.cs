using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVFXManager : MonoBehaviour
{
    public VisualEffect Player_FootStep;
    public VisualEffect slashVFX;
    public VisualEffect healVFX;
    public ParticleSystem Blade01;
    public ParticleSystem Blade02;
    public ParticleSystem Blade03;
    
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
    
    public void PlayBlade02() {
        Blade02.Play();
    }
    
    public void PlayBlade03() {
        Blade03.Play();
    }
    
    public void StopBlade() {
        Blade01.Simulate(0);
        Blade01.Stop();
        
        Blade02.Simulate(0);
        Blade02.Stop();
        
        Blade03.Simulate(0);
        Blade03.Stop();
    }
    
    public void PlaySlash(Vector3 pos) {
        slashVFX.transform.position = pos;
        slashVFX.Play();
    }
    
    public void HealVFX() {
        healVFX.Play();
    }
}

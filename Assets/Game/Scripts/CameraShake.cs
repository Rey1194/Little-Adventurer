using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeTime = 0.2f;
    
    private CinemachineVirtualCamera cinemachineVC;
    private CinemachineBasicMultiChannelPerlin _cbmp;
    private float timer;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        instance = this;
        cinemachineVC = GetComponent<CinemachineVirtualCamera>();
    }
    
    // Start is called before the first frame update
    void Start() {
        StopShake();
    }

    // Update is called once per frame
    void Update() {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.G)) {
                ShakeCamera();
            }
        #endif
        
        if (timer > 0) {
            timer -= Time.deltaTime;
            
            if (timer <= 0) {
                StopShake();
            }
        }
        
        // llamar al shake funcion si se presiona mouse; 
    }
    
    public void ShakeCamera() {
        CinemachineBasicMultiChannelPerlin _cbmp = cinemachineVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmp.m_AmplitudeGain = shakeIntensity;
        timer = shakeTime;
    }
    
    private void StopShake() {
        CinemachineBasicMultiChannelPerlin _cbmp = cinemachineVC.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmp.m_AmplitudeGain = 0f;
        timer = 0;
    }
}

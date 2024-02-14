using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject gameVisual;
    private Collider _gateCollider;
    private float openDuration = 2f;
    private float openTargetY = -1.5f;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _gateCollider =GetComponent<Collider>();
    }
    
    IEnumerator OpenGateAnimation() {
        return null;
    }
}

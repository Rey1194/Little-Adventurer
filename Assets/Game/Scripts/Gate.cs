using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public GameObject gateVisual;
    private Collider _gateCollider;
    [SerializeField] private float openDuration = 2f;
    [SerializeField] private float openTargetY = -1.5f;
    
    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        _gateCollider =GetComponent<Collider>();
    }
    
    IEnumerator OpenGateAnimation() {
        yield return new WaitForSeconds(2f);
        SFXManager.instance.PlayAudio(5);
        float currentOpenDuration = 0;
        Vector3 startPos = gateVisual.transform.position;
        Vector3 targetPos = startPos + Vector3.up * openTargetY;
        
        while (currentOpenDuration < openDuration) {
            currentOpenDuration += Time.deltaTime;
            gateVisual.transform.position = Vector3.Lerp(startPos, targetPos, currentOpenDuration / openDuration);
            yield return null;
        }
        
        _gateCollider.enabled = false;
    }
    
    public void Open() {
        StartCoroutine(OpenGateAnimation());
    }
}

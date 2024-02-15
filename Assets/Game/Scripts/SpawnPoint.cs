using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject EnemyToSpawn;
    
    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
    protected void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3 (0f, 0.5f, 0);
        Gizmos.DrawWireCube(center, Vector3.one);
        Gizmos.DrawLine(center, center + transform.forward * 2);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> spawnPointList;
    private bool hasSpawned;
    public Collider _collider;
    
    // Awake is called when the script instance is being loaded.
    private void Awake() {
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
    }

    public void SpawnCharacters () {
        if (hasSpawned) {
            return;
        }
        
        hasSpawned = true;
        
        foreach (SpawnPoint point in spawnPointList) {
            if( point.EnemyToSpawn != null && point != null ) {
                GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, Quaternion.identity);
            }
        }
    }
    
    // OnTriggerEnter is called when the Collider other enters the trigger.
    protected void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            SpawnCharacters();
        }
    }
    
    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
    protected void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _collider.bounds.size);
    }
}

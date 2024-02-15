using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    private List<SpawnPoint> spawnPointList;
    private List<Character> spawnedCharacters;
    private bool hasSpawned;
    public Collider _collider;
    public  UnityEvent onAllSpawnedCharacterEliminated;
    
    // Awake is called when the script instance is being loaded.
    private void Awake() {
        var spawnPointArray = transform.parent.GetComponentsInChildren<SpawnPoint>();
        spawnPointList = new List<SpawnPoint>(spawnPointArray);
        spawnedCharacters = new List<Character>();
    }
    
    // Update is called every frame, if the MonoBehaviour is enabled.
    protected void Update() {
        if (!hasSpawned || spawnedCharacters.Count == 0){
            return;
        }
        
        bool allSpawnedAreDead = true;
        
        foreach (Character c in spawnedCharacters) {
            if (c.currentState != Character.CharacterState.dead) {
                allSpawnedAreDead = false;
                break;
            }
        }
        
        if (allSpawnedAreDead) {
            if (onAllSpawnedCharacterEliminated != null) {
                onAllSpawnedCharacterEliminated.Invoke();
            }
            spawnedCharacters.Clear();
        }
    }

    public void SpawnCharacters () {
        if (hasSpawned) {
            return;
        }
        
        hasSpawned = true;
        
        foreach (SpawnPoint point in spawnPointList) {
            if( point.EnemyToSpawn != null && point != null ) {
                GameObject spawnedGameObject = Instantiate(point.EnemyToSpawn, point.transform.position, point.transform.rotation);
                spawnedCharacters.Add(spawnedGameObject.GetComponent<Character>());
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

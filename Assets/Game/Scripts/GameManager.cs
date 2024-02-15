using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Character playerCharacter;
    private bool gameIsOver;
    
    // Start is called before the first frame update
    void awake()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }
    
    public void GameOver() {
        Debug.LogWarning("Game Over");
    }
    
    public void GameIsFinished() {
        Debug.Log("Game is Finished");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver) {
            return;
        }
        
        if (playerCharacter.currentState == Character.CharacterState.dead) {
            gameIsOver = true;
            GameOver();
        }

    }
    
}

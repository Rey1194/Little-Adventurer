using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character playerCharacter;
    public GameUI_Manager UIManager;
    private ButtonSound buttonSFX;
    private bool gameIsOver;
    
    // Start is called before the first frame update
    void awake() {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }
    
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    protected void Start() {
        buttonSFX = GameObject.FindGameObjectWithTag("SFXManager").GetComponent<ButtonSound>();
    }
    
    public void GameOver() {
        UIManager.ShowGameOverUI();
    }
    
    public void GameIsFinished() {
        UIManager.ShowGameFinishedUI();
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
        
        if ( Input.GetKeyDown(KeyCode.Escape) ) {
            // play sfx button
            buttonSFX.PlayButtonSound();
            UIManager.TogglePauseUI();
        }

    }
    
    public void Restart() {
        // reinicia la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ReturnMainMenu() {
        // regresa al menú principal
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    
}

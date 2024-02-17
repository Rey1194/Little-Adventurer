using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenu_Manager : MonoBehaviour
{
    // Botones
    public Button startButton;
    public Button quitButton;
    public Button[] buttons;
    
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    protected void Start() {
        // manejo del UI
        foreach (Button button in buttons) {
            button.onClick.AddListener( () => ButtonClicked(button) );
        }
    }
    
    void ButtonClicked(Button clickedButton) {
        if ( clickedButton == startButton ) {
            Button_Start();
        }
        else if( clickedButton == quitButton ) {
            Button_Quit();
        }
    }
    
    public void Button_Start() {
        SceneManager.LoadScene("GameScene");
    }
    
    public void Button_Quit(){
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }
}

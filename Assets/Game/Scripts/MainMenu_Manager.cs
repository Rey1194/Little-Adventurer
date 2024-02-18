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
    public AudioSource audioSource;
    public AudioClip buttonSound;
    
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    protected void Start() {
        // Obtener el componente AudioSource si no se ha asignado
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }
        // manejo del UI
        foreach (Button button in buttons) {
            button.onClick.AddListener( () => ButtonClicked(button) );
        }
    }
    
    void ButtonClicked(Button clickedButton) {
        if ( clickedButton == startButton ) {
            PlayButtonSound();
            Button_Start();
        }
        else if( clickedButton == quitButton ) {
            PlayButtonSound();
            Button_Quit();
        }
    }
    
    public void PlayButtonSound() {
        audioSource.PlayOneShot(buttonSound);
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

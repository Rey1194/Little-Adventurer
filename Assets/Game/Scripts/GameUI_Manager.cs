using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI_Manager : MonoBehaviour
{
    public GameManager GM;
    public TMPro.TextMeshProUGUI coinText;
    public Slider healthSlider;
    // Canvas
    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinished;
    // Buttons
    public Button Pause_Button_Resume;
    public Button Pause_Button_Restart;
    public Button Pause_Button_MainMenu;
    public Button GameOver_Button_Restart;
    public Button GameOver_Button_MainMenu;
    public Button GameFinished_Button_Restart;
    public Button GameFinished_Button_MainMenu;
    public Button[] buttons;
    // SFX
    private ButtonSound buttonSFX;
    // state machine
    public enum GameUI_State {
        GamePlay,
        Pause,
        GameOver,
        GameIsFinished,
    }
    public GameUI_State currentState;
    
    // Start is called before the first frame update
    void Start() {
        
        SwitchUIState(GameUI_State.GamePlay);
        // encontrar en la escena al SFX Manager
        buttonSFX = GameObject.FindGameObjectWithTag("SFXManager").GetComponent<ButtonSound>();
        
        // manejo de los botones en el UI
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => ButtonClicked(button));
        }
        
    }

    // Update is called once per frame
    void Update() {
        // se busca el componente health del player que está asignado en el game manager
        healthSlider.value = GM.playerCharacter.GetComponent<Health>().currentHealthPercentage;
        // se hace lo mismo con el coin pero se lo convierte a string
        coinText.text = GM.playerCharacter.totalCoins.ToString();
    }
    
    void ButtonClicked(Button clickedButton)
    {
        
        if (clickedButton == Pause_Button_Restart || clickedButton == GameOver_Button_Restart || clickedButton == GameFinished_Button_Restart)
        {
            buttonSFX.PlayButtonSound();
            GM.Restart();
        }
        else if (clickedButton == Pause_Button_MainMenu || clickedButton == GameOver_Button_MainMenu || clickedButton == GameFinished_Button_MainMenu)
        {
            buttonSFX.PlayButtonSound();
            GM.ReturnMainMenu();
        }
        else if (clickedButton == Pause_Button_Resume) {
            buttonSFX.PlayButtonSound();
            SwitchUIState(GameUI_State.GamePlay);
        }
    }
    
    public void SwitchUIState(GameUI_State state) {
        
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);
        
        Time.timeScale = 1;
        
        switch (state){
        case GameUI_State.GamePlay:
            break;
        case GameUI_State.Pause:
            Time.timeScale = 0;
            UI_Pause.SetActive(true);
            break;
        case GameUI_State.GameOver:
            UI_GameOver.SetActive(true);
            break;
        case GameUI_State.GameIsFinished:
            UI_GameIsFinished.SetActive(true);
            break;
        }
        
        currentState = state;
    }
        
    public void TogglePauseUI() {
        if (currentState == GameUI_State.GamePlay) {
            SwitchUIState(GameUI_State.Pause);
        }
        else if (currentState == GameUI_State.Pause) {
            SwitchUIState(GameUI_State.GamePlay);
        }
    }
    
    public void ShowGameOverUI() {
        SwitchUIState(GameUI_State.GameOver);
    }
    
    public void ShowGameFinishedUI() {
        SwitchUIState(GameUI_State.GameIsFinished);
    }
}

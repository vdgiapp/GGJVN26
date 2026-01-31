using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set;}
    
    public UIManager ui;
    public CustomerManager customerManager;
    public TypewriterManager typewriter;
    public PlayerController player;
    
    [Header("Camera")]
    public Camera menuCamera;
    
    [Header("Audio")]
    public AudioSource musicSource;
    
    [HideInInspector] public bool isGameStarted = false;
    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public bool isPaused = false;
    
    private void Awake()
    {
        instance = this;
    }
    
    public void StartGame()
    {
        ui.ShowGameUI();

        isGameStarted = true;
        
        menuCamera.enabled = false;
        menuCamera.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
        
        musicSource.spatialBlend = 0.75f;

        customerManager.SpawnCustomer(0);
        
        ResumeGame();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
        
        ui.ShowPauseUI(true);
        isPaused = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        
        ui.ShowPauseUI(false);
        isPaused = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        
        ui.ShowGameOverUI(true);
        isGameOver = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
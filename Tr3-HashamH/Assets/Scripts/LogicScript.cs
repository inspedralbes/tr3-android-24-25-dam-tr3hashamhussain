using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System;

public class LogicScript : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject gameOverScreen;
    public Text scoreText;
    public int pipesToWin = 15;
    public PipeSpawnScript pipeSpawnScript;
    public BirdScript birdScript;
    public Button restartButton; // Referencia explícita al botón
    public Button restartWinButton; // Referencia explícita al botón
    public Button exitButton; // Nuevo botón de salir
    public Button exitWinButton; // Nuevo botón de salir en win screen
    private string playerId;
    private string playerName = "Jugador";
    private int playerScore = 0;
    private bool gameEnded = false;

    void Start()
    {
        playerId = Guid.NewGuid().ToString();
        UpdateScoreDisplay();
        
        // Configurar el botón de reinicio si está asignado
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (restartWinButton != null)
            restartWinButton.onClick.AddListener(RestartGame);
            
        // Configurar botones de salir
        if (exitButton != null)
            exitButton.onClick.AddListener(GoToMainMenu);
            
        if (exitWinButton != null)
            exitWinButton.onClick.AddListener(GoToMainMenu);
    }

    public void AddScore(int scoreToAdd)
    {
        if (gameEnded) return;
        
        playerScore += scoreToAdd;
        UpdateScoreDisplay();

        if (playerScore >= pipesToWin)
        {
            WinGame();
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = playerScore.ToString();
    }

    public void RestartGame()
    {
        // Asegurarse de que el tiempo se reanude
        Time.timeScale = 1;
        
        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        // Alternativa: SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
        
        Time.timeScale = 0;
        SendStatToBackend();
    }

    public void WinGame()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        
        if (winScreen != null)
            winScreen.SetActive(true);
        
        Time.timeScale = 0;
        SendStatToBackend();
    }

    private void SendStatToBackend()
    {
        int jumps = birdScript != null ? birdScript.jumps : 0;
        string gameMode = SceneManager.GetActiveScene().name;
        
        StartCoroutine(SendStat(playerId, playerName, jumps, playerScore, gameMode));
    }

    private IEnumerator SendStat(string playerId, string playerName, int jumps, int pipesPassed, string gameMode)
    {
        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);
        form.AddField("playerName", playerName);
        form.AddField("jumps", jumps);
        form.AddField("pipesPassed", pipesPassed);
        form.AddField("gameMode", gameMode);

        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://localhost:3000/stats", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al enviar estadísticas: " + webRequest.error);
            }
            else
            {
                Debug.Log("Estadísticas enviadas correctamente");
            }
        }
    }
     public void GoToMainMenu()
    {
        // Asegurarse de que el tiempo se reanude
        Time.timeScale = 1;
        
        // Cargar la escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class LogicScript : MonoBehaviour
{
    // Tus variables públicas originales
    public GameObject winScreen;
    public GameObject gameOverScreen;
    public Text scoreText;
    public int pipesToWin = 15;
    public PipeSpawnScript pipeSpawnScript;
    public BirdScript birdScript;
    public Button restartButton;
    public Button restartWinButton;
    public Button exitButton;
    public Button exitWinButton;
    
    private string playerId;
    private string playerName = "Jugador";
    private int playerScore = 0;
    private bool gameEnded = false;
    private float lastStatUpdateTime = 0f;
    private const float statUpdateInterval = 5f; // Actualizar cada 5 segundos

    void Start()
    {
        playerId = SystemInfo.deviceUniqueIdentifier;
        UpdateScoreDisplay();
        
        // Configuración de botones original
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (restartWinButton != null) restartWinButton.onClick.AddListener(RestartGame);
        if (exitButton != null) exitButton.onClick.AddListener(GoToMainMenu);
        if (exitWinButton != null) exitWinButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        // Sistema de polling para actualizaciones
        if (Time.time - lastStatUpdateTime > statUpdateInterval)
        {
            lastStatUpdateTime = Time.time;
            StartCoroutine(CheckForUpdates());
        }
    }

    // Tus métodos originales sin cambios
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
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        StartCoroutine(SendStatToBackend());
    }

    public void WinGame()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        if (winScreen != null) winScreen.SetActive(true);
        Time.timeScale = 0;
        StartCoroutine(SendStatToBackend());
    }

    private IEnumerator SendStatToBackend()
{
    int jumps = birdScript != null ? birdScript.jumps : 0;
    string gameMode = SceneManager.GetActiveScene().name;
    
    StatData statData = new StatData
    {
        playerId = playerId,
        playerName = playerName,
        jumps = jumps,
        pipesPassed = playerScore,
        gameMode = gameMode
    };

    string jsonData = JsonUtility.ToJson(statData);
    string url = "http://localhost:3300/stats";
    
    using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar estadísticas: " + webRequest.error);
        }
        else
        {
            Debug.Log("Estadísticas enviadas correctamente");
            
            // Opcional: Verificar actualización inmediata
            StartCoroutine(CheckForUpdates());
        }
    }
}

    private IEnumerator CheckForUpdates()
    {
        string url = "http://localhost:3300/stats/recent";
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Aquí podrías procesar la respuesta si necesitas algo específico
                Debug.Log("Comprobación de actualizaciones exitosa");
            }
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    [System.Serializable]
    private class StatData
    {
        public string playerId;
        public string playerName;
        public int jumps;
        public int pipesPassed;
        public string gameMode;
    }
}
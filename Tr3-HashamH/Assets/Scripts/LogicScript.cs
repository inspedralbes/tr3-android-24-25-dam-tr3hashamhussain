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
    public int playerScore;
    public Text scoreText;
    public int pipesToWin = 15;
    public PipeSpawnScript pipeSpawnScript;
    public BirdScript birdScript;

    private string playerId; // ID único del jugador
    private string playerName = "Jugador"; // Nombre predeterminado

    void Start()
    {
        // Generar un ID único para el jugador
        playerId = Guid.NewGuid().ToString();

        // Configurar valores iniciales para el nivel fácil
        if (SceneManager.GetActiveScene().name == "M-Facil")
        {
            pipeSpawnScript.spawnRate = 2.5f;
            birdScript.flapStrength = 14f;
        }
    }

    [ContextMenu("Increase Score")]
    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd;
        scoreText.text = playerScore.ToString();

        // Verificar si el jugador ha ganado
        if (playerScore >= pipesToWin)
        {
            WinGame();
        }
    }

    public void restartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;
        SendStatToBackend();
    }

    public void WinGame()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0;
        SendStatToBackend();
    }

    // Método para enviar estadísticas al backend
    private void SendStatToBackend()
    {
        int jumps = birdScript.jumps; // Obtener el número de saltos
        int pipesPassed = playerScore; // Tuberías pasadas
        string gameMode = SceneManager.GetActiveScene().name; // Modo de juego

        StartCoroutine(SendStat(playerId, playerName, jumps, pipesPassed, gameMode));
    }

    // Coroutine para enviar estadísticas al backend
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

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al enviar estadísticas: " + webRequest.error);
            }
            else
            {
                Debug.Log("Estadísticas enviadas correctamente");
            }
        }
    }
}
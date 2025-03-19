using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public GameObject winScreen; // Pantalla de victoria
    public GameObject gameOverScreen; // Pantalla de game over
    public int playerScore;
    public Text scoreText;
    public int pipesToWin = 5; // Número de tuberías para ganar

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
        Time.timeScale = 1; // Restablecer el tiempo del juego
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0; // Detener el tiempo del juego
    }

    public void WinGame()
    {
        winScreen.SetActive(true); // Mostrar pantalla de victoria
        Time.timeScale = 0; // Detener el tiempo del juego
    }
}
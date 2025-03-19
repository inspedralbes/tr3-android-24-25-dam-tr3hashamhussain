using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameSettingsLoader : MonoBehaviour
{
    public string gameSettingsUrl = "http://localhost:3000/game-settings"; // Endpoint para obtener la configuración
    public BirdScript birdScript; // Referencia al script del pájaro
    public PipeSpawnScript pipeSpawnScript; // Referencia al script de generación de tuberías

    void Start()
    {
        StartCoroutine(LoadGameSettings());
    }

    IEnumerator LoadGameSettings()
    {
        // Obtener la configuración del juego desde el servidor
        using (UnityWebRequest webRequest = UnityWebRequest.Get(gameSettingsUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al obtener la configuración del juego: " + webRequest.error);
            }
            else
            {
                // Parsear la respuesta JSON
                string jsonResponse = webRequest.downloadHandler.text;
                GameSettings gameSettings = JsonUtility.FromJson<GameSettings>(jsonResponse);

                // Aplicar los valores al juego
                if (birdScript != null)
                {
                    birdScript.flapStrength = gameSettings.flapStrength;
                }

                if (pipeSpawnScript != null)
                {
                    pipeSpawnScript.spawnRate = gameSettings.pipeSpawnRate;
                }

                Debug.Log("Configuración del juego actualizada.");
            }
        }
    }

    [System.Serializable]
    private class GameSettings
    {
        public float flapStrength;
        public float pipeSpawnRate;
    }
}
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameSettingsLoader : MonoBehaviour
{
    public string settingsUrl = "http://localhost:3000/game-settings";
    public BirdScript birdScript;
    public PipeSpawnScript pipeSpawnScript;
    public float checkInterval = 1.5f;
    
    [Header("Debug")]
    public GameSettings lastSettings;
    
    private float timer = 0f;
    private List<PipeMoveScript> activePipes = new List<PipeMoveScript>();
    private bool settingsLoaded = false;

    void Start()
    {
        StartCoroutine(InitialLoad());
    }

    IEnumerator InitialLoad()
    {
        yield return new WaitForSeconds(0.5f); // Pequeña espera inicial
        yield return StartCoroutine(LoadGameSettings(true));
    }

    void Update()
    {
        if (!settingsLoaded) return;
        
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            StartCoroutine(LoadGameSettings());
            timer = 0f;
        }
    }

    public void RegisterPipe(PipeMoveScript pipe)
    {
        if (pipe == null || activePipes.Contains(pipe)) return;
        
        activePipes.Add(pipe);
        if (lastSettings != null)
        {
            pipe.MoveSpeed = lastSettings.pipeMoveSpeed;
            Debug.Log($"Nueva tubería registrada. Velocidad: {pipe.MoveSpeed}");
        }
    }

    public void UnregisterPipe(PipeMoveScript pipe)
    {
        if (pipe != null && activePipes.Contains(pipe))
        {
            activePipes.Remove(pipe);
        }
    }

    IEnumerator LoadGameSettings(bool isInitialLoad = false)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(settingsUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                ProcessSettings(webRequest.downloadHandler.text, isInitialLoad);
            }
            else if (isInitialLoad)
            {
                Debug.LogWarning("Fallo al cargar configuración inicial. Usando valores por defecto.");
                ApplyDefaultSettings();
            }
        }
    }

    void ProcessSettings(string jsonSettings, bool isInitialLoad)
    {
        GameSettings newSettings = JsonUtility.FromJson<GameSettings>(jsonSettings);
        
        if (newSettings == null)
        {
            Debug.LogError("Error al procesar configuración: JSON inválido");
            return;
        }

        if (isInitialLoad || !SettingsEqual(lastSettings, newSettings))
        {
            ApplySettings(newSettings);
            lastSettings = newSettings;
            settingsLoaded = true;
            
            Debug.Log($"Configuración {(isInitialLoad ? "inicial" : "actualizada")}:\n" + 
                     $"• Fuerza salto: {newSettings.flapStrength}\n" +
                     $"• Ratio spawn: {newSettings.pipeSpawnRate}s\n" +
                     $"• Velocidad: {newSettings.pipeMoveSpeed}\n" +
                     $"• Enemigos: {newSettings.enemySpawnChance}%");
        }
    }

    void ApplySettings(GameSettings settings)
    {
        // Aplicar al pájaro
        if (birdScript != null)
        {
            birdScript.flapStrength = settings.flapStrength;
        }
        
        // Aplicar al spawner de tuberías
        if (pipeSpawnScript != null)
        {
            pipeSpawnScript.spawnRate = settings.pipeSpawnRate;
            pipeSpawnScript.enemySpawnChance = (int)settings.enemySpawnChance;
        }
        
        // Aplicar a todas las tuberías existentes
        UpdateAllPipesSpeed(settings.pipeMoveSpeed);
    }

    void ApplyDefaultSettings()
    {
        GameSettings defaults = new GameSettings 
        {
            flapStrength = 10f,
            pipeSpawnRate = 2f,
            pipeMoveSpeed = 9.5f,
            enemySpawnChance = 25f
        };
        
        ApplySettings(defaults);
        lastSettings = defaults;
        settingsLoaded = true;
    }

    void UpdateAllPipesSpeed(float newSpeed)
    {
        // Limpiar tuberías destruidas
        activePipes.RemoveAll(pipe => pipe == null);
        
        // Actualizar todas las tuberías activas
        foreach (var pipe in activePipes)
        {
            pipe.MoveSpeed = newSpeed;
        }
        
        Debug.Log($"Velocidad actualizada a {newSpeed} en {activePipes.Count} tuberías activas");
    }

    bool SettingsEqual(GameSettings a, GameSettings b)
    {
        if (a == null || b == null) return false;
        return Mathf.Approximately(a.flapStrength, b.flapStrength) &&
               Mathf.Approximately(a.pipeSpawnRate, b.pipeSpawnRate) &&
               Mathf.Approximately(a.pipeMoveSpeed, b.pipeMoveSpeed) &&
               Mathf.Approximately(a.enemySpawnChance, b.enemySpawnChance);
    }
}
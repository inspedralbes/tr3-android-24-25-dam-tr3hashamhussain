using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System;
using System.Net;
public class GameSettingsLoader : MonoBehaviour
{
    [Header("Configuración de Conexión")]
    [Tooltip("URL completa del endpoint de configuración del juego")]
    public string settingsUrl = "http://localhost:3400/game-settings";
    
    [Tooltip("Intervalo en segundos para verificar actualizaciones")]
    public float checkInterval = 2f;
    
    [Header("Referencias")]
    [Tooltip("Referencia al script del pájaro")]
    public BirdScript birdScript;
    
    [Tooltip("Referencia al script de generación de tuberías")]
    public PipeSpawnScript pipeSpawnScript;

    [Header("Debug")]
    [Tooltip("Última configuración cargada")]
    public GameSettings lastSettings;
    
    [Tooltip("Mostrar logs detallados")]
    public bool verboseLogging = true;

    private float timer = 0f;
    private List<PipeMoveScript> activePipes = new List<PipeMoveScript>();
    private bool settingsLoaded = false;
    private bool initialLoadCompleted = false;

    [Serializable]
    public class GameSettings
    {
        public float flapStrength;
        public float pipeSpawnRate;
        public float pipeMoveSpeed;
        public float enemySpawnChance;
    }

    void Awake()
    {
        // Configuración para desarrollo - deshabilita verificación SSL
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (verboseLogging) Debug.Log("Configurando callback de certificado para desarrollo");
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        #endif
    }

    void Start()
    {
        if (string.IsNullOrEmpty(settingsUrl))
        {
            Debug.LogError("La URL de configuración no está definida!");
            ApplyDefaultSettings();
            return;
        }

        StartCoroutine(InitialLoad());
    }

    void Update()
    {
        if (!initialLoadCompleted) return;
        
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            StartCoroutine(FetchGameSettings());
            timer = 0f;
        }
    }

    IEnumerator InitialLoad()
    {
        if (verboseLogging) Debug.Log("Iniciando carga inicial de configuración...");
        
        yield return new WaitForSeconds(0.5f); // Pequeña espera inicial

        int retries = 3;
        bool success = false;
        
        for (int i = 0; i < retries; i++)
        {
            if (verboseLogging) Debug.Log($"Intento {i + 1} de {retries}...");
            
            yield return StartCoroutine(FetchGameSettings(true));
            
            if (settingsLoaded)
            {
                success = true;
                break;
            }
            
            yield return new WaitForSeconds(1f); // Espera entre reintentos
        }

        if (!success)
        {
            Debug.LogWarning("No se pudo cargar la configuración inicial después de varios intentos. Usando valores por defecto.");
            ApplyDefaultSettings();
        }

        initialLoadCompleted = true;
    }

    IEnumerator FetchGameSettings(bool isInitialLoad = false)
    {
        if (verboseLogging) Debug.Log($"Solicitando configuración desde: {settingsUrl}");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(settingsUrl))
        {
            #if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            webRequest.certificateHandler = new BypassCertificate();
            #endif
            
            webRequest.timeout = 3; // Timeout de 3 segundos

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                if (verboseLogging) Debug.Log($"Respuesta recibida: {webRequest.downloadHandler.text}");
                ProcessSettings(webRequest.downloadHandler.text, isInitialLoad);
            }
            else
            {
                Debug.LogWarning($"Error al cargar configuración: {webRequest.error}");
                
                if (isInitialLoad && !settingsLoaded)
                {
                    Debug.LogWarning("Usando valores por defecto para carga inicial.");
                    ApplyDefaultSettings();
                }
            }
        }
    }

    void ProcessSettings(string jsonSettings, bool isInitialLoad)
    {
        try
        {
            GameSettings newSettings = JsonUtility.FromJson<GameSettings>(jsonSettings);
            
            if (newSettings == null)
            {
                Debug.LogError("Error al procesar configuración: JSON inválido");
                return;
            }

            // Validación de valores
            newSettings.flapStrength = Mathf.Clamp(newSettings.flapStrength, 5f, 20f);
            newSettings.pipeSpawnRate = Mathf.Clamp(newSettings.pipeSpawnRate, 0.5f, 5f);
            newSettings.pipeMoveSpeed = Mathf.Clamp(newSettings.pipeMoveSpeed, 1f, 30f);
            newSettings.enemySpawnChance = Mathf.Clamp(newSettings.enemySpawnChance, 0f, 100f);

            if (isInitialLoad || !SettingsEqual(lastSettings, newSettings))
            {
                ApplySettings(newSettings);
                lastSettings = newSettings;
                settingsLoaded = true;
                
                if (verboseLogging)
                {
                    Debug.Log($"Configuración {(isInitialLoad ? "inicial" : "actualizada")}:\n" + 
                             $"• Fuerza salto: {newSettings.flapStrength}\n" +
                             $"• Ratio spawn: {newSettings.pipeSpawnRate}s\n" +
                             $"• Velocidad: {newSettings.pipeMoveSpeed}\n" +
                             $"• Enemigos: {newSettings.enemySpawnChance}%");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepción al procesar configuración: {ex.Message}");
            
            if (isInitialLoad && !settingsLoaded)
            {
                Debug.LogWarning("Aplicando valores por defecto debido a error de procesamiento");
                ApplyDefaultSettings();
            }
        }
    }

    void ApplySettings(GameSettings settings)
    {
        // Aplicar al pájaro
        if (birdScript != null)
        {
            birdScript.flapStrength = settings.flapStrength;
        }
        else
        {
            Debug.LogWarning("BirdScript no asignado en el inspector");
        }
        
        // Aplicar al spawner de tuberías
        if (pipeSpawnScript != null)
        {
            pipeSpawnScript.spawnRate = settings.pipeSpawnRate;
            pipeSpawnScript.enemySpawnChance = (int)settings.enemySpawnChance;
        }
        else
        {
            Debug.LogWarning("PipeSpawnScript no asignado en el inspector");
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
        
        if (verboseLogging) Debug.Log("Configuración por defecto aplicada");
    }

    void UpdateAllPipesSpeed(float newSpeed)
    {
        // Limpiar tuberías destruidas
        activePipes.RemoveAll(pipe => pipe == null);
        
        // Actualizar todas las tuberías activas
        foreach (var pipe in activePipes)
        {
            if (pipe != null)
            {
                pipe.MoveSpeed = newSpeed;
            }
        }
        
        if (verboseLogging) Debug.Log($"Velocidad actualizada a {newSpeed} en {activePipes.Count} tuberías activas");
    }

    bool SettingsEqual(GameSettings a, GameSettings b)
    {
        if (a == null || b == null) return false;
        return Mathf.Approximately(a.flapStrength, b.flapStrength) &&
               Mathf.Approximately(a.pipeSpawnRate, b.pipeSpawnRate) &&
               Mathf.Approximately(a.pipeMoveSpeed, b.pipeMoveSpeed) &&
               Mathf.Approximately(a.enemySpawnChance, b.enemySpawnChance);
    }

    public void RegisterPipe(PipeMoveScript pipe)
    {
        if (pipe == null || activePipes.Contains(pipe)) return;
        
        activePipes.Add(pipe);
        if (lastSettings != null)
        {
            pipe.MoveSpeed = lastSettings.pipeMoveSpeed;
            if (verboseLogging) Debug.Log($"Nueva tubería registrada. Velocidad: {pipe.MoveSpeed}");
        }
    }

    public void UnregisterPipe(PipeMoveScript pipe)
    {
        if (pipe != null && activePipes.Contains(pipe))
        {
            activePipes.Remove(pipe);
            if (verboseLogging) Debug.Log($"Tubería eliminada. Tuberías activas: {activePipes.Count}");
        }
    }

    // Método para manejar certificados en desarrollo
    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // Acepta todos los certificados en desarrollo
        return true;
    }
}

// Clase para manejar certificados en producción
public class BypassCertificate : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // En producción, deberías implementar una validación real aquí
        return true;
    }
}
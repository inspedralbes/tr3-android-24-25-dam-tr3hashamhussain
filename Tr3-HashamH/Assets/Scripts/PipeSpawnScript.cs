using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public GameObject pipeEnemy;
    public float spawnRate = 2f;
    public float heightOffset = 10f;
    [Range(0, 100)] public int enemySpawnChance = 25;
    
    private float timer = 0f;
    private GameSettingsLoader settingsLoader;

    void Start()
    {
        settingsLoader = FindObjectOfType<GameSettingsLoader>();
        if (settingsLoader == null)
        {
            Debug.LogError("GameSettingsLoader no encontrado en la escena!");
        }
    }

    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    void SpawnPipe()
    {
        if (pipe == null) return;

        Vector3 spawnPos = new Vector3(
            transform.position.x,
            Random.Range(transform.position.y - heightOffset, transform.position.y + heightOffset),
            0
        );

        GameObject newPipe = Instantiate(
            ShouldSpawnEnemy() ? pipeEnemy : pipe,
            spawnPos,
            Quaternion.identity
        );

        ApplyCurrentSettings(newPipe);
    }

    bool ShouldSpawnEnemy()
    {
        return pipeEnemy != null && Random.Range(0, 100) < enemySpawnChance;
    }

    void ApplyCurrentSettings(GameObject pipeObj)
    {
        if (settingsLoader == null || settingsLoader.lastSettings == null) return;

        var moveScript = pipeObj.GetComponent<PipeMoveScript>();
        if (moveScript != null)
        {
            moveScript.MoveSpeed = settingsLoader.lastSettings.pipeMoveSpeed;
            Debug.Log($"Tubería creada con velocidad: {moveScript.MoveSpeed}");
        }
    }
}
using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public GameObject pipeEnemy; // Añadir referencia al prefab PipeEnemy
    public float spawnRate = 2; // Hacer pública esta variable
    private float timer = 0;
    public float heightOffset = 10;

    void Start()
    {
        spawnPipe();
    }

    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            spawnPipe();
            timer = 0;
        }
    }

    void spawnPipe()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        Vector3 spawnPosition = new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0);

        // 25% de probabilidad de spawnear un PipeEnemy
        if (Random.Range(0, 100) < 25)
        {
            Instantiate(pipeEnemy, spawnPosition, transform.rotation);
        }
        else
        {
            Instantiate(pipe, spawnPosition, transform.rotation);
        }
    }
}
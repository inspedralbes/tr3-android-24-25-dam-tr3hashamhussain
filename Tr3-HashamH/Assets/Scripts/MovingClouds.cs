using UnityEngine;

public class MovingClouds : MonoBehaviour
{
    public GameObject cloudPrefab; // The cloud prefab
    public float spawnInterval = 2f; // Time between spawns
    public Transform[] spawnPoints; // Array of spawn points
    public float deadZone = -35f; // Position where clouds get destroyed

    private void Start()
    {
        // Start spawning clouds
        InvokeRepeating("SpawnCloud", 0f, spawnInterval);
    }

    void SpawnCloud()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.Log("No spawn points available.");
            return; // No spawn points available
        }

        // Choose a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the cloud prefab at the spawn point
        GameObject cloud = Instantiate(cloudPrefab, spawnPoint.position, Quaternion.identity);

        // Add the CloudMove component to handle movement and deletion
        cloud.AddComponent<CloudMove>();

        Debug.Log("Spawned cloud at: " + spawnPoint.position);
    }

    private void OnDrawGizmos()
    {
        // Draw spawn points in the scene view
        Gizmos.color = Color.red;
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}

// Script para mover las nubes y eliminarlas al llegar a la deadZone
public class CloudMove : MonoBehaviour
{
    public float moveSpeed = 2.0f; // Velocidad del movimiento
    private float deadZone = -35f; // Punto de eliminación

    void Update()
    {
        // Movimiento hacia la izquierda
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Si la nube cruza la deadZone, se destruye
        if (transform.position.x < deadZone)
        {
            Debug.Log("Cloud Eliminated");
            Destroy(gameObject);
        }
    }
}

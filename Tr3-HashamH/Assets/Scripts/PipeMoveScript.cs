using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
    public float moveSpeed = 9.5f; // Velocidad fija de las tuberías
    public float deadZone = -35;

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Eliminar la tubería si sale de la pantalla
        if (transform.position.x < deadZone)
        {
            Debug.Log("Pipe Eliminado");
            Destroy(gameObject);
        }
    }
}
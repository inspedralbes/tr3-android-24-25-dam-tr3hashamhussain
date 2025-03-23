using UnityEngine;

public class TopPipeEnemy : MonoBehaviour
{
    public float moveDistance = 4f; // Distancia que se moverá el enemigo hacia abajo
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position; // Guardar la posición original
    }

    public void MoveEnemy()
    {
        Vector3 newPosition = transform.position; // Usar la posición actual
        newPosition.y -= moveDistance; // Mover hacia abajo en el eje Y
        transform.position = newPosition;
    }
}
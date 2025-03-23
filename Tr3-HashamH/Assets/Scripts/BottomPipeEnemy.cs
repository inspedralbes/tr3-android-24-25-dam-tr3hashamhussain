using UnityEngine;

public class BottomPipeEnemy : MonoBehaviour
{
    public float moveDistance = 4f; // Distancia que se moverá el enemigo hacia arriba
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position; // Guardar la posición original
    }

    public void MoveEnemy()
    {
        Vector3 newPosition = transform.position; // Usar la posición actual
        newPosition.y += moveDistance; // Mover hacia arriba en el eje Y
        transform.position = newPosition;
    }
}
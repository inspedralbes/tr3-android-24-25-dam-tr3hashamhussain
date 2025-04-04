using UnityEngine;

public class AvisadorScript : MonoBehaviour
{
    public TopPipeEnemy topEnemy; // Referencia al enemigo de arriba
    public BottomPipeEnemy bottomEnemy; // Referencia al enemigo de abajo

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tenga el tag "Player"
        {
            // Elegir aleatoriamente entre el enemigo de arriba o el de abajo
            int randomChoice = Random.Range(0, 2); // 0 o 1

            if (randomChoice == 0 && topEnemy != null)
            {
                topEnemy.MoveEnemy(); // Mover el enemigo de arriba
            }
            else if (randomChoice == 1 && bottomEnemy != null)
            {
                bottomEnemy.MoveEnemy(); // Mover el enemigo de abajo
            }
        }
    }
}
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public float flapStrength;
    public float downForceMultiplier = 1.5f; // Multiplicador de fuerza hacia abajo
    public LogicScript logic;
    public bool birdIsAlive = true;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void Update()
    {
        // Detectar tanto la tecla de espacio como el clic derecho del mouse
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && birdIsAlive)
        {
            rigidbody2D.linearVelocity = Vector2.up * flapStrength;
        }

        // Aplicar fuerza adicional hacia abajo cuando no se presiona la barra espaciadora ni el clic derecho
        if (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0) && birdIsAlive && rigidbody2D.linearVelocity.y < 0)
        {
            rigidbody2D.linearVelocity += Vector2.down * downForceMultiplier * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameOver();
        birdIsAlive = false;
    }
}
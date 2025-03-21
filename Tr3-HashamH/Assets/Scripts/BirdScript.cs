using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public Rigidbody2D rigidbody2D;
    public float flapStrength = 10;
    public float downForceMultiplier = 1.5f;
    public LogicScript logic;
    public bool birdIsAlive = true;
    public int jumps = 0; // Contador de saltos

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && birdIsAlive)
        {
            rigidbody2D.linearVelocity = Vector2.up * flapStrength;
            jumps++; // Incrementar el contador de saltos
        }

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
using UnityEngine;

public class PipeMoveScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 9.5f;
    public float deadZone = -35f;
    
    public float MoveSpeed
    {
        get => moveSpeed;
        set
        {
            if (Mathf.Abs(moveSpeed - value) > 0.01f)
            {
                moveSpeed = value;
                Debug.Log($"{gameObject.name} velocidad actualizada: {moveSpeed}");
            }
        }
    }

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x < deadZone)
        {
            DestroyPipe();
        }
    }

    void OnEnable()
    {
        var loader = FindObjectOfType<GameSettingsLoader>();
        if (loader != null) loader.RegisterPipe(this);
    }

    void OnDisable()
    {
        DestroyPipe();
    }

    void DestroyPipe()
    {
        var loader = FindObjectOfType<GameSettingsLoader>();
        if (loader != null) loader.UnregisterPipe(this);
        
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
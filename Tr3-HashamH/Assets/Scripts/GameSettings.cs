using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public float flapStrength = 10f;
    public float pipeSpawnRate = 2f;
    public float pipeMoveSpeed = 9.5f;
    public float enemySpawnChance = 25f;

    public override string ToString()
    {
        return $"Velocidad: {pipeMoveSpeed} | Spawn: {pipeSpawnRate}s | Salto: {flapStrength} | Enemigos: {enemySpawnChance}%";
    }
}
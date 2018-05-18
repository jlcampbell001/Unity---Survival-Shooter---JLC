using UnityEngine;

[System.Serializable]
public class EnemyToSpawn
{
    public GameObject enemy;           // The enemy prefac to be spawned.
    public int chanceOfSpawn = 100;    // The chance the enemy will spawn.  Range 1 - 100.  If less then 1 it will not spawn ever.
}

public class EnemyManager : MonoBehaviour
{
    public PlayerHealth playerHealth;       // Reference to the player's heatlh.
    //public GameObject enemy;                // The enemy prefab to be spawned.
    public EnemyToSpawn[] enemies;
    public float spawnTime = 3f;            // How long between each spawn.
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

    void Start()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }


    void Spawn()
    {
        // If the player has no health left...
        if (playerHealth.currentHealth < 0f)
        {
            // ... exit the function.
            return;
        }

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject enemy = null;

        // Pick a enemy to spawn.
        while (enemy == null)
        {
            // Pick a random enemy to try and spawn.
            EnemyToSpawn possibleEnemy = enemies[Random.Range(0, enemies.Length)];

            // Check and see if the enemy will be spawned.
            if (possibleEnemy.chanceOfSpawn > 0)
            {
                if (Random.Range(1, 100) < possibleEnemy.chanceOfSpawn)
                {
                    enemy = possibleEnemy.enemy;
                }
            }
        }

        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
}
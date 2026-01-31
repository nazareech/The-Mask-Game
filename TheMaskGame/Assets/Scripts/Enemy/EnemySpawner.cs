using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Налаштування Спавна")]
    public int enemiesToSpawn = 50;
    public float spawnRadius = 50f;
    private EnemyPooler pooler;

    void Start()
    {
        pooler = FindFirstObjectByType<EnemyPooler>();

        if (pooler == null)
        {
            Debug.LogError("EnemyPooler не знайдено!");
            return;
        }

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        int successfullySpawned = 0;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = pooler.GetPooledEnemy();

            if (enemy != null)
            {
                Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;
                randomPosition.y = transform.position.y; // Тримаємо на одній висоті

                enemy.transform.position = randomPosition;
                enemy.transform.rotation = Quaternion.identity;

                // Робимо ворога незалежним від пулу перед спавном (опціонально, залежить від вашої архітектури)
                enemy.transform.SetParent(null);

                enemy.SetActive(true); // Вмикаємо об'єкт (раніше тут був NetworkServer.Spawn)

                successfullySpawned++;
            }
            else
            {
                Debug.LogWarning("Пул ворогів вичерпано!");
                break;
            }
        }
        Debug.Log($"Заспавнено {successfullySpawned} ворогів локально.");
    }
}
using UnityEngine;
using System.Collections.Generic;

public class EnemyPooler : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int poolSize = 100;

    private List<GameObject> pooledEnemies = new List<GameObject>();
    private bool isInitialized = false;

    void Start()
    {
        if (!isInitialized) InitializePool();
    }

    public void InitializePool()
    {
        if (isInitialized) return;

        pooledEnemies = new List<GameObject>();
        GameObject temp;

        for (int i = 0; i < poolSize; i++)
        {
            temp = Instantiate(enemyPrefab);
            temp.transform.SetParent(this.transform); // Тримаємо їх "дітьми" пулу для чистоти в ієрархії
            temp.SetActive(false);
            pooledEnemies.Add(temp);
        }

        isInitialized = true;
        Debug.Log($"Пул ініціалізовано. {poolSize} ворогів.");
    }

    public GameObject GetPooledEnemy()
    {
        // Якщо хтось просить ворога раніше, ніж спрацював Start
        if (!isInitialized || pooledEnemies == null)
        {
            InitializePool();
        }

        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            // Перевіряємо, чи об'єкт не знищений і чи він вимкнений
            if (pooledEnemies[i] != null && !pooledEnemies[i].activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }

        // Опціонально: можна додати розширення пулу, якщо не вистачило ворогів
        return null;
    }
}
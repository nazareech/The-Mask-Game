using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; // Масив точок спавна
    [SerializeField] private GameObject[] enemyPrefabs; // Масив префабів ворогів (різні типи)
    [SerializeField] private float spawnInterval = 3f;

    [Header("Targets")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform totemTransform; // Сюди перетягнеш Тотем в інспекторі

    [Header("Dependencies")]
    [SerializeField] private RadialMenu radialMenu; // Посилання на меню з ресурсами

    private bool isSpawning = false;

    private void Start()
    {
        // Запускаємо корутину спавна
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Перевіряємо умови спавна або ліміти кількості ворогів тут
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefabs.Length == 0) return;

        // 1. Вибираємо випадкову позицію
        int randomPointIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomPointIndex];

        // 2. Вибираємо випадкового ворога (різні типи)
        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedPrefab = enemyPrefabs[randomEnemyIndex];

        // 3. Створюємо ворога
        GameObject newEnemy = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);

        // 4. Визначаємо ціль (Гравець чи Тотем?)
        Transform currentTarget = playerTransform; // За замовчуванням - гравець

        // Перевірка умови з твого скрипта
        if (radialMenu != null && radialMenu.AllMaskIsUnlocked())
        {
            if (totemTransform != null)
            {
                currentTarget = totemTransform; // Якщо маски зібрані - атакуємо Тотем
            }
        }

        // 5. Передаємо ціль ворогу
        // Припускаємо, що на ворогу висить скрипт EnemyAI (змініть назву на свій скрипт)
        var enemyScript = newEnemy.GetComponent<EnemyAI>();
        if (enemyScript != null)
        {
            enemyScript.SetTarget(currentTarget);
        }
    }
}
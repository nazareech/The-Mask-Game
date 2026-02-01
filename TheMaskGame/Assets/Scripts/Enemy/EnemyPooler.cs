using UnityEngine;
using System.Collections.Generic;

public class EnemyPooler : MonoBehaviour
{
    // Клас для налаштування пулу в інспекторі
    [System.Serializable]
    public class Pool
    {
        public string tag;           // Унікальна назва (напр. "WeakEnemy", "StrongEnemy")
        public GameObject prefab;    // Префаб
        public int size;             // Кількість
    }

    [Header("Налаштування пулів")]
    public List<Pool> pools; // Список пулів, який ви наповните в Інспекторі

    public Dictionary<string, Queue<GameObject>> poolDictionary; // Словник для швидкого доступу

    private void Start()
    {
        InitializePools();
    }

    public void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.SetParent(this.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // Метод тепер приймає "tag", щоб знати, кого саме діставати
    public GameObject GetPooledEnemy(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Пул з тегом {tag} не існує!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // Повертаємо об'єкт в чергу (щоб використовувати по колу), 
        // але перед цим перевіряємо, чи він не активний (якщо пул малий, це може статися)
        // Для простоти просто додаємо його в кінець черги:
        poolDictionary[tag].Enqueue(objectToSpawn);

        // Якщо об'єкт вже активний, краще створити новий або повернути null, 
        // але в простій версії ми просто повертаємо його і сподіваємось, що пул достатньо великий.
        if (objectToSpawn.activeInHierarchy)
        {
            // Опціонально: розширити пул тут, якщо не вистачає
            return null;
        }

        return objectToSpawn;
    }
}
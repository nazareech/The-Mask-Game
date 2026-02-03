using UnityEngine;
using System.Collections.Generic;

public class BananaPool : MonoBehaviour
{
    public static BananaPool Instance;

    [Header("Налаштування")]
    public GameObject bananaPrefab; // Сюди перетягніть префаб банана з BananaScript
    public int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewBanana(false);
        }
    }

    private GameObject CreateNewBanana(bool startActive)
    {
        GameObject banana = Instantiate(bananaPrefab);
        banana.SetActive(startActive);
        banana.transform.SetParent(transform); // Для порядку в ієрархії
        if (!startActive) pool.Enqueue(banana);
        return banana;
    }

    public GameObject GetBanana(Vector3 position, Quaternion rotation)
    {
        GameObject banana;

        if (pool.Count > 0)
        {
            banana = pool.Dequeue();
        }
        else
        {
            // Якщо пул пустий, створюємо новий об'єкт
            banana = CreateNewBanana(false);
        }

        banana.transform.position = position;
        banana.transform.rotation = rotation;
        banana.SetActive(true);

        return banana;
    }

    public void ReturnBanana(GameObject banana)
    {
        banana.SetActive(false);
        pool.Enqueue(banana);
    }
}
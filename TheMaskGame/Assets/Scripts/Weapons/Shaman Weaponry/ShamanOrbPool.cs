using UnityEngine;
using System.Collections.Generic;

public class ShamanOrbPool : MonoBehaviour
{
    public static ShamanOrbPool Instance;

    [Header("Налаштування")]
    public GameObject orbPrefab; // Сюди префаб кулі з ShamanOrbScript
    public int poolSize = 10;

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
            CreateNewOrb(false);
        }
    }

    private GameObject CreateNewOrb(bool startActive)
    {
        GameObject orb = Instantiate(orbPrefab);
        orb.SetActive(startActive);
        orb.transform.SetParent(transform);
        if (!startActive) pool.Enqueue(orb);
        return orb;
    }

    public GameObject GetOrb(Vector3 position, Quaternion rotation)
    {
        GameObject orb;
        if (pool.Count > 0) orb = pool.Dequeue();
        else orb = CreateNewOrb(false);

        orb.transform.position = position;
        orb.transform.rotation = rotation;
        orb.SetActive(true);

        return orb;
    }

    public void ReturnOrb(GameObject orb)
    {
        orb.SetActive(false);
        pool.Enqueue(orb);
    }
}
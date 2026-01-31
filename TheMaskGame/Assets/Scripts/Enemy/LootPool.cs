using UnityEngine;
using System.Collections.Generic;

public class LootPool : MonoBehaviour
{
    public static LootPool Instance; // Сінглтон

    [Header("Settings")]
    public GameObject lootPrefab;
    public int poolSize = 20;

    [Header("Audio Effects")]
    public AudioClip LootDropSound;
    public AudioClip LootPicupSound;
    public AudioSource audioSource;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    // Замість OnStartServer використовуємо Start
    void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewLootObject();
        }
    }

    private GameObject CreateNewLootObject()
    {
        GameObject loot = Instantiate(lootPrefab);
        loot.SetActive(false);
        // Можна додати parent, щоб не засмічувати ієрархію
        loot.transform.SetParent(transform);
        pool.Enqueue(loot);
        return loot;
    }

    public GameObject GetLoot(Vector3 position, Quaternion rotation)
    {
        GameObject loot;

        PlaySound(false); // Викликаємо звук напряму, без RPC

        if (pool.Count == 0)
        {
            loot = CreateNewLootObject(); // Використовуємо той самий метод створення
        }
        else
        {
            loot = pool.Dequeue();
        }

        loot.transform.position = position;
        loot.transform.rotation = rotation;

        loot.SetActive(true); // Просто вмикаємо об'єкт

        return loot;
    }

    private void PlaySound(bool isReturn)
    {
        if (audioSource != null && LootDropSound != null && LootPicupSound != null) 
        {
            if (isReturn)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                float randomVolume = Random.Range(0.8f, 1.0f);
                audioSource.PlayOneShot(LootDropSound, randomVolume);
            }
            else
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                float randomVolume = Random.Range(0.8f, 1.0f);
                audioSource.PlayOneShot(LootPicupSound, randomVolume);

            }
        }
    }

    public void ReturnLoot(GameObject loot)
    {
        PlaySound(true);

        loot.SetActive(false); // Просто ховаємо
        loot.transform.SetParent(transform); // Повертаємо під батьківський об'єкт (для порядку)
        pool.Enqueue(loot);
    }
}
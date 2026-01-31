using UnityEngine;

public class LootPickup : MonoBehaviour
{
    private float coinsAmount; // Звичайна змінна замість SyncVar

    void OnTriggerEnter(Collider other)
    {
        // Перевірка тегу
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                
                player.AddCoins(coinsAmount);
                // Повертаємо об'єкт у пул
                if (LootPool.Instance != null)
                {
                    LootPool.Instance.ReturnLoot(gameObject);
                }
                else
                {
                    Destroy(gameObject); // Якщо пулу немає, просто знищуємо
                }
            }
        }
    }

    public void SetValue(float amount)
    {
        coinsAmount = amount;
    }
}
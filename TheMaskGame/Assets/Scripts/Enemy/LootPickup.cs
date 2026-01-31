using UnityEngine;

public class LootPickup : MonoBehaviour
{
    private float coinsAmount; // «вичайна зм≥нна зам≥сть SyncVar

    void OnTriggerEnter(Collider other)
    {
        // ѕерев≥рка тегу
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                //player.AddCoins(coinsAmount);
                // ѕовертаЇмо об'Їкт у пул
                if (LootPool.Instance != null)
                {
                    LootPool.Instance.ReturnLoot(gameObject);
                }
                else
                {
                    Destroy(gameObject); // якщо пулу немаЇ, просто знищуЇмо
                }
            }
        }
    }

    public void SetValue(float amount)
    {
        coinsAmount = amount;
    }
}
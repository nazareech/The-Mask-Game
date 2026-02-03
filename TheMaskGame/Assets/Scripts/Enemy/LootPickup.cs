using UnityEngine;

public class LootPickup : MonoBehaviour
{
    // Робимо змінну публічною або додаємо геттер, щоб PlayerController міг її прочитати
    public float coinsAmount;

    public void SetValue(float amount)
    {
        coinsAmount = amount;
    }

    // Метод для "збору" монети (щоб гарно видаляти її з пулу)
    public void Collect()
    {
        if (LootPool.Instance != null)
        {
            LootPool.Instance.ReturnLoot(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
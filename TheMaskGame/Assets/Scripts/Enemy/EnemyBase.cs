using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    [Header("Damage Inflicted")]
    private float damageInflicted = 10f;

    [Header("Reward Settings")]
    [SerializeField] private float totalReward = 100f; // Загальна нагорода
    [Range(0, 1)]
    [SerializeField] private float directDepositPercent = 0.3f; // 30% на рахунок, 70% на землю

    // SyncVar не потрібен в одиночній грі, просто приватна змінна
    private float currentHealth;

    // OnEnable ідеально підходить для об'єктів з пулу.
    // Спрацьовує щоразу, коли ми робимо SetActive(true)
    void OnEnable()
    {
        currentHealth = maxHealth;
    }

    // Прибираємо [Server], тепер це звичайний публічний метод
    public void TakeDamage(float damageAmount, GameObject attacker)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die(attacker);
        }
    }

    // Логіка смерті
    public void Die(GameObject attacker)
    {
        float directMoney = totalReward * directDepositPercent; // Гроші на рахунок
        float lootMoney = totalReward - directMoney; // Гроші, що випадають

        // Нарахування грошей гравцю напряму
        if (attacker != null)
        {
            // Переконайтеся, що на гравці або на кулі є посилання на PlayerController
            PlayerController player = attacker.GetComponent<PlayerController>();

            // Іноді "attacker" може бути кулею, тоді треба шукати власника (залежить від вашої реалізації стрільби)
            if (player == null && attacker.CompareTag("Player"))
            {
                player = attacker.GetComponent<PlayerController>();
            }

            if (player != null)
            {
                player.AddCoins(directMoney);
            }
        }

        // Випадання луту
        if (lootMoney > 0)
        {
            if (LootPool.Instance != null)
            {
                // Отримуємо об'єкт з пулу (він вже активний і на позиції завдяки новому скрипту LootPool)
                GameObject lootObj = LootPool.Instance.GetLoot(transform.position + Vector3.up, Quaternion.identity);

                LootPickup lootItem = lootObj.GetComponent<LootPickup>();
                if (lootItem != null)
                {
                    lootItem.SetValue(lootMoney);
                }

                // NetworkServer.Spawn тут більше не потрібен
            }
            else
            {
                Debug.LogWarning("LootPool Instance не знайдено!");
            }
        }

        // Замість NetworkServer.UnSpawn просто ховаємо об'єкт
        gameObject.SetActive(false);

        Debug.Log($"{gameObject.name} помер.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            /*
            ScriptedBullet bullet = other.GetComponent<ScriptedBullet>();
            if (bullet != null)
            {
                // Передаємо власника кулі (bullet.GetOwner()), щоб знати, кому нарахувати гроші
                TakeDamage(bullet.GetDamage(), bullet.GetOwner());

                bullet.ReturnToPool();
            }
            */
        }
    }
    public float GetCurrentEnemyHealth()
    {
        return currentHealth;
    }

    public float GetEnemyDamage()
    {
        return damageInflicted;
    }
}
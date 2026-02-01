using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Movement Settings")] // <--- НОВЕ: Налаштування руху
    [SerializeField] private float moveSpeed = 3.5f; // Швидкість руху

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Combat Settings")]
    [SerializeField] private float damageInflicted = 10f;
    [SerializeField] private float attackInterval = 1f;
    private float nextAttackTime = 0f;

    [Header("Reward Settings")]
    [SerializeField] private float totalReward = 100f;
    [Range(0, 1)]
    [SerializeField] private float directDepositPercent = 0.3f;

    [Header("Components")]
    private NavMeshAgent agent;
    private Transform currentTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = moveSpeed; // <--- НОВЕ: Застосовуємо швидкість при активації
        }

        GetComponent<Collider>().enabled = true;
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        if (agent != null && currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    private void Update()
    {
        if (agent != null && currentTarget != null && !isDead)
        {
            agent.SetDestination(currentTarget.position);
        }
    }

    // --- Логіка Атаки Тотема ---
    private void OnCollisionStay(Collision collision)
    {
        TotemController totem = collision.gameObject.GetComponent<TotemController>();

        if (totem != null)
        {
            if (Time.time >= nextAttackTime)
            {
                totem.TakeDamage(damageInflicted);
                nextAttackTime = Time.time + attackInterval;
            }
        }
    }

    // --- Логіка отримання шкоди ---

    private bool isDead = false;

    public void TakeDamage(float damageAmount, GameObject attacker)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die(attacker);
        }
    }

    public void Die(GameObject attacker)
    {
        if (isDead) return;
        isDead = true;

        if (agent != null) agent.enabled = false;
        GetComponent<Collider>().enabled = false;

        float directMoney = totalReward * directDepositPercent;
        float lootMoney = totalReward - directMoney;

        if (attacker != null)
        {
            PlayerController player = attacker.GetComponent<PlayerController>();
            if (player == null && attacker.CompareTag("Player"))
            {
                player = attacker.GetComponent<PlayerController>();
            }

            if (player != null)
            {
                player.AddCoins(directMoney);
            }
        }

        if (lootMoney > 0 && LootPool.Instance != null)
        {
            GameObject lootObj = LootPool.Instance.GetLoot(transform.position + Vector3.up, Quaternion.identity);
            LootPickup lootItem = lootObj.GetComponent<LootPickup>();
            if (lootItem != null) lootItem.SetValue(lootMoney);
        }

        gameObject.SetActive(false);
        Debug.Log($"{gameObject.name} помер.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Banana"))
        {
            // Логіка банана
        }
    }
}
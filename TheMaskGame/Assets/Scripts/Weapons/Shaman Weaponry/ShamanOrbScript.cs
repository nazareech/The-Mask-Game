using UnityEngine;

public class ShamanOrbScript : MonoBehaviour
{
    private GameObject owner;
    private float speed;
    private float damage;
    private float knockbackForce;

    [Header("Налаштування")]
    public float lifeTime = 4f;

    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    public void Setup(float newSpeed, float newDamage, float newKnockback, GameObject newOwner)
    {
        speed = newSpeed;
        damage = newDamage;
        knockbackForce = newKnockback;
        owner = newOwner;
    }

    void Update()
    {
        // Рух кулі вперед
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // ---------------------------------------------------------
    // УНІВЕРСАЛЬНА СИСТЕМА ВЛУЧАННЯ
    // ---------------------------------------------------------

    // 1. Спрацьовує, якщо куля пролітає крізь тригер
    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    // 2. Спрацьовує, якщо куля врізається в твердий об'єкт
    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    // Спільна логіка удару
    private void HandleHit(GameObject target)
    {
        // Ігноруємо власника (щоб не вбити самого себе при стрільбі)
        if (owner != null && target == owner) return;

        EnemyBase enemy = target.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // 1. Завдаємо шкоди
            enemy.TakeDamage(damage, owner);

            // 2. Відштовхуємо ворога
            Rigidbody enemyRb = target.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Напрямок: Куди летить куля (transform.forward) + трохи вгору
                Vector3 knockbackDir = transform.forward + (Vector3.up * 0.2f);

                // Скидаємо швидкість ворога перед ударом, щоб імпульс був чітким
                if (!enemyRb.isKinematic)
                {
                    enemyRb.linearVelocity = Vector3.zero;
                    enemyRb.AddForce(knockbackDir.normalized * knockbackForce, ForceMode.Impulse);
                }
            }

            // Тут можна додати ефект вибуху (Spawn Particle)
            Debug.Log($"Куля влучила в {target.name} і відштовхнула його!");

            ReturnToPool();
        }
        else
        {
            // Якщо влучили в стіну/землю (і це не тригер-зона, а твердий об'єкт)
            // Перевіряємо, чи це не сам гравець (на всяк випадок)
            if (target != owner && !target.CompareTag("Player"))
            {
                ReturnToPool();
            }
        }
    }

    public void ReturnToPool()
    {
        if (ShamanOrbPool.Instance != null)
        {
            ShamanOrbPool.Instance.ReturnOrb(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class ShamanOrbScript : MonoBehaviour
{
    private GameObject owner;
    private float speed;
    private float damage;
    private float knockbackForce; // Сила відштовхування

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

    // Метод налаштування (додали knockback)
    public void Setup(float newSpeed, float newDamage, float newKnockback, GameObject newOwner)
    {
        speed = newSpeed;
        damage = newDamage;
        knockbackForce = newKnockback;
        owner = newOwner;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (owner != null && other.gameObject == owner) return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            // 1. Завдаємо шкоди
            enemy.TakeDamage(damage, owner);

            // 2. Відштовхуємо ворога
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Відштовхуємо в напрямку польоту кулі
                enemyRb.AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
            }

            // Ефект вибуху можна додати тут (Instantiate particle...)

            ReturnToPool();
        }
        else if (!other.isTrigger)
        {
            ReturnToPool();
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
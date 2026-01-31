using UnityEngine;

public class BananaScript : MonoBehaviour
{
    private GameObject owner; // Хто кинув (щоб не влучити в себе, якщо треба)
    private float speed;
    private float damage;

    [Header("Налаштування")]
    public float lifeTime = 3f; // Скільки летить банан до зникнення

    private void OnEnable()
    {
        // Запускаємо таймер смерті при активації об'єкта
        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    // Метод ініціалізації при кидку
    public void Setup(float newSpeed, float newDamage, GameObject newOwner)
    {
        speed = newSpeed;
        damage = newDamage;
        owner = newOwner;
    }

    void Update()
    {
        // Політ банана вперед
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Опціонально: Можна додати обертання самого мешу банана для краси
        // transform.GetChild(0).Rotate(Vector3.right * 360 * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ігноруємо колайдер самого гравця (якщо це він кинув)
        if (owner != null && other.gameObject == owner) return;

        // Перевіряємо, чи це ворог
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            Debug.Log($"Банан влучив у {enemy.name}!");
            enemy.TakeDamage(damage, owner);
            ReturnToPool();
        }
        // Якщо влучили в стіну (не тригер)
        else if (!other.isTrigger)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        if (BananaPool.Instance != null)
        {
            BananaPool.Instance.ReturnBanana(gameObject);
        }
        else
        {
            Destroy(gameObject); // На випадок якщо пулу не існує
        }
    }
}
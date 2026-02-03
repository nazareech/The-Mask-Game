using UnityEngine;
using UnityEngine.Events;

public class TotemController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Events")]
    public UnityEvent OnTotemDestroyed; // Подія, коли тотем знищено (наприклад, Game Over)

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Запуск анімації отримання урону (якщо є така анімація)
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        Debug.Log("Totem Destroyed!");
        OnTotemDestroyed?.Invoke();

        // Тут можна додати ефекти вибуху або затримку перед знищенням об'єкта
        // Destroy(gameObject, 2f); 
    }
}
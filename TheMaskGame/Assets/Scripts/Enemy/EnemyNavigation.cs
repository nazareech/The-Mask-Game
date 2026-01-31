using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetTransform;
    private PlayerController targetController; // Посилання на контролер гравця

    // Замінюємо NetworkAnimator на звичайний Animator
    public Animator animator;

    [Header("AI Settings")]
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float lookSpeed = 5f;

    [Header("Combat Settings")]
    public float attackInterval = 1.5f;
    private float lastAttackTime;
    public float damageAmount = 10f;

    // Таймер пошуку
    private float searchTimer;
    private float searchInterval = 0.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Якщо аніматор не прив'язаний в інспекторі, пробуємо знайти його автоматично
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
    }

    // Замість [ServerCallback] використовуємо звичайний Update
    void Update()
    {
        // 1. Перевірка на смерть цілі
        if (targetTransform != null && targetController != null)
        {
            /*
            // Переконайтеся, що у PlayerController є метод GetPlayerIsDead() або публічна властивість isDead
            if (targetController.GetPlayerIsDead())
            {
                targetTransform = null;
                targetController = null;
                agent.isStopped = true;

                // Можна скинути анімацію руху
                if (animator) animator.SetFloat("Speed", 0);
            }
            */
        }

        // Періодично шукаємо гравця
        searchTimer -= Time.deltaTime;
        if (searchTimer <= 0)
        {
            FindClosestPlayer();
            searchTimer = searchInterval;
        }

        // Якщо цілі немає — стоїмо
        if (targetTransform == null) return;

        float distance = Vector3.Distance(transform.position, targetTransform.position);

        // Логіка переслідування
        if (distance <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            agent.isStopped = true;
            if (animator) animator.SetFloat("Speed", 0);
        }

        // Логіка атаки
        if (distance <= attackRange)
        {
            AttackPlayer();
        }
    }

    void FindClosestPlayer()
    {
        // В одиночній грі часто достатньо FindGameObjectWithTag (без "s"), 
        // але залишимо логіку пошуку найближчого на випадок, якщо ви додасте союзників.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        float closestDistance = Mathf.Infinity;
        Transform potentialTarget = null;
        PlayerController bestController = null;

        foreach (GameObject player in players)
        {
            PlayerController pc = player.GetComponent<PlayerController>();

            // if (pc == null || pc.GetPlayerIsDead()) continue;

            float d = Vector3.Distance(transform.position, player.transform.position);

            if (d < closestDistance)
            {
                closestDistance = d;
                potentialTarget = player.transform;
                bestController = pc;
            }
        }

        targetTransform = potentialTarget;
        targetController = bestController;
    }

    void ChasePlayer()
    {
        if (targetTransform == null) return;

        agent.isStopped = false;
        agent.SetDestination(targetTransform.position);

        // Передаємо швидкість в аніматор для анімації бігу
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    void AttackPlayer()
    {
        if (targetTransform == null) return;

        agent.isStopped = true;
        if (animator) animator.SetFloat("Speed", 0); // Зупиняємо анімацію бігу

        // Поворот до гравця
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        }

        // Перевірка кулдауну атаки
        if (Time.time >= lastAttackTime + attackInterval)
        {
            // Наносимо урон
            if (targetController != null)
            {
                // targetController.TakeDamage(damageAmount);
                Debug.Log($"Enemy attacked player for {damageAmount} damage");
            }

            lastAttackTime = Time.time;

            // Запускаємо звичайний тригер аніматора
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
}
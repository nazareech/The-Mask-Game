using UnityEngine;
using UnityEngine.AI; // Якщо використовуєш NavMesh

public class EnemyAI : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent agent; // Приклад компонента руху

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Цей метод ми будемо викликати зі спавнера
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (target != null && agent != null)
        {
            // Рухаємось до цілі
            agent.SetDestination(target.position);

            // Тут твоя логіка атаки...
            // Наприклад: float distance = Vector3.Distance(transform.position, target.position);
            // Якщо дистанція мала -> атакувати target.GetComponent<TotemController>()?.TakeDamage(10);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BoarState : PlayerState
{
    private bool isDashing = false;

    public BoarState(PlayerController c) : base(c) { }

    public override void Enter() { Debug.Log("Режим Кабана: Таран!"); }
    public override void Exit() { isDashing = false; }

    public override void Update()
    {
        // Кабан рухається, якщо не в стані ривка (або можна дозволити корегувати рух)
        controller.StandardMovement(controller.boarSpeed);
    }

    public override void Ability()
    {
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && !isDashing)
        {
            controller.StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        controller.PlaySound(controller.boarAttackSound);

        float startTime = Time.time;

        // Ривок триває 0.25 секунди
        while (Time.time < startTime + 0.25f)
        {
            // Рухаємо персонажа вперед з великою силою
            controller.characterController.Move(controller.transform.forward * controller.boarDashForce * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
    }

    // -------------------------------------------------------------
    // ЛОГІКА АТАКИ
    // -------------------------------------------------------------

    // 1. Для м'яких ворогів (Trigger)
    public override void OnTriggerEnter(Collider other)
    {
        TryRamAttack(other.gameObject);
    }

    // 2. Для твердих ворогів (Solid Collider) - ЦЕ ТЕ, ЧОГО НЕ ВИСТАЧАЛО
    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryRamAttack(hit.gameObject);
    }

    // Спільна логіка удару
    private void TryRamAttack(GameObject target)
    {
        // Атакуємо тільки якщо зараз йде ривок
        if (!isDashing) return;

        EnemyBase enemy = target.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // 1. Наносимо урон
            enemy.TakeDamage(controller.boarDashDamage, controller.gameObject);
            Debug.Log($"Кабан протаранив {target.name}!");

            // 2. Фізика відштовхування
            Rigidbody enemyRb = target.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Напрямок удару: вперед від гравця + трохи вгору (щоб ворог підлетів і не терся об землю)
                Vector3 knockbackDir = controller.transform.forward + (Vector3.up * 0.3f);

                // Скидаємо поточну швидкість ворога, щоб удар був відчутнішим
                enemyRb.linearVelocity = Vector3.zero;

                // ForceMode.Impulse ідеально підходить для миттєвих ударів
                enemyRb.AddForce(knockbackDir.normalized * controller.boarKnockbackForce, ForceMode.Impulse);
            }

            // (Опціонально) Якщо вдарилися в твердого ворога, можна зупинити ривок, щоб не пролітати крізь нього
            // isDashing = false; 
        }
    }
}
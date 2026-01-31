using UnityEngine;
using UnityEngine.InputSystem;

public class BirdState : PlayerState
{
    public BirdState(PlayerController c) : base(c) { }

    public override void Enter()
    {
        controller.useGravity = false;
        controller.ResetVelocity();
        // Піднімаємо персонажа вгору один раз при вході (використовуємо налаштування з контролера)
        controller.characterController.Move(Vector3.up * controller.birdHoverHeight);
    }

    public override void Exit()
    {
        controller.useGravity = true;
    }

    public override void Update()
    {
        // Рух з швидкістю птаха
        controller.StandardMovement(controller.birdSpeed);

        // Керування висотою
        var k = Keyboard.current;
        if (k.spaceKey.isPressed) controller.Fly(5f);       // Вгору
        if (k.leftCtrlKey.isPressed) controller.Fly(-5f);   // Вниз
    }

    public override void Ability()
    {
        // У птаха немає активної здібності на кнопку, вона атакує тараном
    }

    // --- ЛОГІКА АТАКИ ---

    // 1. Якщо влетіли в тригер (м'який ворог)
    public override void OnTriggerEnter(Collider other)
    {
        TryPeckAttack(other.gameObject);
    }

    // 2. Якщо врізалися фізично (твердий ворог)
    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryPeckAttack(hit.gameObject);
    }

    private void TryPeckAttack(GameObject target)
    {
        EnemyBase enemy = target.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // Перевіряємо, чи ворог попереду (щоб не бити дупою)
            Vector3 directionToEnemy = (target.transform.position - controller.transform.position).normalized;

            // Якщо кут між поглядом птаха і ворогом малий (ворог спереду)
            if (Vector3.Dot(controller.transform.forward, directionToEnemy) > 0.3f)
            {
                // Завдаємо шкоди
                enemy.TakeDamage(controller.birdDamage, controller.gameObject);

                // Граємо звук
                controller.PlaySound(controller.birdAttackSound);

                Debug.Log("Птах клюнув ворога!");

                // Відштовхування (Knockback)
                Rigidbody enemyRb = target.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    // Вектор удару: вперед + трохи вгору (Vector3.up * 0.5f)
                    Vector3 knockbackDir = controller.transform.forward + (Vector3.up * 0.5f);
                    enemyRb.AddForce(knockbackDir.normalized * controller.birdKnockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}
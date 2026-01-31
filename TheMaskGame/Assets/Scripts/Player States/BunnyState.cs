using UnityEngine;
using UnityEngine.InputSystem;

public class BunnyState : PlayerState
{
    public BunnyState(PlayerController c) : base(c) { }

    public override void Enter()
    {
        controller.jumpHeight = controller.bunnyHighJump;
    }
    public override void Exit()
    {
        controller.jumpHeight = 2f; // Повертаємо стандарт
    }

    public override void Update() { controller.StandardMovement(controller.bunnySpeed); }

    public override void Ability()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.characterController.isGrounded)
        {
            // Формула стрибка
            controller.velocity.y = Mathf.Sqrt(controller.jumpHeight * -2f * controller.gravity);
            // Можна додати звук звичайного стрибка тут
        }
    }

    // Обробка приземлення на ворога
    public override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        CheckStompAttack(hit.gameObject);
    }

    // Також перевіряємо тригери
    public override void OnTriggerEnter(Collider other)
    {
        CheckStompAttack(other.gameObject);
    }

    private void CheckStompAttack(GameObject target)
    {
        // Якщо падаємо вниз
        if (controller.velocity.y < 0)
        {
            EnemyBase enemy = target.GetComponent<EnemyBase>();
            if (enemy != null && controller.transform.position.y > target.transform.position.y)
            {
                enemy.TakeDamage(controller.bunnyStompDamage, controller.gameObject);

                // Звук "шмяк"
                controller.PlaySound(controller.bunnyAttackSound);

                // Відскок
                controller.velocity.y = Mathf.Sqrt(controller.bunnyBounceForce * -2f * controller.gravity);
            }
        }
    }
}
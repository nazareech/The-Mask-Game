using UnityEngine;
using UnityEngine.InputSystem;

public class BunnyState : PlayerState
{
    private float nextJumpTime = 0f; // Таймер перезарядки стрибка
    public BunnyState(PlayerController c) : base(c) { }

    public override void Enter()
    {
        controller.jumpHeight = controller.bunnyHighJump;
        controller.SwitchModel(controller.bunnyModel);
    }
    public override void Exit()
    {
        controller.jumpHeight = 2f; // Повертаємо стандарт
    }

    public override void Update() 
    {
        controller.StandardMovement(controller.bunnySpeed); 
        controller.UpdateAnimationMovement();
    }

    public override void Ability()
    {
        // Логіка стрибка:
        // 1. Натиснуто пробіл
        // 2. Персонаж на землі (isGrounded) - інакше не відштовхнеться
        // 3. Минув час перезарядки (Time.time >= nextJumpTime)
         
        if (Keyboard.current.spaceKey.wasPressedThisFrame
            && controller.isGrounded
            && Time.time >= nextJumpTime)
        {
            // Встановлюємо час наступного дозволеного стрибка
            nextJumpTime = Time.time + controller.bunnyJumpCooldown;

            // Анімація стрибка
            if (controller.currentAnimator != null)
                controller.currentAnimator.SetTrigger("Jump");

            // Формула стрибка
            controller.velocity.y = Mathf.Sqrt(controller.jumpHeight * -2f * controller.gravity);

            // Тут можна додати звичайний звук стрибка (не атаки)
            controller.PlaySound(controller.bunnyjumpSound); 
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
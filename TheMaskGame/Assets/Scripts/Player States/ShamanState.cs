using UnityEngine;
using UnityEngine.InputSystem;

public class ShamanState : PlayerState
{
    public ShamanState(PlayerController c, RadialMenu r) : base(c, r) { }

    public override void Enter()
    {
        // Вмикаємо модель Шамана
        controller.SwitchModel(controller.shamanModel);
    }
    public override void Exit() { }

    public override void Update()
    {
        controller.StandardMovement(controller.shamanSpeed);
        
        // Оновлюємо анімацію бігу
        controller.UpdateAnimationMovement();
    }

    public override void Ability()
    {
        // Перевіряємо натискання кнопки ТА чи настав час для атаки (Time.time >= nextAttackTime)
        if ((Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            && !radialMenu.IsOpen()
            && Time.time >= controller.nextAttackTime)
        {
            // Встановлюємо наступний дозволений час пострілу: поточний час + затримка
            controller.nextAttackTime = Time.time + controller.shamanAttackCooldown;

            Shoot();
        }
    }

    private void Shoot()
    {
        // Відтворюємо звук
        controller.PlaySound(controller.shamanAttackSound);

        // Запуск анімації атаки
        if (controller.currentAnimator != null)
            controller.currentAnimator.SetTrigger("Attack");

        if (ShamanOrbPool.Instance == null) return;

        Vector3 spawnPos = controller.firePoint != null ? controller.firePoint.position : controller.transform.position + controller.transform.forward;
        GameObject orb = ShamanOrbPool.Instance.GetOrb(spawnPos, controller.transform.rotation);

        ShamanOrbScript script = orb.GetComponent<ShamanOrbScript>();
        if (script != null)
        {
            script.Setup(controller.shamanOrbSpeed, controller.shamanOrbDamage, controller.shamanOrbKnockback, controller.gameObject);
        }
    }
}
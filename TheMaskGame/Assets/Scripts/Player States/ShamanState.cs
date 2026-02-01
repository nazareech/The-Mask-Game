using UnityEngine;
using UnityEngine.InputSystem;

public class ShamanState : PlayerState
{
    public ShamanState(PlayerController c, RadialMenu r) : base(c, r) { }

    public override void Enter()
    {
        // Наприклад, вмикаємо візуальні ефекти шамана тут
    }
    public override void Exit() { }

    public override void Update()
    {
        controller.StandardMovement(controller.shamanSpeed);
    }

    public override void Ability()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame && !radialMenu.IsOpen())
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Відтворюємо звук
        controller.PlaySound(controller.shamanAttackSound);

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
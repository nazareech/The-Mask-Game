using UnityEngine;
using UnityEngine.InputSystem;

public class GorillaState : PlayerState
{
    public GorillaState(PlayerController c, RadialMenu r) : base(c, r) { }
    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { controller.StandardMovement(controller.gorillaSpeed); }

    public override void Ability()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame && !radialMenu.IsOpen())
        {
            ThrowBanana();
        }
    }

    private void ThrowBanana()
    {
        controller.PlaySound(controller.gorillaAttackSound); // Звук кидка

        if (BananaPool.Instance == null) return;
        Vector3 spawnPos = controller.firePoint != null ? controller.firePoint.position : controller.transform.position + controller.transform.forward;

        GameObject banana = BananaPool.Instance.GetBanana(spawnPos, controller.transform.rotation);
        BananaScript script = banana.GetComponent<BananaScript>();
        if (script != null)
        {
            script.Setup(controller.bananaSpeed, controller.bananaDamage, controller.gameObject);
        }
    }
}
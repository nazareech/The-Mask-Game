using UnityEngine;
using UnityEngine.InputSystem;

public class GorillaState : PlayerState
{
    public GorillaState(PlayerController c) : base(c) { }
    public override void Enter() { }
    public override void Exit() { }

    public override void Update() { controller.StandardMovement(6f); }
    public override void Ability()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            controller.ThrowBanana();
    }
}

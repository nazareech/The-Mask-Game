using UnityEngine;
using UnityEngine.InputSystem;
public class ShamanState : PlayerState
{
    public ShamanState(PlayerController controller) : base(controller)
    {
    }

    public override void Ability()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
            controller.StaffAttack();
    }

    public override void Enter()
    {
        controller.GetComponent<PlayerRotator>().SetRotationActive(true);
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        controller.StandardMovement(3f); // Повільний рух
    }
}

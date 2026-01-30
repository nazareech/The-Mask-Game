using UnityEngine;
using UnityEngine.InputSystem;

public class BoarState : PlayerState
{
    public BoarState(PlayerController c) : base(c) { }
    public override void Enter() { Debug.Log("Став кабаном: Сила і ривок!"); }
    public override void Exit() { }

    public override void Update() { controller.StandardMovement(5f); } // Повільніший

    public override void Ability()
    {
        // Ривок вперед при натисканні Shift
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame)
            controller.Dash(10f); // Ривок вперед
    }
}

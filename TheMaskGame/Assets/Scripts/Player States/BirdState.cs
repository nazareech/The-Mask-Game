using UnityEngine;
using UnityEngine.InputSystem;

public class BirdState : PlayerState
{
    private float hoverHeight = 2.0f; // На скільки піднятися при зміні стану

    public BirdState(PlayerController c) : base(c) { }
    public override void Enter() { 
        controller.useGravity = false;
        controller.ResetVelocity();
        // 3. Піднімаємо персонажа вгору один раз при вході
        controller.characterController.Move(Vector3.up * hoverHeight);
    }
    public override void Exit() { 
        controller.useGravity = true; 
    }

    public override void Update()
    {
        controller.StandardMovement(20f); // Швидка

        // Додаткове керування висотою (за бажанням)
        var k = Keyboard.current;
        if (k.spaceKey.isPressed) controller.Fly(5f);       // Вгору
        if (k.leftCtrlKey.isPressed) controller.Fly(-5f);   // Вниз
    }
    public override void Ability() { }
}

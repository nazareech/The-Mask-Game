using UnityEngine;

public class BunnyState : PlayerState
{
    public BunnyState(PlayerController c) : base(c) { }
    public override void Enter() { controller.jumpHeight = 5f; } // Високий стрибок
    public override void Exit() { controller.jumpHeight = 2f; }

    public override void Update() { controller.StandardMovement(8f); }
    public override void Ability() { /* Стрибок обробляється у StandardMovement */ }
}

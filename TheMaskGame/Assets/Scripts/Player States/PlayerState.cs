using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController controller;

    protected RadialMenu radialMenu;

    public PlayerState(PlayerController controller)
    {
        this.controller = controller;

    }
    public PlayerState(PlayerController controller, RadialMenu radialMenu)
    {
        this.controller = controller;
        this.radialMenu = radialMenu;

    }

    public abstract void Enter(); // Виконується при переході у стан
    public abstract void Update(); // Логіка руху та вводу
    public abstract void Ability(); // Унікальна здатність
    public abstract void Exit(); // Очищення при виході
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnControllerColliderHit(ControllerColliderHit hit) { }
}

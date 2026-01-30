using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController controller;

    public PlayerState(PlayerController controller)
    {
        this.controller = controller;
    }

    public abstract void Enter(); // Виконується при переході у стан
    public abstract void Update(); // Логіка руху та вводу
    public abstract void Ability(); // Унікальна здатність
    public abstract void Exit(); // Очищення при виході
}

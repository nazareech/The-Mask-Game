
// Типи тварин для ідентифікації
using UnityEngine;

public enum AnimalType
{
    Shanaman,
    Boar,
    Bunny,
    Bird,
    Gorilla
}

[System.Serializable]
public class RadialMenuOption
{
    public string Name;         // Назва (наприклад "Boar")
    public Texture Icon;        // Іконка
    public AnimalType Type;     // Який це режим
    public bool IsUnlocked;     // Чи доступний цей режим зараз
}

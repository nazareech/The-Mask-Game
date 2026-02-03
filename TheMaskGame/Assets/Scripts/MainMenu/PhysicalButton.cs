using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Transform movable; 
    [SerializeField] private Vector3 targetLocalPos = new Vector3(0, -0.05f, 0); 
    
    private bool isAnimating;

    private void OnMouseUpAsButton()
    {
        // Перевірка на "забудькуватість" (щоб не було жовтих помилок)
        if (movable == null)
        {
            Debug.LogError($"Увага! На {gameObject.name} не призначено Movable в інспекторі!");
            onClick.Invoke();
            return;
        }

        if (isAnimating) return;

        isAnimating = true;
        // ТУТ БУЛА ПОМИЛКА: тепер викликаємо OnUp
        movable.DOLocalMove(targetLocalPos, animationDuration / 2).OnComplete(OnUp);
    }

    private void OnUp()
    {
        onClick.Invoke();
        // Повертаємо кнопку назад
        movable.DOLocalMove(Vector3.zero, animationDuration / 2).OnComplete(End);
    }

    private void End()
    {
        isAnimating = false;
    }
}
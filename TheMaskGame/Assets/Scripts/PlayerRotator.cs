using UnityEngine;

public class PlayerRotator : MonoBehaviour
{
    [Header("Налаштування")]
    [Tooltip("Посилання на головну камеру")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("Час згладжування повороту (менше = швидше)")]
    [SerializeField] private float turnSmoothTime = 0.1f;

    // Змінна для зберігання поточної швидкості повороту (потрібна для Mathf.SmoothDampAngle)
    private float turnSmoothVelocity;

    private void Start()
    {
        // Якщо камеру не призначили в інспекторі, пробуємо знайти головну
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        RotatePlayerToCamera();
    }

    private void RotatePlayerToCamera()
    {
        if (cameraTransform == null) return;

        // Отримуємо кут повороту камери по Y (горизонталь)
        float targetAngle = cameraTransform.eulerAngles.y;

        // Плавно змінюємо поточний кут персонажа до кута камери
        // Це робить поворот м'яким, а не миттєвим
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        // Застосовуємо поворот до персонажа (зберігаючи 0 по X і Z, щоб він не падав)
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    // 
    public void SetRotationActive(bool isActive)
    {
        this.enabled = isActive;
    }
}

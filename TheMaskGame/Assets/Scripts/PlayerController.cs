using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [Header("Налаштування")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool useGravity = true;
    private Vector3 velocity;

    [Header("Налаштування миші")]
    public float mouseSensitivity = 100f;
    public GameObject playerCamera; // Сюди перетягніть камеру в інспекторі
    private float xRotation = 0f;
    private Transform cameraTransform;
    private Camera cameraComponent;

    public CharacterController characterController;
    
    private int cameraSelected = 0;
    [SerializeField]
    private GameObject[] cameras;

    void Start()
    {
        SetCamera(cameraSelected); // Встановлюємо початкову камеру

        //// Курсор зникає та фіксується в центрі екрана
        //Cursor.lockState = CursorLockMode.Locked;

        cameraTransform = playerCamera.GetComponent<Transform>();
        cameraComponent = playerCamera.GetComponent<Camera>();

        characterController = GetComponent<CharacterController>();
        SetState(new BoarState(this)); // Початковий стан
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    void Update()
    {

        if (cameraSelected == 1)
        {
            Vector3 targetPoint = GetMousePoint();
            // РАСЧЕТ НАПРАВЛЕНИЯ ОТ ТОРСА (transform.position)
            Vector3 direction = targetPoint - transform.position;
            direction.y = 0; // Игнорируем высоту

            if (direction.sqrMagnitude > 0.01f)
            {
                // Обертаємо Spine (верхню частину)
                HandleMouseLook(direction);
            }
        }
        else
        {
            HandleMouseLook(); // Додаємо метод для огляду
        }

        // 1 - Кабан
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("Current state is: Boar");
            SetState(new BoarState(this));
        }
        // 2 - Кролик
        if (Keyboard.current.digit2Key.wasPressedThisFrame){
            Debug.Log("Current state is: Bunny");
            SetState(new BunnyState(this));
            }
        // 3 - Колібрі
        if (Keyboard.current.digit3Key.wasPressedThisFrame){
            Debug.Log("Current state is: Colibry");
            SetState(new BirdState(this));
        }
        // 4 - Горила
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            Debug.Log("Current state is: Gorilla");
            SetState(new GorillaState(this));
        }

        if(Keyboard.current[Key.T].wasPressedThisFrame)
        {
            cameraSelected = (cameraSelected + 1) % cameras.Length;
            SetCamera(cameraSelected);
        }

        currentState.Update();
        currentState.Ability();
        ApplyGravity();
    }

    private void SetCamera(int currentCameraIndex)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            // Камера вмикається, тільки якщо її індекс збігається з обраним
            cameras[i].SetActive(i == currentCameraIndex);
        }
    }

    //============Top View Camera=====================
    // функция для считування позиції мишки
    Vector3 GetMousePoint()
    {
      
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cameraComponent.ScreenPointToRay(mousePos);
        // Створюємо площину на рівні ніг персонажа
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            return ray.GetPoint(hitDist);
        }
        return transform.position + transform.forward; // Повертаємо точку попереду, щоб не було нульового вектора
    }
    private void HandleMouseLook(Vector3 direction)
    {
        Quaternion diseredRotation = Quaternion.LookRotation(direction);
        // 15f - це швидкість повороту, можна винести в налаштування
        transform.rotation = Quaternion.Slerp(transform.rotation, diseredRotation, Time.deltaTime * 15f);
    }
    //================================================

    //============Third View Camera===================
    private void HandleMouseLook()
    {
        // Отримуємо рух миші з нової системи Input System
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Повертаємо тіло персонажа вліво-вправо
        transform.Rotate(Vector3.up * mouseX);

        // Розраховуємо нахил камери вгору-вниз
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Обмежуємо огляд, щоб не "провернути" голову

        // Призначаємо обертання камері
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    //================================================


    public void SetState(PlayerState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Допоміжні методи для станів
    public void StandardMovement(float speed)
    {
        var k = Keyboard.current;
        Vector3 move = transform.right * (k.dKey.isPressed ? 1 : k.aKey.isPressed ? -1 : 0) +
                       transform.forward * (k.wKey.isPressed ? 1 : k.sKey.isPressed ? -1 : 0);
        characterController.Move(move * speed * Time.deltaTime);

        if (k.spaceKey.wasPressedThisFrame && Physics.Raycast(transform.position, Vector3.down, 1.1f))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void Dash(float force) { characterController.Move(transform.forward * force); }
    public void Fly(float ySpeed) {
        characterController.Move(Vector3.up * ySpeed * Time.deltaTime);
        velocity.y = 0; 
    }

    public void ThrowBanana() { Debug.Log("Кинув банан!"); }

    private void ApplyGravity()
    {
        if (useGravity)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            // Якщо гравітація вимкнена (стан птаха), 
            // ми просто тримаємо vertical velocity на нулі
            velocity.y = 0;
        }
        characterController.Move(velocity * Time.deltaTime);
    }
}
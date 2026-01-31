using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [Header("Налаштування")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool useGravity = true;
    private Vector3 velocity;

    [Header("Бойові Налаштування")] // 
    public float dashDamage = 100f;  // Урон від ривка кабана
    private bool isDashing = false; // Чи відбувається зараз ривок

    [Header("Атака Горили")]
    public Transform firePoint; // Створіть пустий об'єкт перед гравцем, звідки вилітатиме банан
    public float bananaSpeed = 15f;
    public float bananaDamage = 25f;

    [Header("Налаштування миші")]
    public float mouseSensitivity = 100f;
    public GameObject playerCamera; // Сюди перетягніть камеру в інспекторі


    // Посилання на ваше меню (зверніть увагу на назву класу, у вас було RedialMenu)
    [SerializeField]
    RadialMenu menuController;

    public CharacterController characterController;

    void Start()
    {
        HideCursor(); // Ховаємо курсор на початку гри

        characterController = GetComponent<CharacterController>();
        SetState(new ShamanState(this)); // Початковий стан
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    void Update()
    { 
        /*
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
        */


        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            menuController.Toggle();
        }

        currentState.Update();
        currentState.Ability();
        ApplyGravity();
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Звільняє курсор
    }

    // Викликайте це, коли закриваєте меню і повертаєтесь до гри
    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; // Блокує курсор
    }

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

    }

    public void BunnyJump(float jaumpHeight)
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void Dash(float force) 
    {
        // Вмикаємо режим атаки
        if (!isDashing)
        {
            StartCoroutine(DashRoutine(force));
        }
    }
    private IEnumerator DashRoutine(float force)
    {
        isDashing = true; // Вмикаємо "хітбокс"

        // Виконуємо рух
        characterController.Move(transform.forward * force);

        // Чекаємо зовсім трохи, щоб фізика встигла зарахувати зіткнення
        // (0.2 секунди "активного" стану тарана)
        yield return new WaitForSeconds(0.2f);

        isDashing = false; // Вимикаємо "хітбокс"
    }
    public void Fly(float ySpeed) {
        characterController.Move(Vector3.up * ySpeed * Time.deltaTime);
        velocity.y = 0; 
    }

    public void ThrowBanana() 
    {
        if (BananaPool.Instance == null)
        {
            Debug.LogError("BananaPool не знайдено на сцені!");
            return;
        }

        // Визначаємо точку спавну (якщо firePoint не задано, кидаємо з центру гравця + трохи вперед)
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up;
        Quaternion spawnRot = transform.rotation; // Банан летить туди, куди дивиться гравець

        // Дістаємо банан з пулу
        GameObject banana = BananaPool.Instance.GetBanana(spawnPos, spawnRot);

        // Налаштовуємо його параметри
        BananaScript script = banana.GetComponent<BananaScript>();
        if (script != null)
        {
            script.Setup(bananaSpeed, bananaDamage, this.gameObject);
        }
    }

    public void StaffAttack() { Debug.Log("Атакував посохом!"); }

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

    // Вбудований метод Unity, який спрацьовує, коли колайдер входить в тригер
    private void OnTriggerEnter(Collider other)
    {
        // 1. Логіка підбору здібностей (ваша стара)
        AbilityPickup item = other.GetComponent<AbilityPickup>();
        if (item != null)
        {
            if (menuController != null) menuController.UnlockMode(item.typeToUnlock);
            if (item.pickupEffect != null) Instantiate(item.pickupEffect, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }

        // 2. NEW: Логіка атаки кабана (якщо це ворог і ми в ривку)
        if (isDashing)
        {
            // Шукаємо компонент EnemyBase на об'єкті, в який врізалися
            EnemyBase enemy = other.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                Debug.Log($"Врізалися в ворога {enemy.name} під час ривка! Здоров'я ворога: {enemy.GetCurrentEnemyHealth()}");
                enemy.TakeDamage(dashDamage, this.gameObject);

                // Опціонально: Можна додати ефект відштовхування або звуку удару тут
            }
        }
    }

    public void AddCoins(float directMoney)
    {
        Debug.Log($"Додано {directMoney} монет гравцю.");
    }
}
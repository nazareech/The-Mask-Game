using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;
    public CharacterController characterController;
    public AudioSource audioSource;

    [Header("Загальні налаштування")]
    public float mouseSensitivity = 100f;
    public GameObject playerCamera;
    public RadialMenu menuController; // Ваше меню
    private float CoinCount = 0f;

    // --- Фізика ---
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool useGravity = true;
    [HideInInspector] public Vector3 velocity;

    // --- Налаштування Шамана ---
    [Header("Шаман")]
    public float shamanSpeed = 3f;
    public float shamanOrbSpeed = 20f;
    public float shamanOrbDamage = 40f;
    public float shamanOrbKnockback = 10f;
    public AudioClip shamanAttackSound; // Звук!

    // --- Налаштування Кабана ---
    [Header("Кабан")]
    public float boarSpeed = 5f;
    public float boarDashForce = 10f;
    public float boarDashDamage = 100f;
    public float boarKnockbackForce = 50f; // (Сила удару)
    public AudioClip boarAttackSound; // Звук!

    // --- Налаштування Горили ---
    [Header("Горила")]
    public float gorillaSpeed = 6f;
    public Transform firePoint;
    public float bananaSpeed = 15f;
    public float bananaDamage = 25f;
    public AudioClip gorillaAttackSound; // Звук!

    // --- Налаштування Птаха ---
    [Header("Птах")]
    public float birdSpeed = 20f;
    public float birdHoverHeight = 2f;
    public float birdDamage = 20f;
    public float birdKnockbackForce = 15f;
    public AudioClip birdAttackSound; // Звук

    // --- Налаштування Кролика ---
    [Header("Кролик")]
    public float bunnySpeed = 8f;
    public float bunnyHighJump = 5f;
    public float bunnyStompDamage = 50f;
    public float bunnyBounceForce = 8f;
    public AudioClip bunnyAttackSound; // Звук!

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        HideCursor();
        SetState(new ShamanState(this, menuController)); // Початковий стан
    }

    private void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame) menuController.Toggle();

        currentState.Update();
        currentState.Ability();
        ApplyGravity();
    }

    // Зміна стану
    public void SetState(PlayerState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // --- Делегація подій фізики у поточний стан ---
    private void OnTriggerEnter(Collider other)
    {
        // Логіка підбору предметів (загальна для всіх)
        AbilityPickup item = other.GetComponent<AbilityPickup>();
        if (item != null)
        {
            menuController.UnlockMode(item.typeToUnlock);
            Destroy(other.gameObject);
            return;
        }

        // Специфічна логіка стану (атака кабана, птаха тощо)
        currentState.OnTriggerEnter(other);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        currentState.OnControllerColliderHit(hit);
    }

    // --- Допоміжні методи (Movement, Audio) ---
    public void StandardMovement(float speed)
    {
        var k = Keyboard.current;
        Vector3 move = transform.right * (k.dKey.isPressed ? 1 : k.aKey.isPressed ? -1 : 0) +
                       transform.forward * (k.wKey.isPressed ? 1 : k.sKey.isPressed ? -1 : 0);
        characterController.Move(move * speed * Time.deltaTime);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(clip);
        }
    }

    private void ApplyGravity()
    {
        if (useGravity) velocity.y += gravity * Time.deltaTime;
        else velocity.y = 0;
        characterController.Move(velocity * Time.deltaTime);
    }

    // Методи курсору (Hide/Show) залишаємо тут...
    public void ShowCursor() { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }
    public void HideCursor() { Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }

    public void ResetVelocity() { velocity = Vector3.zero; }

    // ---------------------------------------------------------
    // ЕКОНОМІКА
    // ---------------------------------------------------------


    public void AddCoins(float directMoney)
    {
        Debug.Log($"Додано {directMoney} монет гравцю.");
        CoinCount += directMoney;
    }

    public float GetCoinCount()
    {
        return CoinCount;
    }

    // ---------------------------------------------------------
    // ПОЛІТ
    // ---------------------------------------------------------
    public void Fly(float ySpeed)
    {
        // Рухаємо персонажа вгору/вниз
        characterController.Move(Vector3.up * ySpeed * Time.deltaTime);

        // Скидаємо вертикальну швидкість, щоб гравітація не тягнула вниз під час польоту
        velocity.y = 0;
    }
}
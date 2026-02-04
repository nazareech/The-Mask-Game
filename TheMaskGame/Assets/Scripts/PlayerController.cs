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
    public Pause pauseMenuController; // Ваше меню
    private float CoinCount = 0f;
    [HideInInspector]
    public float nextAttackTime = 0f; // Змінна для відліку часу

    // --- Фізика ---
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool useGravity = true;
    [HideInInspector] public Vector3 velocity;

    [Header("Перевірка Землі")]
    public Transform groundCheck;  // Сюди перетягніть об'єкт GroundCheck
    public float groundDistance = 0.4f; // Радіус сфери перевірки
    public LayerMask groundMask;   // Тут виберіть шар "Ground
    public bool isGrounded; // Ця змінна головна перевірка землі

    // --- Налаштування Шамана ---
    [Header("Шаман")]
    public float shamanSpeed = 3f;
    public float shamanOrbSpeed = 20f;
    public float shamanOrbDamage = 40f;
    public float shamanOrbKnockback = 10f;
    public float shamanAttackCooldown = 1.0f; // Час перезарядки (секунди)
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
    public float gorillaAttackCooldown = 1.5f; // Час перезарядки (секунди)
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
    public float bunnyJumpCooldown = 0.8f; // Час між стрибками
    public AudioClip bunnyAttackSound; // Звук Шмяк!
    public AudioClip bunnyjumpSound; // Звук!

    // --- Візуальні Моделі ---
    [Header("Візуальні Моделі (Mesh Objects)")]
    public GameObject shamanModel;
    public GameObject boarModel;
    //public GameObject gorillaModel;
    public GameObject birdModel;
    public GameObject bunnyModel;

    // Поточний активний аніматор
    [HideInInspector] public Animator currentAnimator;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        HideCursor();
        SetState(new ShamanState(this, menuController)); // Початковий стан

    }

    private void Update()
    {
        // 1. ВЛАСНА ПЕРЕВІРКА ЗЕМЛІ
        // Створюємо невидиму сферу на ногах і перевіряємо, чи торкається вона шару Ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 2. СКИДАННЯ ШВИДКОСТІ
        // Якщо ми на землі і швидкість падіння вже велика, скидаємо її до -2
        // (-2 краще ніж 0, бо це "притискає" гравця до підлоги)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Keyboard.current.tabKey.wasPressedThisFrame) menuController.Toggle();


        if (Keyboard.current.escapeKey.wasPressedThisFrame) pauseMenuController.PauseGame();

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

        // 2.НОВА ЛОГІКА: Підбір монет(LootPickup)
        // Ми перевіряємо, чи є на об'єкті скрипт LootPickup
        // (Можна також перевіряти other.CompareTag("Coin"), це буде трохи швидше)
        LootPickup coin = other.GetComponent<LootPickup>();
        if (coin != null)
        {
            // Додаємо монети
            AddCoins(coin.coinsAmount);

            // Викликаємо метод збору на самій монеті (щоб вона сховалася/повернулася в пул)
            coin.Collect();
            return;
        }

        // Специфічна логіка стану (атака кабана, птаха тощо)
        currentState.OnTriggerEnter(other);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        currentState.OnControllerColliderHit(hit);
    }

    // Метод для перемикання моделей
    public void SwitchModel(GameObject activeModel)
    {
        // 1. Вимикаємо всі моделі
        shamanModel.SetActive(false);
        boarModel.SetActive(false);
        //gorillaModel.SetActive(false);
        birdModel.SetActive(false);
        bunnyModel.SetActive(false);

        // 2. Вмикаємо потрібну
        if (activeModel != null)
        {
            activeModel.SetActive(true);
            // 3. Отримуємо аніматор з активної моделі
            currentAnimator = activeModel.GetComponent<Animator>();
        }
    }

    // Метод для передачі швидкості в аніматор (викликатиметься в Update станів)
    public void UpdateAnimationMovement()
    {
        if (currentAnimator != null)
        {
            // Передаємо горизонтальну швидкість (без врахування стрибка/падіння по Y)
            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
            float speed = horizontalVelocity.magnitude;

            // В аніматорі має бути параметр Float з назвою "Speed"
            //currentAnimator.SetFloat("Speed", speed);
            currentAnimator.SetBool("IsMoving", speed > 0);

            // Опціонально: чи ми на землі
            //currentAnimator.SetBool("IsGrounded", characterController.isGrounded);
        }
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
    public void ShowCursor() 
    {
        Cursor.lockState = CursorLockMode.None; // Відпускаємо курсор
        Cursor.visible = true; // Робимо курсор видимим 
    }
    public void HideCursor() 
    {
        Cursor.lockState = CursorLockMode.Locked; // Блокуємо курсор
        Cursor.visible = false; // Робимо курсор не видимим
    }

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
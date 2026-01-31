using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerState currentState;

    [Header("Налаштування")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public bool useGravity = true;
    private Vector3 velocity;

    //-------Шаман-------//
    [Header("Атака Шамана")]
    public float shamanOrbSpeed = 20f;      // Швидкість кулі
    public float shamanOrbDamage = 40f;     // Урон кулі
    public float shamanOrbKnockback = 10f;  // Сила відштовхування

    //-------Кабан-------//
    [Header("Бойові Налаштування")] // 
    public float dashDamage = 100f;  // Урон від ривка кабана
    private bool isDashing = false; // Чи відбувається зараз ривок

    //-------Горила-------//
    [Header("Атака Горили")]
    public Transform firePoint; // Створіть пустий об'єкт перед гравцем, звідки вилітатиме банан
    public float bananaSpeed = 15f;
    public float bananaDamage = 25f;

    //-------Колібрі------//
    [Header("Атака Колібрі")]
    public float birdDamage = 20f;        // Шкода від зіткнення
    public float birdKnockbackForce = 15f; // Сила відкидування

    //-----Кролик-------//
    [Header("Атака Кролика")]
    public float bunnyStompDamage = 50f; // Урон від стрибка на голову
    public float bunnyBounceForce = 8f;  // Сила відскоку після удару
    // ----------------//

    [Header("Налаштування миші")]
    public float mouseSensitivity = 100f;
    public GameObject playerCamera; // Сюди перетягніть камеру в інспекторі


    [Header("Audio Effects")]
    public AudioClip GetDamageSound;
    public AudioClip DialDamagepSound;
    public AudioSource audioSource;

    // Посилання на ваше меню (зверніть увагу на назву класу, у вас було RedialMenu)
    [SerializeField]
    RadialMenu menuController;

    public CharacterController characterController;

    private float CoinCount = 0f;

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

    // Cтрільба Шамана 
    public void ShootShamanOrb()
    {
        if (ShamanOrbPool.Instance == null)
        {
            Debug.LogError("ShamanOrbPool не знайдено на сцені!");
            return;
        }

        // Використовуємо ту саму firePoint, що і для банана, або позицію гравця
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward + Vector3.up;

        // Дістаємо кулю з пулу
        GameObject orb = ShamanOrbPool.Instance.GetOrb(spawnPos, transform.rotation);

        // Налаштовуємо швидкість, урон та відкидування
        ShamanOrbScript script = orb.GetComponent<ShamanOrbScript>();
        if (script != null)
        {
            script.Setup(shamanOrbSpeed, shamanOrbDamage, shamanOrbKnockback, this.gameObject);
        }
    }

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

    private void TryBunnyAttack(GameObject otherGameObject)
    {
        // 1. Перевіряємо чи ми Кролик
        if (currentState is BunnyState)
        {
            // 2. Перевіряємо чи ми падаємо (velocity.y < 0)
            // Це важливо, щоб не бити ворога, коли стрибаємо знизу-вверх крізь нього
            if (velocity.y < 0)
            {
                EnemyBase enemy = otherGameObject.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    // 3. Перевіряємо чи гравець вище за ворога
                    // (Додаємо 0.5f для похибки, щоб спрацювало навіть якщо ми трохи опустились)
                    if (transform.position.y > otherGameObject.transform.position.y)
                    {
                        Debug.Log("Кролик розчавив ворога!");
                        enemy.TakeDamage(bunnyStompDamage, this.gameObject);

                        // 4. Ефект відскоку (як в Маріо)
                        // Підкидаємо гравця трохи вгору
                        velocity.y = Mathf.Sqrt(bunnyBounceForce * -2f * gravity);

                        // Звук удару
                        PlaySound(false);
                    }
                }
            }
        }
    }

    private void TryBirdAttack(GameObject otherGameObject)
    {
        // Перевіряємо, чи ми зараз Птах
        if (currentState is BirdState)
        {
            EnemyBase enemy = otherGameObject.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                // Перевірка: чи ворог перед нами?
                // Беремо вектор від гравця до ворога
                Vector3 directionToEnemy = (otherGameObject.transform.position - transform.position).normalized;

                // Dot Product повертає > 0, якщо ворог попереду, < 0, якщо позаду
                if (Vector3.Dot(transform.forward, directionToEnemy) > 0.3f)
                {
                    Debug.Log("Птах клюнув ворога!");

                    // 1. Завдаємо шкоди
                    enemy.TakeDamage(birdDamage, this.gameObject);

                    // 2. Відкидаємо (Knockback)
                    // Для цього на ворозі має бути Rigidbody або ваша власна система knockback в EnemyBase
                    Rigidbody enemyRb = otherGameObject.GetComponent<Rigidbody>();
                    if (enemyRb != null)
                    {
                        // Відкидаємо в напрямку погляду птаха + трохи вгору
                        Vector3 knockbackDir = transform.forward + Vector3.up * 0.2f;
                        enemyRb.AddForce(knockbackDir.normalized * birdKnockbackForce, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    // Вбудований метод Unity, який спрацьовує, коли колайдер входить в тригер
    private void OnTriggerEnter(Collider other)
    {
        AbilityPickup item = other.GetComponent<AbilityPickup>();
        if (item != null)
        {
            if (menuController != null) menuController.UnlockMode(item.typeToUnlock);
            if (item.pickupEffect != null) Instantiate(item.pickupEffect, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }

        // Логіка атаки кабана
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
        // Логіка Птаха
        TryBirdAttack(other.gameObject);

        // Логіка Кролика
        TryBunnyAttack(other.gameObject);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Логіка Птаха (якщо ворог твердий)
        TryBirdAttack(hit.gameObject);

        // Додаємо перевірку для Кролика
        // Це основний метод для CharacterController при приземленні на щось тверде
        TryBunnyAttack(hit.gameObject);
    }

    private void PlaySound(bool isDealDamage)
    {
        if (audioSource != null && GetDamageSound != null && DialDamagepSound != null)
        {
            if (isDealDamage)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                float randomVolume = Random.Range(0.8f, 1.0f);
                audioSource.PlayOneShot(GetDamageSound, randomVolume);
            }
            else
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                float randomVolume = Random.Range(0.8f, 1.0f);
                audioSource.PlayOneShot(DialDamagepSound, randomVolume);

            }
        }
    }

    public void AddCoins(float directMoney)
    {
        Debug.Log($"Додано {directMoney} монет гравцю.");
        CoinCount += directMoney;
    }

    public float GetCoinCount(){return CoinCount;}
}
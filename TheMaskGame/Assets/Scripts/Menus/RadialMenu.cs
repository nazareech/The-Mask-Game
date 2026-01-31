using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;

public class RadialMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject EntryPrefab;
    [SerializeField] float Radius = 300f;

    [Header("Logic")]
    [SerializeField] PlayerController playerController; // Посилання на гравця

    // Список всіх можливих режимів, налаштовується в Інспекторі
    [SerializeField] List<RadialMenuOption> AllOptions;

    private List<RadialMenuEntry> _activeEntries = new List<RadialMenuEntry>();
    private bool _isOpen = false;


    [SerializeField]
    List<Texture> Icons;
    void Start()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

    }

    public void Toggle()
    {
        if (_isOpen) Close();
        else Open();

        Cursor.lockState = CursorLockMode.None; // Відкриваємо курсор
    }

    public void Open()
    {
        _isOpen = true;

        // Фільтруємо лише розблоковані режими
        List<RadialMenuOption> unlockedOptions = new List<RadialMenuOption>();
        foreach (var option in AllOptions)
        {
            if (option.IsUnlocked) unlockedOptions.Add(option);
        }

        int count = unlockedOptions.Count;

        for (int i = 0; i < count; i++)
        {
            // Створюємо кнопку
            GameObject entryObject = Instantiate(EntryPrefab, transform);
            RadialMenuEntry entry = entryObject.GetComponent<RadialMenuEntry>();

            // Налаштовуємо вигляд
            entry.SetLabel(unlockedOptions[i].Name);
            entry.SetIcon(unlockedOptions[i].Icon);

            // Прив'язуємо дані до кнопки (щоб ми знали, що це за режим при кліку)
            // Ми використовуємо замикання (closure), щоб передати конкретну опцію
            RadialMenuOption currentOption = unlockedOptions[i];
            entry.SetCallback((e) => OnOptionSelected(currentOption));

            _activeEntries.Add(entry);
        }

        Rearrange();
    }

    public void Close()
    {
        _isOpen = false;

        foreach (var entry in _activeEntries)
        {// Перевірка на null
            if (entry == null) continue;

            RectTransform rect = entry.GetComponent<RectTransform>();
            GameObject obj = entry.gameObject;

            // 1. ВАЖЛИВО: Негайно зупиняємо будь-які анімації (hover/scale), що зараз грають
            rect.DOKill();
            obj.transform.DOKill();

            // 2. Запускаємо анімацію закриття
            rect.DOAnchorPos(Vector3.zero, 0.3f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Перевіряємо ще раз перед знищенням
                    if (obj != null)
                    {
                        // Остання перевірка на вбивство твінів
                        DOTween.Kill(obj.transform);
                        DOTween.Kill(rect);
                        Destroy(obj);
                    }
                });
        }
        _activeEntries.Clear();
    }

    void Rearrange()
    {
        float radiansOfSeparation = (2 * Mathf.PI) / _activeEntries.Count;

        for (int i = 0; i < _activeEntries.Count; i++)
        {
            float x = Mathf.Sin(radiansOfSeparation * i) * Radius;
            float y = Mathf.Cos(radiansOfSeparation * i) * Radius;

            RectTransform rect = _activeEntries[i].GetComponent<RectTransform>();

            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad).SetDelay(.05f * i);
            rect.DOAnchorPos(new Vector3(x, y, 0), 0.3f).SetEase(Ease.OutQuad).SetDelay(.05f * i);
        }
    }

    // Головна функція вибору
    void OnOptionSelected(RadialMenuOption option)
    {
        Debug.Log($"Selected mode: {option.Name}");

    
        // Перемикаємо стан гравця
        SwitchPlayerState(option.Type);

        // Закриваємо меню після вибору
        Close();
    }

    // Логіка перемикання станів
    void SwitchPlayerState(AnimalType type)
    {
        if (playerController == null) return;

        switch (type)
        {
            case AnimalType.Boar:
                Debug.Log("Switching to Boar");
                playerController.SetState(new BoarState(playerController));
                break;

            case AnimalType.Bunny:
                Debug.Log("Switching to Bunny");
                playerController.SetState(new BunnyState(playerController));
                break;

            case AnimalType.Bird:
                Debug.Log("Switching to Colibry/Bird");
                playerController.SetState(new BirdState(playerController));
                break;

            case AnimalType.Gorilla:
                Debug.Log("Switching to Gorilla");
                playerController.SetState(new GorillaState(playerController));
                break;

            case AnimalType.Shanaman:
                Debug.Log("Switching to Shanaman");
                playerController.SetState(new ShamanState(playerController));
                break;
        }
    }

    // === ПУБЛІЧНИЙ МЕТОД ДЛЯ РОЗБЛОКУВАННЯ (з трігера або досягнення) ===
    public void UnlockMode(AnimalType typeToUnlock)
    {
        foreach (var option in AllOptions)
        {
            if (option.Type == typeToUnlock)
            {
                if (!option.IsUnlocked)
                {
                    option.IsUnlocked = true;
                    Debug.Log($"UNLOCKED NEW MODE: {option.Name}!");

                    // Тут можна додати ефект/звук отримання нової здібності
                }
                return;
            }
        }
    }
}

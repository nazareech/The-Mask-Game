using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadingScreen;
    public GameObject StartButtonUI;
    public Slider progressBar;

    public void LoadLevelBtn(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        StartButtonUI.SetActive(false);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            // Progress повертає від 0 до 0.9. Останні 0.1 — це активація сцени.
            // Тому нормалізуємо значення до 1 для слайдера.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            progressBar.value = progress;

            yield return null; // Чекаємо наступного кадру
        }
    }
}
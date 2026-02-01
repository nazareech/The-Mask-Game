using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour
{
    [Header("Loading Settings")]
    public GameObject loadingScreen; // Канвас завантаження
    public Slider progressBar;

    // Метод для кнопки "Грати"
    public void LoadGameScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    // Метод для кнопки "Вихід"
    public void ExitGame()
    {
        Debug.Log("Гра закривається...");
        Application.Quit();
    }

    // Метод для кнопки "Налаштування"
    public void OpenSettings()
    {
        Debug.Log("Тут має відкритися вікно налаштувань (UI або нова камера)");
        // Тут можна викликати Canvas.SetActive(true) або переключити на ще одну камеру
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        if(loadingScreen != null) loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if(progressBar != null) progressBar.value = progress;
            yield return null;
        }
    }
}
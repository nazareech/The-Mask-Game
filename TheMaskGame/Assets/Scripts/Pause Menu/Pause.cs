using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject canvasPanel;
    public GameObject menuItems;
    private bool isPaused = false;

    public void PauseGame()
    {
        if (isPaused)
        {
            canvasPanel.SetActive(true);
            menuItems.SetActive(true);
            Time.timeScale = 0f; // Зупиняємо час у грі
            Cursor.lockState = CursorLockMode.None; // Відпускаємо курсор
            Cursor.visible = true; // Робимо курсор видимим
        }
        else
        {
            canvasPanel.SetActive(false);
            menuItems.SetActive(false);
            Time.timeScale = 1f; // Повертаємо час у грі
            Cursor.lockState = CursorLockMode.Locked; // Блокуємо курсор
            Cursor.visible = false; // Робимо курсор не видимим
        }

        isPaused = !isPaused;
    }

}

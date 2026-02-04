using Unity.Cinemachine; // Якщо стара версія, тут буде using Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraPoint[] cameras;
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private AudioSource audioSource;
    public AudioClip switchCameraSound; // Звук!

    private int current;

    private void Start() // Змінив Awake на Start для певності ініціалізації
    {
        foreach (var item in cameras)
        {
            if(item.cam != null) item.cam.Priority = inactivePriority;
        }
        if(cameras.Length > 0) cameras[0].cam.Priority = activePriority;
    }

    public void SetCamera(int id)
    {
        // Відтворюємо звук перемикання камери з випадковою висотою тону
        audioSource.pitch = Random.Range(0.2f, 0.5f);
        audioSource.PlayOneShot(switchCameraSound);

        if (id < 0 || id >= cameras.Length || id == current) return;

        // Вимикаємо стару
        if (cameras[current].cam != null) cameras[current].cam.Priority = inactivePriority;
        cameras[current].onClose.Invoke();

        // Вмикаємо нову
        if (cameras[id].cam != null) cameras[id].cam.Priority = activePriority;
        current = id;
        cameras[id].onOpen.Invoke();


        if (id == 2)
        {
            OnAnimtation(true); // Запускаємо анімацію персонажа на початку
        }
        else
        {
            OnAnimtation(false); // Зупиняємо анімацію персонажа, якщо це не камера 2
        }
    }

    private void Update()
    {
        // Логіка повернення назад на ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cameras[current].previousCam != null)
            {
                for (int i = 0; i < cameras.Length; i++)
                {
                    if (cameras[i].cam == cameras[current].previousCam)
                    {
                        SetCamera(i);
                        return;
                    }
                }
            }
        }
    }

    private void OnAnimtation(bool isActive)
    {
        characterAnimator.SetBool("Dancing", isActive);
    }

    [System.Serializable]
    public struct CameraPoint
    {
        public CinemachineCamera cam; // Для старого Cinemachine змініть на CinemachineVirtualCamera
        public CinemachineCamera previousCam;
        public UnityEvent onOpen;
        public UnityEvent onClose;
    }
}
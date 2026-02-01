using Unity.Cinemachine;
using UnityEngine;
using System;
using UnityEngine.Events;


public class CamerasManager : MonoBehaviour
{
    public GameObject StartButtonUI;

    [SerializeField] private CameraPoint[] cameras;
    [SerializeField] private int activePriority = 1;
    [SerializeField] private int inactivePriority = 0;

    private int current;

    private void Awake()
    {
        foreach (var item in cameras)
        {
            item.cam.Priority = inactivePriority;
        }
        cameras[0].cam.Priority = activePriority;
    }

    public void SetCamera(int id)
    {
        if (id < 0 || id >= cameras.Length || id == current)
        {
            return;
        }

        cameras[current].cam.Priority = inactivePriority;
        cameras[current].onClose.Invoke();

        cameras[id].cam.Priority = activePriority;
        current = id;

        cameras[id].onOpen.Invoke();
    }

    private void Update()
    {
        StartButtonUI.SetActive(cameras[current].cam.name == "CinemachineCamera_3");

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

    [System.Serializable]
    private struct CameraPoint
    {
        public CinemachineCamera cam;
        public CinemachineCamera previousCam;
        public UnityEvent onOpen;
        public UnityEvent onClose;
    }
}

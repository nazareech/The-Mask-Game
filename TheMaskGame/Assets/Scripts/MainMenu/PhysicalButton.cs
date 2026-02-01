using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PhysicalButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private float animationDuration;
    [SerializeField] private Transform movable;
    [SerializeField] private Vector3 targetLocalPos;
    private bool isAnimating;

    private void OnMouseUpAsButton()
    {
        if (isAnimating)
        {
            return;
        }
        isAnimating = true;
        movable.DOLocalMove(targetLocalPos, animationDuration / 2).OnComplete(OnComplete);
    }

    private void OnComplete()
    {
        onClick.Invoke();
        movable.DOLocalMove(Vector3.zero, animationDuration / 2).OnComplete(End);
    }

    private void End()
    {
        isAnimating = false;
    }

}
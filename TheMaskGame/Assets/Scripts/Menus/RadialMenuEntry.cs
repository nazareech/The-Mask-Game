using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class RadialMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void RadialMenuEntryDelegate(RadialMenuEntry entry);

    [SerializeField]
    TextMeshProUGUI Label;

    [SerializeField]
    RawImage Icon;

    [SerializeField]
    RawImage Backer;

    RectTransform Rect;
    RadialMenuEntryDelegate Callback;

    bool _isDestroyed = false; // Прапорець, щоб знати, чи об'єкт ще живий

    private void Start()
    {
        Rect = Icon.GetComponent<RectTransform>();
    }

    public void SetLabel(string label)
    {
        Label.text = label;
    }

    public void SetIcon(Texture icon)
    {
        Icon.texture = icon;
    }

    public void SetBacker(Texture backer)
    {
        Backer.texture = backer;
    }

    public Texture GetIcon()
    {
        return Icon.texture;
    }

    public void SetCallback(RadialMenuEntryDelegate callback)
    {
        Callback = callback;
    }
  
    public void OnPointerClick(PointerEventData eventData)
    {
        Callback?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Перевірка, чи об'єкт ще існує
        if (_isDestroyed || Rect == null) return;

        Rect.DOComplete();
        Rect.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isDestroyed || Rect == null) return;

        Rect.DOComplete();
        Rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }

    // Коли об'єкт знищується, примусово вбиваємо всі його анімації
    private void OnDestroy()
    {
        _isDestroyed = true;
        // Примусово вбиваємо всі анімації, пов'язані з цим об'єктом і його RectTransform
        transform.DOKill();
        if (Rect != null) Rect.DOKill();
    }

}

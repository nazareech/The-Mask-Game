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

    RectTransform Rect;
    RadialMenuEntryDelegate Callback;

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
        Rect.DOComplete();
        Rect.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Rect.DOComplete();
        Rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad);
    }

}

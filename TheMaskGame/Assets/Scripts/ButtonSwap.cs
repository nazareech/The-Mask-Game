using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // Для работы с TextMeshPro

public class ButtonHoverEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText; // Перетащи объект текста сюда в Инспекторе
    
    // Строковые переменные для HEX-кодов
    public string normalHex = "#FFFFFF"; // Белый по умолчанию
    public string hoverHex = "#FF5733";  // Твой оранжевый

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            Color myColor;
            if (ColorUtility.TryParseHtmlString(hoverHex, out myColor))
            {
                buttonText.color = myColor;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonText != null)
        {
            Color myColor;
            if (ColorUtility.TryParseHtmlString(normalHex, out myColor))
            {
                buttonText.color = myColor;
            }
        }
    }
}
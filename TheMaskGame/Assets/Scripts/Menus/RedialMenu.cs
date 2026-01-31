using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;

public class RedialMenu : MonoBehaviour
{
    [SerializeField]
    GameObject EntryPrefab;

    [SerializeField]
    float Radius = 300f;

    public int amountOfMasks = 5;
    List<RadialMenuEntry> Entries;

    [SerializeField]
    RawImage TargetIcon;

    [SerializeField]
    List<Texture> Icons;
    void Start()
    {
        Entries = new List<RadialMenuEntry>();
        //amountOfMasks = Icons.Count;
    }

   void AddEntry(string label, Texture icon, RadialMenuEntry.RadialMenuEntryDelegate pCallback)
   {
        GameObject entryObject = Instantiate(EntryPrefab, transform);

        RadialMenuEntry entry = entryObject.GetComponent<RadialMenuEntry>();
        entry.SetLabel(label);
        entry.SetIcon(icon);
        entry.SetCallback(pCallback);

        Entries.Add(entry);

    }

    public void Open()
    {
        for (int i = 0; i < amountOfMasks; i++)
        {
            AddEntry("Button " + i.ToString(), Icons[i], SetTargetIcon);
        }

        Rearrange();
    }

    public void Close()
    {
        for (int i = 0; i < amountOfMasks; i++)
        {
            RectTransform rect = Entries[i].GetComponent<RectTransform>();
            GameObject entry = Entries[i].gameObject;

            rect.DOAnchorPos(Vector3.zero, 0.3f).SetEase(Ease.OutQuad).onComplete = 
                delegate()
                {
                    Destroy(entry);
                };
        }
        Entries.Clear();
    }

    public void Toggle()
    {
        if (Entries.Count == 0)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    void Rearrange()
    {
        float radiansOfSeparation = (2 * Mathf.PI) / Entries.Count;

        for (int i = 0; i < Entries.Count; i++)
        {
            float x = Mathf.Sin(radiansOfSeparation * i) * Radius;
            float y = Mathf.Cos(radiansOfSeparation * i) * Radius;

            RectTransform rect = Entries[i].GetComponent<RectTransform>();

            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutQuad).SetDelay(.05f * i);
            rect.DOAnchorPos(new Vector3(x, y, 0), 0.3f).SetEase(Ease.OutQuad).SetDelay(.05f * i);
        }
    }

    void SetTargetIcon(RadialMenuEntry pEntry)
    {
        TargetIcon.texture = pEntry.GetIcon();
    }
}

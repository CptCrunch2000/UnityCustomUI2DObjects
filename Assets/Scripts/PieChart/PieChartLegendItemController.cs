using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieChartLegendItemController : MonoBehaviour
{
    private Image i;
    private TMP_Text t;

    private void Awake()
    {
        i = transform.GetChild(0).GetComponent<Image>();
        t = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetItem(Color color, string text, string legendText)
    {
        i.color = color;
        t.text = $"<b><size=1.5em>{text}</size></b>{Environment.NewLine}{legendText}";
    }
}

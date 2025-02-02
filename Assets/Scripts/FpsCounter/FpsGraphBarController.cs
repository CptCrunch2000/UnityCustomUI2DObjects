using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FpsGraphBarController : MonoBehaviour
{
    private Transform goMinBar, goAvgBar, goMaxBar;

    private void Awake()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        goMinBar = allChildren.Where(x => x.gameObject.name == "MinFPS").First();
        goAvgBar = allChildren.Where(x => x.gameObject.name == "AvgFPS").First();
        goMaxBar = allChildren.Where(x => x.gameObject.name == "MaxFPS").First();
    }

    public void SetColorOfBars(Color colorMinFps, Color colorAvgFps, Color colorMaxFps)
    {
        goMinBar.GetComponent<Image>().color = colorMinFps;
        goAvgBar.GetComponent<Image>().color = colorAvgFps;
        goMaxBar.GetComponent<Image>().color = colorMaxFps;
    }

    public void SetBarsWidthAndHeight(float barWidth, Vector3 barHeight)
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, 1);
        goMinBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight.x);
        goAvgBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight.y);
        goMaxBar.GetComponent<RectTransform>().sizeDelta = new Vector2(barWidth, barHeight.z);
    }
}

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounterController : MonoBehaviour
{
    private float timer, refresh, avgFramerate;
    private Text tHeader, tFPS;
    private RectTransform rtHeaderPanel;
    private GameObject goDragButton;
    private bool fixedUpdate;

    private void Awake()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        rtHeaderPanel = allChildren.Where(x => x.gameObject.name == "HeaderPanel").First().GetComponent<RectTransform>();
        tHeader = allChildren.Where(x => x.gameObject.name == "HeaderText").First().GetComponent<Text>();
        tFPS = allChildren.Where(x => x.gameObject.name == "FPSText").First().GetComponent<Text>();
        goDragButton = allChildren.Where(x => x.gameObject.name == "DragButton").First().gameObject;
        fixedUpdate = false;
        refresh = 1; // seconds to refresh 
    }

    private void Update()
    {
        if (!fixedUpdate) Calculation();
    }

    private void FixedUpdate()
    {
        if (fixedUpdate) Calculation();
    }

    private void Calculation()
    {
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;

        if (timer <= 0) avgFramerate = (int)(1f / timelapse);
        tFPS.text = avgFramerate.ToString();
    }

    public void ShowFpsCounter(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetPosition(Vector2 pos)
    {
        gameObject.transform.position = new Vector3(pos.x, Screen.height - pos.y - rtHeaderPanel.sizeDelta.y, 0);
    }

    public void SetFixedUpdate(bool active)
    {
        fixedUpdate = active;
    }

    public void ShowDragButton(bool show)
    {
        goDragButton.SetActive(show);

        if (show)
        {

            rtHeaderPanel.sizeDelta = new Vector2(100, 25);
        }
        else
        {
            rtHeaderPanel.sizeDelta = new Vector2(68, 25);
        }
    }

    public void SetBackgroundColor(Color color)
    {
        rtHeaderPanel.GetComponent<Image>().color = color;
    }

    public void SetFontColor(Color color)
    {
        tHeader.color = color;
        tFPS.color = color;
    }
}

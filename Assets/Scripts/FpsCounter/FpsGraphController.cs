using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class FpsGraphController : MonoBehaviour
{
    private float timeUpdate = 1;
    private int highestPossibleFPS = 600;

    private GameObject[] goChartBars;
    private float barSizeWidth = 5;
    private Color minFpsColor = Color.red, avgFpsColor = Color.yellow, maxFpsColor = Color.green;
    private Transform trGraph;
    private Text tFooterText;
    private int stNumLines;
    private int drawLineCounter = -1;
    private FPSCounter _FPSCounter;
    private UILineRenderer minFpsLineRenderer, avgFpsLineRenderer, maxFpsLineRenderer;
    private List<Vector2> minFpsPoints, avgFpsPoints, maxFpsPoints;
    bool drawBars, drawLines;


    private void Awake()
    {
        Application.targetFrameRate = highestPossibleFPS;

        drawBars = true;
        drawLines = true;

        gameObject.transform.GetChild(0).Find("MinFpsColor").GetComponent<Image>().color = minFpsColor;
        gameObject.transform.GetChild(0).Find("AvgFpsColor").GetComponent<Image>().color = avgFpsColor;
        gameObject.transform.GetChild(0).Find("MaxFpsColor").GetComponent<Image>().color = maxFpsColor;
        trGraph = gameObject.transform.GetChild(1).Find("Graph").GetComponent<Transform>();
        tFooterText = gameObject.transform.GetChild(2).Find("FooterText").GetComponent<Text>();

        minFpsLineRenderer = gameObject.transform.GetChild(1).Find("MinFPSLineRenderer").GetComponent<UILineRenderer>();
        minFpsLineRenderer.color = minFpsColor;
        avgFpsLineRenderer = gameObject.transform.GetChild(1).Find("AvgFPSLineRenderer").GetComponent<UILineRenderer>();
        avgFpsLineRenderer.color = avgFpsColor;
        maxFpsLineRenderer = gameObject.transform.GetChild(1).Find("MaxFPSLineRenderer").GetComponent<UILineRenderer>();
        maxFpsLineRenderer.color = maxFpsColor;

        minFpsPoints = new List<Vector2>();
        avgFpsPoints = new List<Vector2>();
        maxFpsPoints = new List<Vector2>();

        ShowDragButton(true);
        CreateBars();

        _FPSCounter = new FPSCounter();
        _FPSCounter.FpsDataCalculated += FPSCounter_FpsDataCalculated;
    }

    private void Update()
    {
        _FPSCounter.Calculate(timeUpdate);
    }

    private void CreateBars()
    {
        var goFpsGraphBarPrefab = Resources.Load("FpsCounter/HUDFpsGraphBar") as GameObject;
        var hlgGraph = trGraph.GetComponent<HorizontalLayoutGroup>();

        gameObject.transform.GetChild(1).Find("MaxFPSText").GetComponent<Text>().text = highestPossibleFPS.ToString();
        gameObject.transform.GetChild(1).Find("HalfFPSText").GetComponent<Text>().text = Mathf.Round(highestPossibleFPS * 0.5f).ToString();

        stNumLines = (int)((trGraph.GetComponent<RectTransform>().rect.width - hlgGraph.padding.left - hlgGraph.padding.right) / (barSizeWidth + hlgGraph.spacing));
        goChartBars = new GameObject[stNumLines];

        for (int i = 0; i < stNumLines; i++)
        {
            goChartBars[i] = Instantiate(goFpsGraphBarPrefab);
            goChartBars[i].SetActive(false);
            goChartBars[i].name = "Bar_" + i;
            goChartBars[i].transform.SetParent(trGraph.gameObject.transform);
            goChartBars[i].GetComponent<FpsGraphBarController>().SetColorOfBars(minFpsColor, avgFpsColor, maxFpsColor);
        }
    }

    private void DrawBars(FPSData fpsData, int index)
    {
        GameObject obj = goChartBars[index];
        obj.SetActive(true);
        var graphHeight = trGraph.GetComponent<RectTransform>().rect.height;
        var minBarHeight = (graphHeight / highestPossibleFPS) * Mathf.Clamp(fpsData.minFPS, 0, highestPossibleFPS);
        var avgBarHeight = (graphHeight / highestPossibleFPS) * Mathf.Clamp(fpsData.avgFPS, 0, highestPossibleFPS);
        var maxBarHeight = (graphHeight / highestPossibleFPS) * Mathf.Clamp(fpsData.maxFPS, 0, highestPossibleFPS);
        obj.GetComponent<FpsGraphBarController>().SetBarsWidthAndHeight(barSizeWidth, new Vector3(minBarHeight, avgBarHeight, maxBarHeight));
    }

    private void DrawLines(FPSData fpsData, int index)
    {
        var graphHeight = trGraph.GetComponent<RectTransform>().rect.height;
        var minBarHeight = (graphHeight / highestPossibleFPS) * Mathf.Clamp(fpsData.minFPS, 0, highestPossibleFPS);
        var avgBarHeight = (graphHeight / highestPossibleFPS) * Mathf.Clamp(fpsData.avgFPS, 0, highestPossibleFPS);
        var maxBarHeight = (graphHeight / highestPossibleFPS) * Mathf.Clamp(fpsData.maxFPS, 0, highestPossibleFPS);

        minFpsPoints.Add(new Vector2((index * 7) + barSizeWidth / 2, minBarHeight - 2.5f));
        avgFpsPoints.Add(new Vector2((index * 7) + barSizeWidth / 2, avgBarHeight - 2.5f));
        maxFpsPoints.Add(new Vector2((index * 7) + barSizeWidth / 2, maxBarHeight - 2.5f));
        RefreshAllLines();
    }

    private void RefreshAllLines()
    {
        minFpsLineRenderer.Points = minFpsPoints.ToArray();
        avgFpsLineRenderer.Points = avgFpsPoints.ToArray();
        maxFpsLineRenderer.Points = maxFpsPoints.ToArray();
    }

    private class FPSCounter
    {
        private FPSData fpsData;
        List<float> fpsBuffer = new List<float>();
        float fpsB, timeCounter;

        public void Calculate(float timeUpdate)
        {
            int fpsBCount = fpsBuffer.Count;

            if (timeCounter <= timeUpdate)
            {
                timeCounter += Time.deltaTime;
                fpsBuffer.Add(1.0f / Time.deltaTime);
            }
            else
            {
                fpsData.minFPS = 10000;
                fpsData.maxFPS = 0;

                for (int x = 0; x < fpsBCount; x++)
                {
                    fpsB += fpsBuffer[x];
                    if (fpsData.minFPS > Mathf.RoundToInt(fpsBuffer[x])) fpsData.minFPS = Mathf.RoundToInt(fpsBuffer[x]);
                    if (fpsData.maxFPS < Mathf.RoundToInt(fpsBuffer[x])) fpsData.maxFPS = Mathf.RoundToInt(fpsBuffer[x]);
                }

                fpsBuffer.Clear();
                fpsBuffer.Add(1.0f / Time.deltaTime);

                fpsB /= fpsBCount;
                fpsData.avgFPS = Mathf.RoundToInt(fpsB);
                fpsData.buffer = fpsBCount;

                fpsB = timeCounter = 0;
                FpsDataCalculated(fpsData);
            }
        }

        public event Action<FPSData> FpsDataCalculated;
    }

    private void FPSCounter_FpsDataCalculated(FPSData fpsData)
    {
        if (drawLineCounter++ < stNumLines - 1)
        {
            tFooterText.text = fpsData.GetFooterInfo;

            if (drawBars) DrawBars(fpsData, drawLineCounter);
            if (drawLines) DrawLines(fpsData, drawLineCounter);
        }
        else
        {
            foreach (Transform child in trGraph) child.gameObject.SetActive(false);
            drawLineCounter = -1;
            minFpsPoints.Clear();
            avgFpsPoints.Clear();
            maxFpsPoints.Clear();
        }
    }

    private struct FPSData
    {
        public int minFPS;
        public int avgFPS;
        public int maxFPS;
        public int buffer;

        public string GetFooterInfo
        {
            get
            {
                return $"MIN {minFPS} | AVG {avgFPS} | MAX {maxFPS} | BUF {buffer}";
            }
        }
    }

    public void ShowFpsGraph(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ShowDragButton(bool show)
    {
        gameObject.transform.GetChild(0).Find("DragButton").gameObject.SetActive(show);
    }

    public void SetFontColor(Color color)
    {
        gameObject.transform.GetChild(0).Find("MinFpsText").GetComponent<Text>().color = color;
        gameObject.transform.GetChild(0).Find("AvgFpsText").GetComponent<Text>().color = color;
        gameObject.transform.GetChild(0).Find("MaxFpsText").GetComponent<Text>().color = color;
        gameObject.transform.GetChild(1).Find("MaxFPSText").GetComponent<Text>().color = color;
        gameObject.transform.GetChild(1).Find("HalfFPSText").GetComponent<Text>().color = color;
    }

    public void SetHeaderAndFooterBackgroundColor(Color color)
    {
        gameObject.transform.Find("CounterHeader").GetComponent<Image>().color = color;
        gameObject.transform.Find("CounterFooter").GetComponent<Image>().color = color;
    }

    public void SetColorOfBars(Color minFpsColor, Color avgFpsColor, Color maxFpsColor)
    {
        minFpsLineRenderer.color = minFpsColor;
        avgFpsLineRenderer.color = avgFpsColor;
        maxFpsLineRenderer.color = maxFpsColor;

        this.minFpsColor = minFpsColor;
        this.avgFpsColor = avgFpsColor;
        this.maxFpsColor = maxFpsColor;

        gameObject.transform.GetChild(0).Find("MinFpsColor").GetComponent<Image>().color = this.minFpsColor;
        gameObject.transform.GetChild(0).Find("AvgFpsColor").GetComponent<Image>().color = this.avgFpsColor;
        gameObject.transform.GetChild(0).Find("MaxFpsColor").GetComponent<Image>().color = this.maxFpsColor;

        for (int i = 0; i < stNumLines; i++)
        {
            goChartBars[i].GetComponent<FpsGraphBarController>().SetColorOfBars(minFpsColor, avgFpsColor, maxFpsColor);
        }
    }

    public void SetGraphBackgroundColor(Color color)
    {
        trGraph.GetComponent<Image>().color = color;
    }
}

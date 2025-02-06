using UnityEngine;

public class PieChartLegendController : MonoBehaviour
{
    private GameObject pcliPrefab;
    private Transform content;


    private void OnEnable()
    {
        Events.onPieChartPieceAdded.Add(AddPieChartPieceLegendeItem);
        Events.onPieChartPieceRemove.Add(RemovePieChartPieceItem);
  } 

    private void OnDisable()
    {
        Events.onPieChartPieceAdded.Remove(AddPieChartPieceLegendeItem);
        Events.onPieChartPieceRemove.Remove(RemovePieChartPieceItem);
    }

    private void Awake()
    {
        pcliPrefab = Resources.Load("PieChart/PieChartLegendItem") as GameObject;
        if (pcliPrefab == null) { Debug.Log("PieChartLegendItem prefab not found"); }

        content = transform.GetChild(2).GetChild(0).GetChild(0);
    }
    
    public void AddPieChartPieceLegendeItem(PieChartPiece pcp)
    {       
        var c = Instantiate(pcliPrefab, Vector3.zero, Quaternion.identity);
        c.GetComponent<PieChartLegendItemController>().SetItem(pcp.color, pcp.text, pcp.legendText);
        c.transform.SetParent(content);
    }

    public void RemovePieChartPieceItem(int index)
    {
        Destroy(content.GetChild(index).gameObject);
    }
}

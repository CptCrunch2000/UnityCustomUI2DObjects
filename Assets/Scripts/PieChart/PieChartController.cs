using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PieChartController : MonoBehaviour
{
    public float radius = 5f;
    public Color circleBackgroundColor = Color.white;
    public PieChartPiece[] pieChartPieces;

    private GameObject circleBackground;
    private GameObject circle;
    private Material circleMaterial;
    private List<PieChartPiece> pieChartPiecesList = new List<PieChartPiece>();

    private void Awake()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        circleBackground = allChildren.Where(x => x.gameObject.name == "CircleBackground").First().gameObject;

        pieChartPiecesList = pieChartPieces.ToList();

        circle = Resources.Load("PieChart/Circle") as GameObject;
        if (circle == null) { Debug.Log("Circle prefab not found"); }

        circleMaterial = Resources.Load("PieChart/RadialFillMaterial") as Material;
        if (circleMaterial == null) { Debug.Log("RadialFillMaterial not found"); }                
    }

    private void Start()
    {
        CreatePieChartPieces();
    }

    public void SetRadius(float rad)
    {
        radius = rad;
        float previousAngle = 0f;

        circleBackground.transform.localScale = new Vector3(radius*2, radius*2, 1);
        foreach (var p in pieChartPiecesList) 
        {
            p.piece.transform.localScale = new Vector3(radius*2, radius*2, 1);
            TMP_Text t = p.piece.transform.GetChild(0).GetComponent<TMP_Text>();
            Vector3 textPos = new Vector3(
                Mathf.Sin(((p.angle / 2) + previousAngle) * Mathf.Deg2Rad) * radius / 2,
                Mathf.Cos(((p.angle / 2) + previousAngle) * Mathf.Deg2Rad) * radius / 2,
                 -1);

            t.transform.position = textPos;
            previousAngle += p.angle;
        }
    }

    public void AddPieChartPiece()
    {        
        var x = new PieChartPiece
        {
            angle = 30f,
            color = new Color( UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f)), // Color.cyan,
            text = "new item",
            textSize = 6f,
            textRotation = 90f,
            legendText = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr.",
            piece = null
        };
        
        if(CreatePieChartPiece(x))
        {
            pieChartPiecesList.Add(x);
        }        
    }

    public void RemovePieChartPiece(int index)
    {
        if (index > pieChartPiecesList.Count-1) 
        {
            Debug.Log($"Error removing piece at index ({index}. Index out of range!)");
            return;
        }
        
        if (!pieChartPiecesList[index].piece.IsDestroyed())
        {
            Destroy(pieChartPiecesList[index].piece);
            pieChartPiecesList.RemoveAt(index);            
            Events.onPieChartPieceRemove.Publish(index);
            float previousAngle = 0f;

            foreach (var p in pieChartPiecesList) 
            {
                Vector3 textPos = new Vector3(
                Mathf.Sin(((p.angle / 2) + previousAngle) * Mathf.Deg2Rad) * radius / 2,
                Mathf.Cos(((p.angle / 2) + previousAngle) * Mathf.Deg2Rad) * radius / 2,
                -1);

                TMP_Text t = p.piece.transform.GetChild(0).GetComponent<TMP_Text>();
                t.transform.position = textPos;

                Renderer r = p.piece.GetComponent<Renderer>();
                r.sharedMaterial.SetFloat("_Arc1", previousAngle);
                previousAngle += p.angle;
            
                r.sharedMaterial.SetFloat("_Arc2", 360f - previousAngle);                    
                t.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (p.angle/2) + (p.textRotation + 360f - previousAngle)));
            }
        }        
    }

    private void CreatePieChartPieces()
    {
        circleBackground.transform.localScale = new Vector3(radius*2, radius*2, 1);
        circleBackground.GetComponent<SpriteRenderer>().color = circleBackgroundColor;

        foreach (var p in pieChartPiecesList) 
        {
            CreatePieChartPiece(p);
        }
    }

    private bool CreatePieChartPiece(PieChartPiece pcp)
    {
            float previousAngle = pieChartPiecesList.Where(x => x.piece != null).Sum(p => p.angle);
            
            if (previousAngle + pcp.angle > 360f) 
            {
                Debug.Log("Error: Can't create piece. Maximal 360 degrees will be exceeded!");
                return false;
            }

            var c = Instantiate(circle, Vector3.zero, Quaternion.identity);
            c.transform.localScale = new Vector3(radius*2, radius*2, 1);
            TMP_Text t = c.transform.GetChild(0).GetComponent<TMP_Text>();

            t.text = pcp.text;

            Vector3 textPos = new Vector3(
                Mathf.Sin(((pcp.angle / 2) + previousAngle) * Mathf.Deg2Rad) * radius / 2,
                Mathf.Cos(((pcp.angle / 2) + previousAngle) * Mathf.Deg2Rad) * radius / 2,
                -1);

            t.transform.position = textPos;
            t.fontSize = pcp.textSize;

            Renderer r = c.GetComponent<Renderer>();
            r.material = null;
            r.material = new Material(circleMaterial);
            r.sharedMaterial.SetColor("_Color", pcp.color);
            r.sharedMaterial.SetFloat("_Angle", 90);
            r.sharedMaterial.SetFloat("_Arc1", previousAngle);
            previousAngle += pcp.angle;
            
            r.sharedMaterial.SetFloat("_Arc2", 360f - previousAngle);
            t.transform.rotation = Quaternion.Euler(new Vector3(0, 0, (pcp.angle/2) + (pcp.textRotation + 360f - previousAngle)));
            c.transform.SetParent(transform);
            pcp.piece = c;

            Events.onPieChartPieceAdded.Publish(pcp);
            return true;
    }
}

[System.Serializable]
public class PieChartPiece
{
    [SerializeField]
    public float angle;

    [SerializeField]
    public Color color;

    [SerializeField]
    public string text;

    [SerializeField]
    public float textSize;

    [SerializeField]
    public float textRotation;

    [SerializeField]
    public string legendText;

    [NonSerialized]
    public GameObject piece;
}


// int[] degrees = new int[] {0, 90, 180, 270}
// for (int x = 0; x < 4; x++) 
// {
//     GameObject foo = Instantiate(circle, new Vector3(Mathf.Sin(degrees[x]*Mathf.Deg2Rad)*radius, Mathf.Cos(degrees[x]*Mathf.Deg2Rad)*radius, 0), Quaternion.identity);
//     foo.transform.SetParent(transform);
//     foo.GetComponent<SpriteRenderer>().sharedMaterial.SetFloat( "_Arc1", degrees[x]);
// }
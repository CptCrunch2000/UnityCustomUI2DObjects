using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isDragging;
    private Vector2 offset;
    private GameObject goMainPanel;

    public void Awake()
    {
        goMainPanel = gameObject.transform.parent.gameObject.transform.parent.gameObject;
    }

    public void Update()
    {
        if (isDragging)
        {
            goMainPanel.transform.position = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()) - offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        offset = eventData.position - new Vector2(goMainPanel.transform.position.x, goMainPanel.transform.position.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}

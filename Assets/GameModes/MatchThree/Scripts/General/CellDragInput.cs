using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellDragInput : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startDragPos;
    private Action<EnumNeighborDirection> OnDragToDirection; 
    private const float DRAG_THRESHOLD = 40f;

    public void SetOnDrag(Action<EnumNeighborDirection> direction) => OnDragToDirection = direction;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - startDragPos;

        if (delta.magnitude < DRAG_THRESHOLD)
            return;

        Vector2 dir = delta.normalized;

        EnumNeighborDirection target = GetTargetDirection(dir);

        OnDragToDirection?.Invoke(target);
    }

    private EnumNeighborDirection GetTargetDirection(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? EnumNeighborDirection.East : EnumNeighborDirection.West;
        else
            return dir.y > 0 ? EnumNeighborDirection.North : EnumNeighborDirection.South;
    }
}
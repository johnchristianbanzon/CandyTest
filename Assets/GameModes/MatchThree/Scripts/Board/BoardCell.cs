
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
    public bool IsActiveCell = true;
    public int CellIndex;
    public int LineIndex;
    public Dictionary<EnumNeighborDirection,BoardCell> Neighbor = new Dictionary<EnumNeighborDirection,BoardCell>();
    public GemView GemView;
    public PowerupView PowerUpView;
    public bool IsPowerUp = false;
    public List<EnumNeighborDirection> _neighborMatchesDirection = new List<EnumNeighborDirection>();
    public List<EnumNeighborDirection> Neighbors = new List<EnumNeighborDirection>();
    public bool IsMatched;
    [SerializeField]
    private CellDragInput _cellDragInput;

    public GameObject GetGraphic()
    {
        if (IsPowerUp==false)
        {
            return GemView.gameObject;
        }
        return PowerUpView.gameObject;
    }

    public void SetOnDrag(Action<BoardCell,EnumNeighborDirection> OnDragToDirection)
    {
        _cellDragInput.SetOnDrag(OnDirectionDrag);

        void OnDirectionDrag(EnumNeighborDirection direction)
        {
            OnDragToDirection?.Invoke(this,direction);
        }
    }

    
    public void CheckNeighborMatches()
    {
        _neighborMatchesDirection.Clear();
       
        Neighbors = Neighbor.Keys.ToList();
        _neighborMatchesDirection = Neighbor
            .Where(pair => pair.Value.GemView.GetGemData().GemType == GemView.GetGemData().GemType)
            .Select(pair => pair.Key)
            .ToList();
    }

    public List<EnumNeighborDirection> GetNeighborMatches()
    {
        return _neighborMatchesDirection;
    }

    public void Match(float delay=0f, Action OnCompleteMatchAnimation=null)
    {
        var graphic = GetGraphic();
        graphic.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(delegate
        {
            if (IsPowerUp)
            {
                // PowerUpView.Release?.Invoke(GemView);
                
            }
            else
            {
                GemView.Release?.Invoke(GemView);
            }
            OnCompleteMatchAnimation?.Invoke();

        }).SetDelay(delay);

        
        IsMatched = true;
    }

    public void SwapAnimation(Transform targetTrasnform)
    {
        var speed = 50f;

        var graphic = GetGraphic();
        var targetGemViewPos = targetTrasnform.transform.position;
        var targetOwnPos = graphic.transform.position;

        var parent = graphic.transform.parent;

        graphic.transform.SetParent(targetTrasnform.transform.parent);
        targetTrasnform.transform.SetParent(parent);

        graphic.transform.DOMove(targetGemViewPos, speed).SetSpeedBased(true).SetEase(Ease.OutCirc);
        targetTrasnform.transform.DOMove(targetOwnPos, speed).SetSpeedBased(true).SetEase(Ease.OutCirc);
    }
}

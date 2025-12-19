using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardLine 
{
    private List<BoardCell> _boardCells = new List<BoardCell>();
    public int BoardLineIndex = 0;

    public Func<GemView> OnGetGem { get; internal set; }
    public Func<EnumPowerUp,PowerupView> OnGetPowerUp { get; internal set; }
    public BoardLine(int boardLineIndex)
    {
        BoardLineIndex = boardLineIndex;
    }

    public List<BoardCell> GetBoardCells() => _boardCells;

    public void AddBoardCell(BoardCell boardCell)
    {
        _boardCells.Add(boardCell);
    }

    public void Fill()
    {
        for (int i = _boardCells.Count - 1; i >=0; i--)
        {
            var gem = OnGetGem?.Invoke();
            gem.transform.SetParent(_boardCells[i].transform);
            _boardCells[i].GemView = gem;
            _boardCells[i].IsPowerUp = false;

            CellDropAnimation(gem, _boardCells[i], i);
        }

    }

    public void CollapseAndRefill()
    {
 
        var directionCheck = EnumNeighborDirection.North;
        for (int i = _boardCells.Count - 1; i >= 0; i--)
        {
            var currentCell = _boardCells[i];
            var startingCell = _boardCells[i];

            if (currentCell.IsMatched==false)
            {
                continue;
            }


            while (currentCell != null && currentCell.IsMatched)
            {
                if (currentCell.Neighbor.ContainsKey(directionCheck) == false)
                {

                    //FILL

                    var gem = OnGetGem?.Invoke();
                 
                    _boardCells[i].GemView = gem;
                    _boardCells[i].IsPowerUp = false;
                    CellDropAnimation(gem, _boardCells[i],i);
                    break;
                }

                currentCell = currentCell.Neighbor[directionCheck];

                if (currentCell.IsMatched == false)
                {
                    //Collaps3

                    startingCell.GemView = currentCell.GemView;
                    startingCell.PowerUpView = currentCell.PowerUpView;
                    startingCell.IsPowerUp = currentCell.IsPowerUp;
                    currentCell.GetGraphic().transform.SetParent(startingCell.transform);
                    currentCell.IsMatched = true;
                    CellCollpaseAnimation(currentCell, _boardCells[i],i);
                     _boardCells[i].GemView = currentCell.GemView;
                    _boardCells[i].IsPowerUp = currentCell.IsPowerUp;
                    _boardCells[i].PowerUpView = currentCell.PowerUpView;

                    break;
                }
            }

            
        }
    }

    public void SpawnPowerUp(int cellIndex)
    {
        var powerUp = OnGetPowerUp?.Invoke((EnumPowerUp)_boardCells[cellIndex].GemView.GetGemData().GemType);
        _boardCells[cellIndex].PowerUpView = powerUp;
        powerUp.transform.position = _boardCells[cellIndex].transform.position;
        powerUp.transform.localScale = Vector3.zero;
        _boardCells[cellIndex].IsPowerUp = true;
        powerUp.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        powerUp.transform.SetParent(_boardCells[cellIndex].transform);
        _boardCells[cellIndex].IsMatched = false;
    }

    private void CellDropAnimation(GemView graphicObject, BoardCell targetCell, int index)
    {
        graphicObject.transform.SetParent(targetCell.transform);
        graphicObject.GetGraphic().DOFade(0, 0.004f);
        graphicObject.transform.position = _boardCells[0].transform.position + new Vector3(0, 15f);
        var speed = 50f;
        var delay = (_boardCells.Count - 1 - index) * 0.14f;
        var lineDelay = BoardLineIndex * 0.06f;
        graphicObject.transform.DOMoveY(targetCell.transform.position.y, speed).SetSpeedBased(true).SetEase(Ease.OutCirc).SetDelay(delay + lineDelay).OnComplete(delegate
        {
            _boardCells[index].IsMatched = false;
        }).OnStart(delegate
        {
            graphicObject.GetGraphic().DOFade(1, 0.2f);
        });
        graphicObject.transform.localScale = Vector3.one;
    }

    private void CellCollpaseAnimation(BoardCell boardCell, BoardCell targetCell, int index)
    {
        var speed = 50f;
        var delay = (_boardCells.Count - 1 - index) * 0.14f;
        var lineDelay = BoardLineIndex * 0.06f;
        boardCell.GetGraphic().transform.DOMoveY(targetCell.transform.position.y, speed).SetSpeedBased(true).SetEase(Ease.OutCirc).SetDelay(delay + lineDelay).OnComplete(delegate
        {
            targetCell.IsMatched = false;
        });
    }
}
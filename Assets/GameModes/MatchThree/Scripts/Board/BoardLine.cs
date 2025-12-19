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
    public Func<PowerupView> OnGetPowerUp { get; internal set; }
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
            gem.transform.position = _boardCells[0].transform.position + new Vector3(0, 15f);
            var speed = 50f;
            var delay = (_boardCells.Count - 1 - i) * 0.14f;
            var lineDelay = BoardLineIndex * 0.06f; 
            gem.transform.DOMoveY(_boardCells[i].transform.position.y, speed).SetSpeedBased(true).SetEase(Ease.OutCirc).SetDelay(delay + lineDelay);
            gem.transform.localScale = Vector3.one;
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
                    gem.transform.SetParent(_boardCells[i].transform);
                    _boardCells[i].GemView = gem;
                    _boardCells[i].IsPowerUp = false;
                    gem.transform.position = _boardCells[0].transform.position + new Vector3(0, 15f);
                    var speed = 50f;
                    var delay = (_boardCells.Count - 1 - i) * 0.14f;
                    var lineDelay = BoardLineIndex * 0.06f;
                    gem.transform.DOMoveY(_boardCells[i].transform.position.y, speed).SetSpeedBased(true).SetEase(Ease.OutCirc).SetDelay(delay + lineDelay).OnComplete(delegate
                    {
                        startingCell.IsMatched = false;
                    });
                    //currentCell.IsMatched = false;
                    gem.transform.localScale = Vector3.one;
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
                    var speed = 50f;
                    var delay = (_boardCells.Count - 1 - i) * 0.14f;
                    var lineDelay = BoardLineIndex * 0.06f;
                    currentCell.GetGraphic().transform.DOMoveY(_boardCells[i].transform.position.y, speed).SetSpeedBased(true).SetEase(Ease.OutCirc).SetDelay(delay + lineDelay).OnComplete(delegate
                    {
                        startingCell.IsMatched = false;
                    });
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
        var powerUp = OnGetPowerUp?.Invoke();
        _boardCells[cellIndex].PowerUpView = powerUp;
        powerUp.transform.position = _boardCells[cellIndex].transform.position;
        powerUp.transform.localScale = Vector3.zero;
        _boardCells[cellIndex].IsPowerUp = true;
        powerUp.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        powerUp.transform.SetParent(_boardCells[cellIndex].transform);
        _boardCells[cellIndex].IsMatched = false;
    }

    private void CellDropDownAnimation(BoardCell boardCell)
    {

    }
}
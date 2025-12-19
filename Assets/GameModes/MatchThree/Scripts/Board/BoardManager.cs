using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public class BoardManager
{
    private IGemFactory _gemFactory;
    private IPowerupFactory _powerUpFactory;
    private Transform _boardLayout;
    public BoardCell _boardCell;
    private List<BoardLine> _boardLines = new List<BoardLine>();
    private List<MatchGroupCheck> _matchGroupCheck = new List<MatchGroupCheck>();
    private Action<BoardCell, EnumNeighborDirection> _onDragBoardCell;
    private Action _onCheckForMatches;
    private Action<bool> _onAllowDrag;
    private BoardCell _currectBoardCellDragged;

    public BoardManager(IGemFactory gemFactory, [Key(MatchThreeScopeKey.BoardCell)] BoardCell boardCell, IPowerupFactory powerUpFactory)
    {
        _gemFactory = gemFactory;
        _boardCell = boardCell;
        _powerUpFactory = powerUpFactory;
    }

    public void SwapBoardCells(BoardCell boardCell, EnumNeighborDirection direction, Action OnCompleteSwap)
    {
        if (boardCell.Neighbor.ContainsKey(direction)==false)
        {
            return;
        }
     
        var neighborTarget = boardCell.Neighbor[direction];
        _currectBoardCellDragged = neighborTarget;
        var currentGemView = boardCell.GemView;
        var currenPowerUp = boardCell.PowerUpView;
        var isPowerUp = boardCell.IsPowerUp;

        boardCell.SwapAnimation(neighborTarget.GetGraphic().transform);


        boardCell.GemView = neighborTarget.GemView;
        boardCell.PowerUpView = neighborTarget.PowerUpView;
        boardCell.IsPowerUp = neighborTarget.IsPowerUp;

        neighborTarget.GemView = currentGemView;
        neighborTarget.PowerUpView = currenPowerUp;
        neighborTarget.IsPowerUp = isPowerUp;

        if (_currectBoardCellDragged.IsPowerUp)
        {
           ActivatePowerUp(boardCell,_currectBoardCellDragged, _currectBoardCellDragged.PowerUpView.GetPowerupData());
        }
        else
        {
            _onCheckForMatches?.Invoke();
            OnCompleteSwap?.Invoke();
        }
        
    }

    public void ActivatePowerUp(BoardCell fromBoardCell, BoardCell boardcell, PowerupData powerUpData)
    {
        //boardcell.Match();

        var bombSeriesDelay = 0.05f;
        var bombDestroyDelay = 1.2f;
       _onAllowDrag?.Invoke(false);
        for (int i = 0; i < powerUpData.Target.Length; i++)
        {
            var targetHit = powerUpData.Target[i];


            if(_boardLines.Count <= boardcell.LineIndex + targetHit[0] || boardcell.LineIndex + targetHit[0] < 0)
            {
                continue;
            }
            if (_boardLines[boardcell.LineIndex + targetHit[0]].GetBoardCells().Count <= boardcell.CellIndex + targetHit[1] ||(boardcell.CellIndex + targetHit[1]<0))
            {
                continue;
            }

            Debug.Log("MATCHING : (" + (boardcell.LineIndex + targetHit[0]) + "," + (boardcell.CellIndex + targetHit[1]) + ")");
            _boardLines[boardcell.LineIndex + targetHit[0]].GetBoardCells()[boardcell.CellIndex + targetHit[1]].Match(i * bombSeriesDelay);
        }
        boardcell.Match(bombDestroyDelay, OnCompletePowerUpAnimation);

        void OnCompletePowerUpAnimation()
        {
            CollapseAndFillBoard();
            _onCheckForMatches();
        }
    }

    public void SetupBoard(BoardSetting boardSetting)
    {
        for (int i = 0; i < boardSetting.ColumnSize; i++)
        {
            var boardLine = new BoardLine(i);
            _boardLines.Add(boardLine);
            _boardLines[i].OnGetGem = GetGem;
            _boardLines[i].OnGetPowerUp = GetPowerUp;
            for (int j = 0; j < boardSetting.RowSize; j++)
            {
         
                var boardCell = GameObject.Instantiate(_boardCell, _boardLayout);
                boardCell.name = "BOARDLINE : " + i + " / Row :"+j;
                boardCell.CellIndex = j;
                boardCell.LineIndex = i;
                boardLine.AddBoardCell(boardCell);
                boardCell.SetOnDrag(_onDragBoardCell);
            }
        }


        
        for (int i = 0; i < _boardLines.Count; i++)
        {
            var lineCells = _boardLines[i].GetBoardCells();
            for (int j = 0; j < lineCells.Count; j++)
            {
                GetAndAddTileNeighbors(lineCells[j]);
            }
        }
    }



    private void GetAndAddTileNeighbors(BoardCell boardCell)
    {
        var potentialNeighborLocations = new List<Vector2Int>() {
            new Vector2Int(0,-1), // up
            new Vector2Int(0,1),  // down
            new Vector2Int(-1,0), // left
            new Vector2Int(1,0) // right
        };


        for (int i = 0; i < potentialNeighborLocations.Count; i++)
        {
            var linePosition = (boardCell.LineIndex + potentialNeighborLocations[i].x);
            var cellPosition = (boardCell.CellIndex + potentialNeighborLocations[i].y);


            if (_boardLines.Count <= linePosition || linePosition < 0)
            {
                continue;
            }

            if(_boardLines[linePosition].GetBoardCells().Count <= cellPosition || (cellPosition < 0))
            {
                continue;
            }

            var neighbor = _boardLines[linePosition].GetBoardCells()[cellPosition];

            if (neighbor.IsActiveCell)
            {
                boardCell.Neighbor.Add((EnumNeighborDirection)i,neighbor);
             

            }
        }

    }

    public bool CheckBoardMatch()
    {
        var matches = new List<BoardCell>();
        var spawnPowerUp = false;
        var horizontalMatches = new List<BoardCell>();
        var verticalMatches = new List<BoardCell>();
        for (int i = 0; i < _boardLines.Count; i++)
        {
            var lineCells = _boardLines[i].GetBoardCells();
            for (int j = 0; j < lineCells.Count; j++)
            {
                lineCells[j].CheckNeighborMatches();
                if(lineCells[j].GetNeighborMatches().Count > 0)
                {
                    var gemType = lineCells[j].GemView.GetGemData().GemType;

                    horizontalMatches.Clear();
                 

                    if (lineCells[j].Neighbor.ContainsKey(EnumNeighborDirection.West))
                    {
                        GetAllNextCells(lineCells[j], EnumNeighborDirection.West, gemType, horizontalMatches);
                    }

                    if (lineCells[j].Neighbor.ContainsKey(EnumNeighborDirection.East))
                    {
                        GetAllNextCells(lineCells[j], EnumNeighborDirection.East, gemType, horizontalMatches);
                    }
                        

                    if(horizontalMatches.Count >3)
                    {
                        matches.AddRange(horizontalMatches);

                    }

                    verticalMatches.Clear();

                    if (lineCells[j].Neighbor.ContainsKey(EnumNeighborDirection.North))
                    {
                        GetAllNextCells(lineCells[j], EnumNeighborDirection.North, gemType, verticalMatches);
                    }

                    if (lineCells[j].Neighbor.ContainsKey(EnumNeighborDirection.South))
                    {
                        GetAllNextCells(lineCells[j], EnumNeighborDirection.South, gemType, verticalMatches);
                    }

                    if (verticalMatches.Count > 3)
                    {
                        matches.AddRange(verticalMatches);
                    }

                    if (spawnPowerUp == false)
                    {
                        spawnPowerUp = horizontalMatches.Count > 4 || verticalMatches.Count > 4;
                    }
                    
                }
            }
        }


        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].Match();
        }

        if (_currectBoardCellDragged != null && spawnPowerUp)
        {
            _boardLines[_currectBoardCellDragged.LineIndex].SpawnPowerUp(_currectBoardCellDragged.CellIndex);
            _currectBoardCellDragged = null;
        }
   
        if (matches.Count > 0)
        {
            _onCheckForMatches?.Invoke();
            _currectBoardCellDragged = null;
        }

        return matches.Count >0;
    }

    public void CollapseAndFillBoard()
    {
        for (int i = 0; i < _boardLines.Count; i++)
        {
            _boardLines[i].CollapseAndRefill();
        }

      //  _onMatchComplete?.Invoke();
    }

    public void GetAllNextCells(BoardCell startingCell, EnumNeighborDirection nextDirection, EnumGemType GemType, List<BoardCell> Matches)
    {
        var currentCell = startingCell;


        while (currentCell != null && currentCell.GemView.GetGemData().GemType == GemType && currentCell.IsPowerUp==false)
        {
            Matches.Add(currentCell);
            if (currentCell.Neighbor.ContainsKey(nextDirection)==false)
            {
                break;
            }
            currentCell = currentCell.Neighbor[nextDirection];
            if (currentCell.GemView == null)
            {
                break;
            }
        }
    }

    private GemView GetGem()
    {
        var createRandomGem = _gemFactory.CreateRandomGem();

        return createRandomGem;
    }

    private PowerupView GetPowerUp(EnumPowerUp powerUp)
    {
        var createRandomGem = _powerUpFactory.CreatePowerUp(powerUp);

        return createRandomGem;
    }

    public void StartGame()
    {
       
        for (int i = 0; i < _boardLines.Count; i++)
        {
            _boardLines[i].Fill();
        }
      
    }

    public void Initialize(Transform layout, Action<BoardCell, EnumNeighborDirection> onDragBoardCell, Action onCheckForMatches, Action<bool> onAllowDrag)
    {
        _onDragBoardCell = onDragBoardCell;
        _boardLayout = layout;
        _onCheckForMatches = onCheckForMatches;
        _onAllowDrag = onAllowDrag;
    }
}
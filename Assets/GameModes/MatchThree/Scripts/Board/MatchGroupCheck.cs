using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Match check for neighbors
/// </summary>
public class MatchGroupCheck 
{
    private List<BoardCell> cells = new();
    private int _matchLimitSetting=3;
    private EnumGemType _currentGemType;

    public List<BoardCell> GetCells() => cells;

    public void SetMatchLimit(int matchLimit)
    {
        _matchLimitSetting  = matchLimit;
    }

    public void AddMatch(BoardCell boardCell)
    {
        cells.Add(boardCell);
        _currentGemType = boardCell.GemView.GetGemData().GemType;
    }


    public bool CheckMatchSuccess()
    {
        Debug.Log("CURRENT  COUNT :" + cells.Count);

        for (int i = 0; i < cells.Count; i++)
        {
            Debug.Log(cells[i].name);
        }
        if(cells.Count >= _matchLimitSetting)
        {
            return true;
        }
        return false;
    }
}
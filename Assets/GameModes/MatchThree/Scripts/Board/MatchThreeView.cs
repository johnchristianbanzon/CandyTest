
using System;
using System.Collections;
using UnityEngine;
using VContainer;

public class MatchThreeView : BaseView
{
    [Inject]
    private BoardManager _boardManager;
    [SerializeField]
    private Transform _boardLayout;
    private Coroutine _onCheckMatch;
    private bool IsCheckingMatch;

    private void Start()
    {
      //  _onCheckMatch = StartCoroutine(OnCheckMatchCoroutine());
        _boardManager.Initialize(_boardLayout, OnDragBoardCell,OnCheckMatches);
        _boardManager.SetupBoard(new BoardSetting(5,5));
        StartCoroutine(StartGame());
        OnCheckMatches();

    }

    private void OnCheckMatches()
    {
        if (IsCheckingMatch)
        {
            return;
        }
        StartCoroutine(OnCheckMatchCoroutine());
    }

    private void OnDragBoardCell(BoardCell cell, EnumNeighborDirection direction)
    {
        _boardManager.SwapBoardCells(cell, direction, OnCheckMatches);

    }

    private IEnumerator OnCheckMatchCoroutine()
    {
        Debug.Log("CHECKING MATCHES!!");
        IsCheckingMatch = true;
        yield return new WaitForSeconds(2f);

        var hasMatches = _boardManager.CheckBoardMatch();
        IsCheckingMatch = false;
        if (hasMatches)
        {
            yield return new WaitForSeconds(1f);
            _boardManager.CollapseAndFillBoard();
            OnCheckMatches();
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.1f);
        _boardManager.StartGame();

    }


    [Inject]
    public void Construct(IGemFactory gemFactory)
    {
         gemFactory.CreateRandomGem();
    }
}
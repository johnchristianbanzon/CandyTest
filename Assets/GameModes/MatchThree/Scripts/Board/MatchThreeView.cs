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
    public bool AllowSwap=true;

    private void Start()
    {
      //  _onCheckMatch = StartCoroutine(OnCheckMatchCoroutine());
        _boardManager.Initialize(_boardLayout, OnDragBoardCell,OnCheckMatches, OnAllowSwap);
        _boardManager.SetupBoard(new BoardSetting(5,5));
        StartCoroutine(StartGame());
        

    }

    public void OnAllowSwap(bool allowSwap)
    {
        AllowSwap = allowSwap;
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
        if (AllowSwap==false)
        {
            return;
        }
        _boardManager.SwapBoardCells(cell, direction, OnCheckMatches);

    }

    private IEnumerator OnCheckMatchCoroutine()
    {
        IsCheckingMatch = true;
        AllowSwap = false;
        yield return new WaitForSeconds(1.5f);

        var hasMatches = _boardManager.CheckBoardMatch();
      
   
        if (hasMatches)
        {
            yield return new WaitForSeconds(1.5f);
            _boardManager.CollapseAndFillBoard();
          
            yield return new WaitForSeconds(0.5f);
            AllowSwap = true;
            IsCheckingMatch = false;
            OnCheckMatches();
        }
        else
        {
            AllowSwap = true;
            IsCheckingMatch = false;
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(0.1f);
        _boardManager.StartGame();
        yield return new WaitForSeconds(0.5f);
        OnCheckMatches();
    }


    [Inject]
    public void Construct(IGemFactory gemFactory)
    {
         gemFactory.CreateRandomGem();
    }
}
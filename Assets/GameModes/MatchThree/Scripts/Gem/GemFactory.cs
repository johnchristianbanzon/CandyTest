using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class GemFactory : IGemFactory
{
    private GemContainerSO _container;
    [Inject]
    [Key(MatchThreeScopeKey.GemView)]
    private GemView _gemView;
    private ObjectPool<GemView> _gemViewPool;


    [Inject]
    public void Construct(GemContainerSO container)
    {
        Debug.Log("FACTORY : " + container);
        _container = container;
        _gemViewPool = new ObjectPool<GemView>(OnCreateGem, OnGetGem, OnReleaseGem);
    }

    private GemView OnCreateGem()
    {
        var gemCreated = GameObject.Instantiate(_gemView);
        gemCreated.SetGemData(_container.GemData[UnityEngine.Random.Range(0,_container.GemData.Length)]);
        gemCreated.Release = OnReleaseGem;
        return gemCreated;
    }

    private void OnGetGem(GemView gemView)
    {
        gemView.gameObject.SetActive(true);
    }

    private void OnReleaseGem(GemView gemView)
    {
        gemView.gameObject.SetActive(false);
    }

    public GemView CreateRandomGem()
    {
        return _gemViewPool.Get();
    }
}
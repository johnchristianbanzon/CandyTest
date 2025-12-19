using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

public class PowerupFactory : IPowerupFactory
{
    private PowerupContainerSO _container;
    [Inject]
    [Key(MatchThreeScopeKey.PowerUpView)]
    private PowerupView _powerUpView;
    private ObjectPool<PowerupView> _powerUpPool;


    [Inject]
    public void Construct(PowerupContainerSO container)
    {
        Debug.Log("FACTORY : " + container);
        _container = container;
        _powerUpPool = new ObjectPool<PowerupView>(OnCreateGem, OnGetGem, OnReleaseGem);
    }

    private PowerupView OnCreateGem()
    {
        var powerUpCreated = GameObject.Instantiate(_powerUpView);
        return powerUpCreated;
    }

    public PowerupView CreatePowerUp(EnumPowerUp powerUp)
    {
        Debug.Log("CREATING POWER UP!");
        var powerUpCreated = _powerUpPool.Get();
        powerUpCreated.SetPowerUpData(_container.PowerUpData.Where(x=>x.Enum== powerUp).FirstOrDefault());
        return powerUpCreated;
    }

    private void OnGetGem(PowerupView gemView)
    {
        gemView.gameObject.SetActive(true);
    }

    private void OnReleaseGem(PowerupView gemView)
    {
        gemView.gameObject.SetActive(false);
    }

}
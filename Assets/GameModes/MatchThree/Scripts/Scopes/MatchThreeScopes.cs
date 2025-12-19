using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Scopes using VContainer, Resolves all match three necessities 
/// </summary>
public class MatchThreeScopes : LifetimeScope
{
    [SerializeField]
    private GemContainerSO _gemContainer;
    [SerializeField]
    private PowerupContainerSO _powerUpContainer;
    [SerializeField]
    private BoardCell _boardCell;
    [SerializeField]
    private GemView _gemView;
    [SerializeField]
    private PowerupView _powerUpView;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IGemFactory, GemFactory>(Lifetime.Singleton);
        builder.Register<IPowerupFactory,PowerupFactory>(Lifetime.Singleton);
        builder.Register<BoardManager>(Lifetime.Singleton);
        builder.RegisterInstance(_gemContainer);
        builder.RegisterInstance(_powerUpContainer);
        builder.RegisterInstance(_boardCell).Keyed(MatchThreeScopeKey.BoardCell);
        builder.RegisterInstance(_gemView).Keyed(MatchThreeScopeKey.GemView);
        builder.RegisterInstance(_powerUpView).Keyed(MatchThreeScopeKey.PowerUpView);
    }
}

using VContainer;
using VContainer.Unity;
using UnityEngine;

public class ScreenLifetimeScope : LifetimeScope
{
    [SerializeField]
    private ScreenManager screenManager;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<ScreenManager>(screenManager);
    }
}

using VContainer;
using VContainer.Unity;
using UnityEngine;
public class StageClearUILifetimeScope : LifetimeScope
{
    [SerializeField]
    private StageClearManager stageClearManager;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<StageClearManager>(stageClearManager);
    }
}

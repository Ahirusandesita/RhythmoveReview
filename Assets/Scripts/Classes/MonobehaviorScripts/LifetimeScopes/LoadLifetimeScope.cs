using VContainer;
using VContainer.Unity;
using UnityEngine;
public enum LoadType
{
    Straight,
    Curve,
    Goal
}

public class LoadLifetimeScope : LifetimeScope
{
    [SerializeField]
    private LoadType loadType;

    protected override void Configure(IContainerBuilder builder)
    {
        switch (loadType)
        {
            case LoadType.Straight:
                builder.RegisterEntryPoint<Straight>(Lifetime.Singleton).WithParameter(this.transform).Build();
                break;
            case LoadType.Curve:
                builder.RegisterEntryPoint<Curve>(Lifetime.Singleton).WithParameter(this.transform).Build();
                break;
            case LoadType.Goal:
                builder.RegisterEntryPoint<Goal>(Lifetime.Singleton).WithParameter(this.transform).Build();
                break;

        }
    }

}

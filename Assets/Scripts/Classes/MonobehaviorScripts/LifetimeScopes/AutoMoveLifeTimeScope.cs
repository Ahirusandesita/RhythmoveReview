/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using System.Collections;
using VContainer.Unity;
using VContainer;
using System.Collections.Generic;

public class AutoMoveLifeTimeScope : LifetimeScope {

    [SerializeField] AutoMovePresenter autoMovePresenter;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AutoMoveView autoMoveView;

    protected override void Configure(IContainerBuilder builder)
    {
        //builder.R
        builder.Register<MoveDirectionLogic>(Lifetime.Singleton);
        builder.Register<IMovable,IMoveEventHandler, AutoMoveLogic>(Lifetime.Singleton).
            WithParameter(this.transform).WithParameter(MoverType.roleModel);

        builder.RegisterComponent<AutoMovePresenter>(autoMovePresenter);
        builder.RegisterComponent<AutoMoveView>(autoMoveView);
    }

    private void Start()
    {
        Container.Resolve<MoveDirectionLogic>();
    }
    private void OnDisable()
    {
        Destroy(this);
    }
}
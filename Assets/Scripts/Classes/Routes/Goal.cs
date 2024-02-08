/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// ゴール
/// </summary>
[System.Serializable]
public class Goal : Route, IStartable
{

    [Inject]
    public Goal(Transform transform, Field field) : base(transform, field) { }

    void IStartable.Start()
    {
        field.AddGoal(this);
    }

    protected override void CreateMovableDirection() { }
}
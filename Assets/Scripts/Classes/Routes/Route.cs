/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer.Unity;
using VContainer;

/// <summary>
/// キャラが移動する道
/// </summary>
public abstract class Route : IReadOnlyPositionAdapter, IStartable, IRoute
{
    protected Transform transform;
    protected Field field;

    /// <summary>
    /// オブジェクトのPositionを取得
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public MoveType[] MovableDirections => movableDirections;
    protected MoveType[] movableDirections;

    [Inject]
    public Route(Transform transform, Field field)
    {
        this.transform = transform;
        this.field = field;
        CreateMovableDirection();
    }

    /// <summary>
    /// 移動できる方向を定義する
    /// </summary>
    protected abstract void CreateMovableDirection();


    void IStartable.Start()
    {
        field.AddObject(this, this);
    }
}
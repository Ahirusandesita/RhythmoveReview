/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;

/// <summary>
/// Player以外のMoveLogic
/// </summary>
public class AutoMoveLogic : MoveLogic
{
    private Vector3 startPosition;

    [Inject]
    public AutoMoveLogic(Transform transform, Field field, MoverType moverType, Vector3 position,Vector3 startPosition) : base(transform, field, moverType, position)
    {
        OnGoal += Reset;
        this.startPosition = startPosition;
    }

    public override void Dispose()
    {
        transform = null;
        OnGoal -= Reset;
    }

    /// <summary>
    /// ゴール到達で初期位置に戻る
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="goalEventArgs"></param>
    private void Reset(object sender, GoalEventArgs goalEventArgs)
    {
        this.transform.position = startPosition;
    }

    protected override MoveEventArgs EditMoveEventArgs(MoveEventArgs moveEventArgs)
    {
        moveEventArgs.player = false;
        return moveEventArgs;
    }
}
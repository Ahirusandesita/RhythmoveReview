/* 制作日
*　製作者
*　最終更新日
*/
using VContainer;

/// <summary>
/// Playerの移動方向切り替えロジック
/// </summary>
public class MoveDirectionLogicPlayer : MoveDirectionLogic
{

    private MoveType moveType = default;

    [Inject]
    public MoveDirectionLogicPlayer(IMovable movable, IProgressRegisterable progressManagement, INoteRegisteredable noteManager, MoveOrderData moveOrderData) : base(movable, progressManagement, noteManager, moveOrderData)
    {
    }

    public override void MoveHandler(object sender, InputEventArgs inputEventArgs)
    {
        if (isClash)
        {
            return;
        }

        if (moveType == MoveType.nonMove)
        {
            MissMove();
            return;
        }

        if (moveType == lastExecutionMoveType)
        {
            Clash();
            return;
        }
        movable.Move(moveType);
        lastExecutionMoveType = moveType;
    }

    protected override void MoveDirectionChange(object sender, NoteEventArgs noteEventArgs)
    {
        //ノーツ許容範囲　早～遅　で移動方向を切り替える
        if (noteEventArgs.noteTiming == NoteTimingType.just)
        {
            return;
        }
        switch (noteEventArgs.noteTiming)
        {
            case NoteTimingType.early:
                MoveDirectionChange();
                moveType = lastMoveType;
                break;
            case NoteTimingType.late:
                moveType = MoveType.nonMove;
                NonMove();
                break;
        }
    }
}
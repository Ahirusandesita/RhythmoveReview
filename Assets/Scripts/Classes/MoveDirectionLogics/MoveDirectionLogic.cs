/* 制作日 2023/11/28
*　製作者 野村侑平
*　最終更新日 2023/11/28
*/
using VContainer;
using UniRx;
using System.Threading.Tasks;
using System;

/// <summary>
/// 移動方向のロジック
/// </summary>
public class MoveDirectionLogic : IDisposable
{
    /// <summary>
    /// ミスハンドラ
    /// </summary>
    /// <param name="sender">発行者</param>
    /// <param name="e">イベント情報</param>
    public delegate void MistakeEventHandler(object sender, MistakeEventArgs e);
    /// <summary>
    ///  移動のミス時に発行される
    /// </summary>
    public event MistakeEventHandler OnMoveMistake;


    protected MoveOrderData moveOrderData;
    protected IProgressRegisterable progressManagement;
    protected IMovable movable;
    private INoteRegisteredable noteManager;

    //最後の移動方向
    protected MoveType lastMoveType;
    //最後に入力された移動方向
    protected MoveType lastExecutionMoveType = MoveType.nonMove;
    /// <summary>
    /// 現在の移動方向
    /// </summary>
    protected ReactiveProperty<MoveType> moveProperty = new ReactiveProperty<MoveType>();
    public IReadOnlyReactiveProperty<MoveType> readOnlyMovePropery => moveProperty;

    protected bool isClash = false;

    /// <summary>
    /// DI
    /// </summary>
    /// <param name="movable"></param>
    /// <param name="progressManagement"></param>
    /// <param name="noteManager"></param>
    [Inject]
    public MoveDirectionLogic(IMovable movable, IProgressRegisterable progressManagement, INoteRegisteredable noteManager, MoveOrderData moveOrderData)
    {
        this.progressManagement = progressManagement;
        this.movable = movable;
        this.noteManager = noteManager;
        this.moveOrderData = moveOrderData;

        progressManagement.OnStageClear += Reset;
        this.movable.OnMove += CheckClash;
        this.noteManager.OnNoteOccurrence += MoveDirectionChange;

        moveProperty.Value = MoveType.up;
        lastMoveType = moveProperty.Value;
        isClash = false;
    }

    public virtual void MoveHandler(object sender, InputEventArgs inputEventArgs)
    {
        //テスト実装
        if (inputEventArgs.non == 10)
        {
            moveProperty.Value = MoveType.nonMove;
            movable.Move(moveProperty.Value);
        }

        //移動できない状態でクラッシュしていなければReturnする
        if (moveProperty.Value == MoveType.nonMove && !isClash)
        {
            return;
        }

        //連続で入力するとMiss
        if (moveProperty.Value == lastExecutionMoveType)
        {
            Clash();
            return;
        }

        movable.Move(moveProperty.Value);
        lastExecutionMoveType = moveProperty.Value;

    }

    public void Dispose()
    {
        moveProperty.Dispose();
        this.noteManager.OnNoteOccurrence -= MoveDirectionChange;
        progressManagement.OnStageClear -= Reset;
    }

    /// <summary>
    /// 移動方向を切り替える処理
    /// </summary>
    protected void MoveDirectionChange()
    {
        if (isClash)
        {
            moveProperty.Value = MoveType.nonMove;
            return;
        }
        
        //順番通りに切り替える
        if (lastMoveType == moveOrderData.MoveOrder_One)
        {
            moveProperty.Value = moveOrderData.MoveOrder_Two;
            lastMoveType = moveProperty.Value;
        }
        else if (lastMoveType == moveOrderData.MoveOrder_Two)
        {
            moveProperty.Value = moveOrderData.MoveOrder_Three;
            lastMoveType = moveProperty.Value;
        }
        else if (lastMoveType == moveOrderData.MoveOrder_Three)
        {
            moveProperty.Value = moveOrderData.MoveOrder_Four;
            lastMoveType = moveProperty.Value;
        }
        else if (lastMoveType == moveOrderData.MoveOrder_Four)
        {
            moveProperty.Value = moveOrderData.MoveOrder_One;
            lastMoveType = moveProperty.Value;
        }
        lastExecutionMoveType = MoveType.nonMove;
    }

   
    /// <summary>
    /// 移動方向の変更処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="noteEventArgs"></param>
    protected virtual async void MoveDirectionChange(object sender, NoteEventArgs noteEventArgs)
    {

        //ジャストタイミング時に変更
        switch (noteEventArgs.noteTiming)
        {
            case NoteTimingType.just:
                MoveDirectionChange();
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
                NonMove();
                break;
        }

    }

    /// <summary>
    /// 移動できないようにする
    /// </summary>
    protected void NonMove()
    {
        moveProperty.Value = MoveType.nonMove;
    }

    /// <summary>
    /// 衝突Miss
    /// </summary>
    protected void Clash()
    {
        MissOccurrence(MissType.Clash);
    }

    /// <summary>
    /// 移動Miss
    /// </summary>
    protected void MissMove()
    {
        MissOccurrence(MissType.MissMove);
    }

    /// <summary>
    /// Missイベント発行
    /// </summary>
    /// <param name="missType"></param>
    private async void MissOccurrence(MissType missType)
    {
        MistakeEventArgs clashEventArgs = new MistakeEventArgs();
        clashEventArgs.nowMissTime = true;
        isClash = true;
        clashEventArgs.missType = missType;
        OnMoveMistake?.Invoke(this, clashEventArgs);

        await Task.Delay(1000);
        clashEventArgs.nowMissTime = false;
        isClash = false;
        OnMoveMistake?.Invoke(this, clashEventArgs);
    }

    /// <summary>
    /// クラッシュしているかチェックする
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="moveEventArgs"></param>
    private void CheckClash(object sender, MoveEventArgs moveEventArgs)
    {
        //クラッシュすればMiss
        if (moveEventArgs.isCrash)
        {
            Clash();
        }
    }


    private void Reset(object sender, FinishEventArgs finishEventArgs)
    {
        Dispose();
    }

}
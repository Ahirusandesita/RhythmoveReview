/* 制作日
*　製作者
*　最終更新日
*/

using System.Collections.Generic;
using VContainer;

/// <summary>
/// フィールド
/// </summary>
public class Field
{

    //とりあえず実装


    /// <summary>
    /// 移動ハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MoveFieldEventHandler(object sender, MoveEventArgs e);
    /// <summary>
    /// フィールドを移動するときに発行する
    /// </summary>
    public event MoveFieldEventHandler OnMoveField;


    private List<IReadOnlyPositionAdapter> objects = new List<IReadOnlyPositionAdapter>();
    private object lockObject = new object();

    private List<IReadOnlyPositionAdapter> goals = new List<IReadOnlyPositionAdapter>();
    private object lockGoal = new object();

    private List<IRoute> roots = new List<IRoute>();
    private object lockRoots = new object();

    private List<IDamageable> damageables = new List<IDamageable>();
    private object lockDamageable = new object();

    private IReadOnlyPositionAdapter positionAdapter;
    private IReadOnlyPositionAdapter demon;

    private IProgressUsable progressUse;

    [Inject]
    public Field(IProgressUsable progressUse)
    {
        this.progressUse = progressUse;
    }


    public void AddObject(IMoveEventHandler moveEventHandler)
    {
        lock (lockObject)
        {
            moveEventHandler.OnMove += IsGoal;
        }
    }
    public void AddObject(IReadOnlyPositionAdapter positionAdapter, IRoute root)
    {
        lock (lockRoots)
        {
            objects.Add(positionAdapter);
            roots.Add(root);
        }
    }
    public void AddGoal(IReadOnlyPositionAdapter positionAdapter)
    {
        lock (lockGoal)
        {
            goals.Add(positionAdapter);
        }
    }
    public void AddDamage(IDamageable damageable)
    {
        lock (lockDamageable)
        {
            damageables.Add(damageable);
            damageable.OnDamage += ALive;
        }
    }

    public bool CanMove(MoveType moveType, IReadOnlyPositionAdapter positionAdapter)
    {
        PositionData positionData = default;

        switch (moveType)
        {
            case MoveType.down:
                positionData.x = 0;
                positionData.y = -1;
                break;
            case MoveType.up:
                positionData.x = 0;
                positionData.y = 1;
                break;
            case MoveType.left:
                positionData.x = -1;
                positionData.y = 0;
                break;
            case MoveType.right:
                positionData.x = 1;
                positionData.y = 0;
                break;
        }

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != positionAdapter)
            {
                if(objects[i].Position == positionAdapter.Position + positionData)
                { 
                    for (int j = 0; j < roots[i].MovableDirections.Length; j++)
                    {
                        if (!MoveTypeOperation.CanMoveDirection(moveType, roots[i].MovableDirections[j]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }


    public void IsGoal(object sender, MoveEventArgs moveEventArgs)
    {
        if (!moveEventArgs.isCrash)
        {
            OnMoveField?.Invoke(this, moveEventArgs);
        }
        if (moveEventArgs.player)
        {
            this.positionAdapter = moveEventArgs.positionAdapter;
        }
        if (moveEventArgs.moverType == MoverType.demon)
        {
            this.demon = moveEventArgs.positionAdapter;
        }



        if (moveEventArgs.moverType == MoverType.demon)
        {
            //Debug if
            if (this.positionAdapter is not null)
            {

                if (moveEventArgs.positionAdapter.Position == this.positionAdapter.Position)
                {
                    foreach (IDamageable damageable in damageables)
                    {
                        damageable.Damage(1);//デバッグ
                    }
                }
            }
        }

        if (moveEventArgs.moverType == MoverType.player)
        {
            //Debug if
            if (!(demon is null))
            {
                if (moveEventArgs.positionAdapter.Position == this.demon.Position)
                {
                    foreach (IDamageable damageable in damageables)
                    {
                        damageable.Damage(1);//デバッグ
                    }
                }
            }
        }




        IReadOnlyPositionAdapter positionAdapter = (IReadOnlyPositionAdapter)sender;

        for (int i = 0; i < goals.Count; i++)
        {
            if (goals[i].Position == positionAdapter.Position)
            {
                if (moveEventArgs.player)
                {
                    progressUse.NextStage();
                }

                moveEventArgs.GoalCallback?.Invoke(this, new GoalEventArgs());
            }
        }
    }

    private void ALive(object sender, DamageEventArgs damageEventArgs)
    {
        if (!damageEventArgs.aLive)
        {
            progressUse.Restart();
        }
    }
}
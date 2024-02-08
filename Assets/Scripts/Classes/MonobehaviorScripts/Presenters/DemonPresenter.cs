/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
using System;
/// <summary>
/// 鬼の処理を仲介する
/// </summary>
public class DemonPresenter : MonoBehaviour, IDisposable
{

    private IProgressRegisterable progressManagement;
    private MoveDirectionLogic moveDirectionLogic;
    private DemonMoveView demonMoveView;
    private IMovable movable;



    [Inject]
    public void Inject(IProgressRegisterable progressManagement, IMovable movable, MoveDirectionLogic moveDirectionLogic, DemonMoveView demonMoveView)
    {
        this.progressManagement = progressManagement;
        this.moveDirectionLogic = moveDirectionLogic;
        this.demonMoveView = demonMoveView;
        this.movable = movable;

        this.demonMoveView.OnInput += moveDirectionLogic.MoveHandler;
        this.progressManagement.OnReset += ResetHandler;
        this.movable.OnMove += demonMoveView.MoveHandler;
    }

    public void Dispose()
    {
        this.demonMoveView.OnInput -= moveDirectionLogic.MoveHandler;
        this.progressManagement.OnReset -= ResetHandler;
        this.movable.OnMove -= demonMoveView.MoveHandler;
    }

    public void ResetHandler(object sender, FinishEventArgs finishEventArgs)
    {
        Dispose();
    }
}
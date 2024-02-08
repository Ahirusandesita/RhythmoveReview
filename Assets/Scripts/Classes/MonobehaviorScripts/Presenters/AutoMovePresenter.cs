/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
using System;

/// <summary>
/// AutoMoveの処理を仲介する
/// </summary>
public class AutoMovePresenter : MonoBehaviour, IDisposable
{

    private IMovable movable;
    private MoveDirectionLogic moveDirectionLogic;
    private AutoMoveView autoMoveView;
    private IProgressRegisterable progressManagement;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement, IMovable movable, MoveDirectionLogic moveDirectionLogic, AutoMoveView autoMoveView)
    {
        this.progressManagement = progressManagement;
        this.movable = movable;
        this.moveDirectionLogic = moveDirectionLogic;
        this.autoMoveView = autoMoveView;

        autoMoveView.OnInput += moveDirectionLogic.MoveHandler;
        this.progressManagement.OnReset += ResetHandler;
        this.movable.OnMove += autoMoveView.MoveHandler;
    }

    public void Dispose()
    {
        this.autoMoveView.OnInput -= moveDirectionLogic.MoveHandler;
        this.progressManagement.OnReset -= ResetHandler;
        this.movable.OnMove -= autoMoveView.MoveHandler;
    }
    public void ResetHandler(object sender, FinishEventArgs finishEventArgs)
    {
        Dispose();
    }

}
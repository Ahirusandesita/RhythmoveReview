/* 制作日 2023/11/28
*　製作者
*　最終更新日 2023/11/28
*/

using UnityEngine;
using VContainer;
using UniRx;
using System;
/// <summary>
/// プレイヤーの処理を仲介する
/// </summary>
public class PlayerPresenter : MonoBehaviour, IDisposable
{
    //仲介する対象クラス
    private PlayerView playerView;
    private IMovable movable;
    private MoveDirectionLogicPlayer moveDirectionLogic;
    private State state;
    private IProgressRegisterable progressManagement;
    private IDisposable disposable;

    /// <summary>
    /// VContainer でPlayerLifeTimeScopeからInjectされる
    /// </summary>
    /// <param name="playerView"></param>
    /// <param name="movable"></param>
    /// <param name="moveDirectionLogic"></param>
    /// <param name="playerSE"></param>
    /// <param name="progressManagement"></param>
    [Inject]
    public void Inject(PlayerView playerView, IMovable movable, MoveDirectionLogicPlayer moveDirectionLogic, IProgressRegisterable progressManagement, State state)
    {
        this.playerView = playerView;
        this.movable = movable;
        this.moveDirectionLogic = moveDirectionLogic;
        this.progressManagement = progressManagement;
        this.state = state;
       
    }

    private void Start()
    {
        //Moveの方向変化時の仲介
        disposable = moveDirectionLogic.readOnlyMovePropery.Skip(1).Subscribe(moveType =>
        {
            playerView.MoveDirectionSprite(moveType);
        }
        );

        moveDirectionLogic.OnMoveMistake += playerView.MistakeHandler;
        progressManagement.OnStageClear += playerView.StageClearHandler;
        playerView.OnInput += moveDirectionLogic.MoveHandler;
        progressManagement.OnReset += ResetHandler;
        state.OnDamage += playerView.DamageHandler;
        movable.OnMove += playerView.MoveHandler;
        movable.OnMove += playerView.MoveSEHandler;


        moveDirectionLogic.OnJustTiming += playerView.JustHandler;
    }

    public void Dispose()
    {
        disposable.Dispose();
        moveDirectionLogic.OnMoveMistake -= playerView.MistakeHandler;
        progressManagement.OnStageClear -= playerView.StageClearHandler;
        progressManagement.OnReset -= ResetHandler;
        playerView.OnInput -= moveDirectionLogic.MoveHandler;
        state.OnDamage -= playerView.DamageHandler;
        movable.OnMove -= playerView.MoveHandler;
        movable.OnMove -= playerView.MoveSEHandler;


        moveDirectionLogic.OnJustTiming -= playerView.JustHandler;
    }

    public void ResetHandler(object sender, FinishEventArgs finishEventArgs)
    {
        Dispose();
    }
}
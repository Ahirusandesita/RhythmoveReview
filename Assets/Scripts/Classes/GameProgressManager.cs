/* 制作日
*　製作者
*　最終更新日
*/
using VContainer.Unity;
using VContainer;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;

/// <summary>
/// ゲームの進行管理クラス
/// </summary>
public class GameProgressManager : IProgressRegisterable, IProgressUsable, IStartable, IDisposable
{
    /// <summary>
    /// スタートハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void StartEventHandler(object sender, StartEventArgs e);
    /// <summary>
    /// ゲームスタート時に発行する
    /// </summary>
    public event StartEventHandler OnStart;

    /// <summary>
    /// 停止ハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PauseEventHandler(object sender, PauseEventArgs e);
    /// <summary>
    /// ゲームの一時停止時に発行する
    /// </summary>
    public event PauseEventHandler OnPause;

    /// <summary>
    /// 終了ハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FinishEventHandler(object sender, FinishEventArgs e);
    /// <summary>
    /// ゲーム終了時に発行する
    /// </summary>
    public event FinishEventHandler OnFinish;
    /// <summary>
    /// ステージクリア時に発行する
    /// </summary>
    public event FinishEventHandler OnStageClear;
    /// <summary>
    /// ゲームのリセット時に発行する
    /// </summary>
    public event FinishEventHandler OnReset;

    /// <summary>
    /// 進行ハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NextEventHandler(object sender, NextEventArgs e);
    /// <summary>
    /// 次ステージ時に発行する
    /// </summary>
    public event NextEventHandler OnNextStage;

    
    private InGameOrderAsset inGameOrderAsset;
    private InGameTemplateAsset inGameTemplateAsset;
    private List<GameObject> gameObjects = new List<GameObject>();

    //仮　ステージ番号
    private int index;

    /// <summary>
    /// 仮
    /// </summary>
    private StateData stateData;

    private CancellationTokenSource cts = new CancellationTokenSource();
    private CancellationToken token;

    [Inject]
    public GameProgressManager(InGameOrderAsset inGameOrderAsset, InGameTemplateAsset inGameTemplateAsset)
    {
        this.inGameOrderAsset = inGameOrderAsset;
        this.inGameTemplateAsset = inGameTemplateAsset;

        //仮
        stateData.hp = 5;
    }

    /// <summary>
    /// インゲーム開始
    /// </summary>
    public void InGameStart()
    {
        CancellationToken token = cts.Token;
        StartEventArgs startEventArgs = new StartEventArgs();
        startEventArgs.stateData = stateData;
        OnStart?.Invoke(this, startEventArgs);
    }
    /// <summary>
    /// ゲーム一時停止
    /// </summary>
    public void Pause()
    {
        PauseEventArgs pauseEventArgs = new PauseEventArgs();
        OnPause?.Invoke(this, pauseEventArgs);
    }
    /// <summary>
    /// インゲーム終了
    /// </summary>
    public void InGameFinish()
    {
        FinishEventArgs finishEventArgs = new FinishEventArgs();
        OnFinish?.Invoke(this, finishEventArgs);
        Dispose();
    }

    public async void NextStage()
    {
        //ステージクリアイベントを発行
        OnStageClear?.Invoke(this, new FinishEventArgs());

        //仮で4000ミリ秒位まつ
        await Task.Delay(4000, token);
        if (cts.IsCancellationRequested)
        {
            return;
        }

        //前回のステージをすべて破棄
        for (int i = 0; i < gameObjects.Count; i++)
        {
            MonoBehaviour.Destroy(gameObjects[i]);
        }
        gameObjects.Clear();

        //次ステージイベント発行
        OnNextStage?.Invoke(this, new NextEventArgs());
        //仮で1000ミリ秒位まつ
        await Task.Delay(1000, token);

        //ステージ番号を加算して最終ステージを超えたらゲーム終了イベント発行
        index++;
        if (index >= inGameOrderAsset.inGameOrders.Count)
        {
            InGameFinish();
            return;
        }


        if (cts.IsCancellationRequested)
        {
            return;
        }
        StageCreate();
    }

    public async void Restart()
    {
        //仮実装
        //HPを減らして０以下なら終了
        stateData.hp--;
        if (stateData.hp <= 0)
        {
            InGameFinish();
        }

        //リセットイベント発行
        FinishEventArgs finishEventArgs = new FinishEventArgs();
        finishEventArgs.stateData = stateData;
        OnReset?.Invoke(this, finishEventArgs);

        //仮で2000ミリ秒位まつ
        await Task.Delay(2000, token);
        if (cts.IsCancellationRequested)
        {
            return;
        }

        //ステージをすべて破棄
        for (int i = 0; i < gameObjects.Count; i++)
        {
            MonoBehaviour.Destroy(gameObjects[i]);
        }
        gameObjects.Clear();
        OnNextStage?.Invoke(this, new NextEventArgs());

        // 仮で1000ミリ秒位まつ
        await Task.Delay(1000, token);

        if (cts.IsCancellationRequested)
        {
            return;
        }
        StageCreate();
    }

    async void IStartable.Start()
    {
        //仮で1000ミリ秒位まつ
        await Task.Delay(1000, token);

        StageCreate();
    }

    public void Dispose()
    {
        cts.Cancel();
    }

    /// <summary>
    /// ステージを生成してゲーム開始
    /// </summary>
    private void StageCreate()
    {
        for (int i = 0; i < inGameTemplateAsset.inGameTemplates.Count; i++)
        {
            gameObjects.Add(MonoBehaviour.Instantiate(inGameTemplateAsset.inGameTemplates[i]));
        }
        gameObjects.Add(MonoBehaviour.Instantiate(inGameOrderAsset.inGameOrders[index]).gameObject);

        InGameStart();
    }
}
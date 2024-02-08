/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using VContainer;

public class StageClearManager : MonoBehaviour,IDisposable
{
    [SerializeField]
    private Image stageClearImage;
    private Color color;
    private IProgressRegisterable progressManagement;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement)
    {
        this.progressManagement = progressManagement;
        this.progressManagement.OnStageClear += StageClear;
        this.progressManagement.OnNextStage += NextStageHandler;
    }

    private void Awake()
    {
        color = stageClearImage.color;
        color.a = 0f;
        stageClearImage.color = color;
    }
    private void StageClear(object sender,FinishEventArgs finishEventArgs)
    {
        color.a = 1f;
        stageClearImage.color = color;
    }
    private void NextStageHandler(object sender,NextEventArgs nextStageEventArgs)
    {
        color.a = 0f;
        stageClearImage.color = color;
    }
    public void Dispose()
    {
        this.progressManagement.OnStageClear -= StageClear;
        this.progressManagement.OnNextStage -= NextStageHandler;
    }
}
/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
using System;

public class BGMManager : MonoBehaviour, IDisposable
{

    [SerializeField]
    private AudioSource audioSource;

    private AudioClip BGM;
    private IProgressRegisterable progressManagement;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement, StageInformationAsset stageInformationAsset)
    {
        this.progressManagement = progressManagement;

        progressManagement.OnStart += StartHandler;
        progressManagement.OnStageClear += StageClearHandler;
        progressManagement.OnReset += ResetHandler;
        BGM = Resources.Load<AudioClip>(stageInformationAsset.BGMRhytmInformation);
    }

    public void StartHandler(object sender, StartEventArgs startEventArgs)
    {
        audioSource.PlayOneShot(BGM);
    }
    public void StageClearHandler(object sender, FinishEventArgs finishEventArgs)
    {
        audioSource.Stop();
    }

    public void Dispose()
    {
        progressManagement.OnStart -= StartHandler;
        progressManagement.OnStageClear -= StageClearHandler;
        progressManagement.OnReset -= ResetHandler;
    }

    private void Stop()
    {
        audioSource.Stop();
    }

    private void ResetHandler(object sender, FinishEventArgs finishEventArgs)
    {
        Stop();
        Dispose();
    }
}
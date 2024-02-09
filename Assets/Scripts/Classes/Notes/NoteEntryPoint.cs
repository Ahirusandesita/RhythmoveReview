/* 制作日
*　製作者
*　最終更新日
*/

using VContainer.Unity;
using VContainer;
using System;

/// <summary>
/// NoteManagerにエントリーポイントでアクセスする
/// </summary>
public class NoteEntryPoint : ITickable, IDisposable
{

    private IProgressRegisterable progressManagement;
    private NoteManager noteManager;


    [Inject]
    public NoteEntryPoint(IProgressRegisterable progressManagement, NoteManager noteManager)
    {
        this.progressManagement = progressManagement;
        this.noteManager = noteManager;

        progressManagement.OnStart += StartHandler;
    }

    public void Tick()
    {
        noteManager.noteTimingHandler?.Invoke();
    }

    public void Dispose()
    {
        progressManagement.OnStart -= StartHandler;
    }

    private void StartHandler(object sender, StartEventArgs startEventArgs)
    {
        noteManager.Play();
    }

}
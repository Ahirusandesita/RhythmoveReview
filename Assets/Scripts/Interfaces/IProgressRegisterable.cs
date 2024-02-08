/* 制作日
*　製作者
*　最終更新日
*/

/// <summary>
/// ゲーム進行にイベント登録
/// </summary>
public interface IProgressRegisterable
{
    public event GameProgressManager.StartEventHandler OnStart;
    public event GameProgressManager.PauseEventHandler OnPause;
    public event GameProgressManager.FinishEventHandler OnFinish;
    public event GameProgressManager.NextEventHandler OnNextStage;
    public event GameProgressManager.FinishEventHandler OnStageClear;
    public event GameProgressManager.FinishEventHandler OnReset;
}
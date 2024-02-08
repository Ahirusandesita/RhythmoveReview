/* 制作日
*　製作者
*　最終更新日
*/

/// <summary>
/// ゲーム進行管理
/// </summary>
public interface IProgressUsable
{
    /// <summary>
    /// 一時停止
    /// </summary>
    void Pause();
    /// <summary>
    /// 次ステージ
    /// </summary>
    void NextStage();
    /// <summary>
    /// リスタート
    /// </summary>
    void Restart();
}
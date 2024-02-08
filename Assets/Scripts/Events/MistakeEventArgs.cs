/* 制作日
*　製作者
*　最終更新日
*/
using System;

/// <summary>
/// ミスイベントの情報
/// </summary>
public class MistakeEventArgs : EventArgs
{
    /// <summary>
    /// 衝突時間中かどうか
    /// </summary>
    public bool nowMissTime;
    /// <summary>
    /// ミスのタイプ
    /// </summary>
    public MissType missType;
    /// <summary>
    /// どの方向にミスをしたか
    /// </summary>
    public MoveType missDirection;
}
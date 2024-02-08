/* 制作日
*　製作者
*　最終更新日
*/
using System;

/// <summary>
/// ダメージイベントの情報
/// </summary>
public class DamageEventArgs : EventArgs
{
    public StateData stateData;
    public bool aLive;
}
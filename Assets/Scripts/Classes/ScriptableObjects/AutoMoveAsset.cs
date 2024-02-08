/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 移動する順番の管理
/// </summary>
[CreateAssetMenu(fileName = "AutoMoveAsset", menuName = "ScriptableObjects/CreateAutoMoveAsset")]
public class AutoMoveAsset : ScriptableObject
{
    /// <summary>
    /// 移動する順番
    /// </summary>
    public List<MoveType> autoMoveOrders = new List<MoveType>();
}
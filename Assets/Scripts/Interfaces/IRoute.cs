/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
/// <summary>
/// 移動できる方向
/// </summary>
public interface IRoute
{
    public MoveType[] MovableDirections { get; }
}

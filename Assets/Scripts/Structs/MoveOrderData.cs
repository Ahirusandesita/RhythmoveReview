/* 制作日
*　製作者
*　最終更新日
*/

/// <summary>
/// 移動方向の順番データ
/// </summary>
[System.Serializable]
public struct MoveOrderData
{
    /// <summary>
    /// 一番目
    /// </summary>
    public MoveType MoveOrder_One;
    /// <summary>
    /// 二番目
    /// </summary>
    public MoveType MoveOrder_Two;
    /// <summary>
    /// 三番目
    /// </summary>
    public MoveType MoveOrder_Three;
    /// <summary>
    /// 四番目
    /// </summary>
    public MoveType MoveOrder_Four;

    /// <summary>
    /// 移動方向を決めて生成
    /// </summary>
    /// <param name="MoveOrder_One">一番目</param>
    /// <param name="MoveOrder_Two">二番目</param>
    /// <param name="MoveOrder_Three">三番目</param>
    /// <param name="MoveOrder_Four">四番目</param>
    public MoveOrderData(MoveType MoveOrder_One, MoveType MoveOrder_Two, MoveType MoveOrder_Three, MoveType MoveOrder_Four)
    {
        this.MoveOrder_One = MoveOrder_One;
        this.MoveOrder_Two = MoveOrder_Two;
        this.MoveOrder_Three = MoveOrder_Three;
        this.MoveOrder_Four = MoveOrder_Four;
    }
}
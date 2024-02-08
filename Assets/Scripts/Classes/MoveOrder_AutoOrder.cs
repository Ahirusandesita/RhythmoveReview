/* 制作日
*　製作者
*　最終更新日
*/
using System.Collections.Generic;

/// <summary>
/// 次に移動するべき場所を管理するクラス
/// </summary>
public class MoveOrder_AutoOrder
{
    private List<MoveType> moveOrder_AutoOrders = new List<MoveType>();

    public MoveOrder_AutoOrder(List<MoveType> moveOrder_AutoOrders)
    {
        this.moveOrder_AutoOrders = moveOrder_AutoOrders;
    }

    public bool this[int index, MoveType moveType]
    {
        get
        {
            if (moveType == moveOrder_AutoOrders[index])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public int AdjustmentIndex(int index)
    {
        if (index >= moveOrder_AutoOrders.Count)
        {
            return 0;
        }
        return index;
    }
}
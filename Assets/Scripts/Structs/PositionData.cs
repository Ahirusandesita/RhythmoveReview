/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;

/// <summary>
/// 座標データ
/// </summary>
public struct PositionData
{

    public float x;
    public float y;

    public PositionData(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static PositionData operator +(PositionData left, PositionData right)
    {
        PositionData positionData = default;
        positionData.x = left.x + right.x;
        positionData.y = left.y + right.y;
        return positionData;
    }
    public static Vector3 operator +(Vector3 left,PositionData right)
    {
        Vector3 vector3 = default;
        vector3.x = left.x + right.x;
        vector3.y = left.y + right.y;
        return vector3;
    }
}
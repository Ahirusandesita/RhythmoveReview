/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
/// <summary>
/// ストレートの移動方向
/// </summary>
public class Straight : Route
{

    [Inject]
    public Straight(Transform transform, Field field) : base(transform, field) { }

    protected override void CreateMovableDirection()
    {
        if (this.transform.rotation == Quaternion.Euler((new Vector3(0f, 0f, 0f))))
        {
            movableDirections = new MoveType[] { MoveType.left, MoveType.right };
        }

        else if (this.transform.rotation == Quaternion.Euler(new Vector3(0f, 0f, 90f)))
        {
            movableDirections = new MoveType[] { MoveType.up, MoveType.down };
        }

        else if (this.transform.rotation == Quaternion.Euler(new Vector3(0f, 0f, 180f)))
        {
            movableDirections = new MoveType[] { MoveType.right, MoveType.right };
        }

        else if (this.transform.rotation == Quaternion.Euler(new Vector3(0f, 0f, 270f)))
        {
            movableDirections = new MoveType[] { MoveType.down, MoveType.up };
        }
    }
}
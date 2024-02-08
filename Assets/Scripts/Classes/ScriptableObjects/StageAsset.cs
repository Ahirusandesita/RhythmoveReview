/* 制作日
*　製作者
*　最終更新日
*/
using UnityEngine;

/// <summary>
/// ステージ情報
/// </summary>
[CreateAssetMenu(fileName = "StageAsset", menuName = "ScriptableObjects/CreateStageAsset")]
public class StageAsset : ScriptableObject
{
    public AutoMoveAsset autoMoveAsset;
    public MoveOrderData moveOrder;
    public FieldManager fieldManager;
    public StageInformationAsset StageInformationAsset;
    public DemonData demonData;
    public SEAsset SEAsset;

    //仮
    public Vector3 StartPosition;
}
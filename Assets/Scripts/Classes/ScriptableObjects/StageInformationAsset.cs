/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;

/// <summary>
/// ステージのBGM、Note情報のファイル名
/// </summary>
[CreateAssetMenu(fileName = "StageInformationAsset", menuName = "ScriptableObjects/CreateStageInformationAsset")]
public class StageInformationAsset : ScriptableObject
{
    public string BGMRhytmInformation;
}
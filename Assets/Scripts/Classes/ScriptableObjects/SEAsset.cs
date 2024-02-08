/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;

/// <summary>
/// SE情報
/// </summary>
[CreateAssetMenu(fileName = "SEAsset", menuName = "ScriptableObjects/CreateSEInformation")]
public class SEAsset : ScriptableObject
{
    public DirectionAudioClip DirectionAudioClip;
    /// <summary>
    /// 誘導SE
    /// </summary>
    public AudioClip ExsampleAudioClip;
}

/// <summary>
/// 移動方向毎のSE
/// </summary>
[System.Serializable]
public class DirectionAudioClip
{
    public AudioClip UpAudioClip;
    public AudioClip DownAudioClip;
    public AudioClip RightAudioClip;
    public AudioClip LeftAudioClip;
}
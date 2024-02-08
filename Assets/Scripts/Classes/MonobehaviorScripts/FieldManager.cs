/* 制作日
*　製作者
*　最終更新日
*/
using UnityEngine;
using VContainer;

/// <summary>
/// ステージ全体の管理クラス
/// </summary>
public class FieldManager : MonoBehaviour
{

    //すべて仮実装

    private GameObject obj;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement)
    {
        progressManagement.OnNextStage += NextStage;
    }

    public void SetGameObject(GameObject obj)
    {
        this.obj = obj;
    }

    public void NextStage(object sender, NextEventArgs nextStageEventArgs)
    {
        Destroy(obj);
    }
}
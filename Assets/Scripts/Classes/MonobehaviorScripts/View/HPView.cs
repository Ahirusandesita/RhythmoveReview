/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
using System.Collections.Generic;
using System;

public class HPView : MonoBehaviour,IDisposable
{

    [SerializeField]
    private GameObject HPUI;

    private List<GameObject> HPUIs = new List<GameObject>();

    private IProgressRegisterable progressManagement;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement)
    {
        this.progressManagement = progressManagement;

        this.progressManagement.OnStart += StartHandler;
        this.progressManagement.OnReset += ResetHandler;
    }

    private void StartHandler(object sender, StartEventArgs startEventArgs)
    {
        this.progressManagement.OnStart -= StartHandler;
        for (int i = 0; i < startEventArgs.stateData.hp; i++)
        {
            HPUIs.Add(Instantiate(HPUI, new Vector3((i * 0.8f) + 5f, 6.15f, 0f), Quaternion.identity));
        }
    }

    private void ResetHandler(object sender, FinishEventArgs finishEventArgs)
    {
        Destroy(HPUIs[finishEventArgs.stateData.hp]);
    }

    public void Dispose()
    {
        progressManagement.OnStart -= StartHandler;
        progressManagement.OnReset -= ResetHandler;
    }

}
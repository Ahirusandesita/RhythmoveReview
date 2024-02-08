/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using System.Collections;
using System;
using VContainer;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;

public class DemonMoveView : MonoBehaviour, IDisposable
{
    public delegate void AutoInputHandler(object sender, InputEventArgs e);
    public event AutoInputHandler OnInput;

    private MoveOrder_AutoOrder moveOrder_AutoOrder;
    private MoveDirectionLogic moveDirectionLogic;
    // private IMoveEventHandler moveEventHandler;
    private Field field;
    private DemonData demonData;

    private IDisposable disposable;

    private CancellationTokenSource cts = new CancellationTokenSource();
    private bool canMove = false;
    private int index = 0;

    private IProgressRegisterable progressManagement;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement, MoveOrder_AutoOrder moveOrder_AutoOrder, MoveDirectionLogic moveDirectionLogic, IMoveEventHandler moveEventHandler, Field field, DemonData demonData)
    {
        this.moveOrder_AutoOrder = moveOrder_AutoOrder;
        this.moveDirectionLogic = moveDirectionLogic;
        this.field = field;
        this.demonData = demonData;
        this.progressManagement = progressManagement;

        this.field.OnMoveField += MoveStart;
        this.progressManagement.OnReset += Reset;
    }

    private void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    private async UniTask InputSubscribe()
    {
        await UniTask.WaitUntil(() => InputSystem.Instance is not null);
        //disposable = InputSystem.Instance.readOnlyMoveProperty.Skip(1).Subscribe(_ => this.gameObject.SetActive(false));
    }

    public void Reset(object sender, FinishEventArgs finishEventArgs)
    {
        //Dispose();
        this.progressManagement.OnReset -= Reset;
    }

    public void Dispose()
    {
        //disposable.Dispose();
        field.OnMoveField -= MoveStart;
        cts.Cancel();
        cts.Dispose();
    }

    private async void MoveStart(object sender, MoveEventArgs moveEventArgs)
    {
        if (!moveEventArgs.player)
        {
            return;
        }

        if (moveEventArgs.player)
        {
            if (moveEventArgs.isCrash)
            {
                return;
            }
            if (moveEventArgs.moveType == MoveType.nonMove)
            {
                return;
            }
        }

        field.OnMoveField -= MoveStart;


        CancellationToken token = cts.Token;
        canMove = false;

        await Task.Delay(1500, token);
        InputEventArgs inputEventArgs = new InputEventArgs();
        inputEventArgs.non = 10;
        OnInput?.Invoke(this, inputEventArgs);
        _ = StartCoroutine(SpawnAnimation());

        MoveDelay(token).Forget();

        this.moveDirectionLogic.readOnlyMovePropery.
           Where(moveType => this.moveOrder_AutoOrder[index, moveType]).Where(moveType => canMove).
           Subscribe(moveType =>
           {
               OnInput?.Invoke(this, new InputEventArgs());
           }
           );

        //this.moveEventHandler.OnGoal += Reset;

        InputSubscribe().Forget();
    }

    private async UniTask MoveDelay(CancellationToken token)
    {

        while (true)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            await UniTask.Delay((TimeSpan.FromSeconds(demonData.demonMoveDelay)));
            canMove = true;
        }
    }

    private IEnumerator SpawnAnimation()
    {
        Vector3 size = this.transform.localScale;
        this.transform.localScale = Vector3.zero;
        this.GetComponent<SpriteRenderer>().enabled = true;
        for(int i = 0; i < 13; i++)
        {
            this.transform.localScale += size / 10f;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < 3; i++)
        {
            this.transform.localScale -= size / 10f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private IEnumerator MoveAnimation()
    {
        Vector3 size = this.transform.localScale;
        for (int i = 0; i < 5; i++)
        {
            this.transform.localScale += size / 10f;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < 5; i++)
        {
            this.transform.localScale -= size / 10f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void MoveHandler(object sender,MoveEventArgs moveEventArgs)
    {
        if(moveEventArgs.moveType == MoveType.nonMove)
        {
            return;
        }
        index++;
        index = this.moveOrder_AutoOrder.AdjustmentIndex(index);
        canMove = false;
        StartCoroutine(MoveAnimation());
    }

}
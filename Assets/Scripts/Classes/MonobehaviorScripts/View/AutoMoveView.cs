/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using System.Collections;
using VContainer;
using System;
using Cysharp.Threading.Tasks;
using UniRx;

public class AutoMoveView : MonoBehaviour, IDisposable
{

    public delegate void AutoInputHandler(object sender, InputEventArgs e);
    public event AutoInputHandler OnInput;

    private MoveOrder_AutoOrder moveOrder_AutoOrder;
    private MoveDirectionLogic moveDirectionLogic;

    private IDisposable disposable;
    private int index = 0;

    private WaitForSeconds waitForSeconds = new WaitForSeconds(0.01f);
    private Vector3 size;

    [SerializeField]
    private AudioSource audioSource;

    private SEAsset SEAsset;

    private void Awake()
    {
        size = this.transform.localScale;
        this.transform.localScale = Vector3.zero;
    }

    [Inject]
    public void Inject(MoveOrder_AutoOrder moveOrder_AutoOrder, MoveDirectionLogic moveDirectionLogic, IMoveEventHandler moveEventHandler,SEAsset SEAsset)
    {
        this.moveOrder_AutoOrder = moveOrder_AutoOrder;
        this.moveDirectionLogic = moveDirectionLogic;
        this.SEAsset = SEAsset;

        this.moveDirectionLogic.readOnlyMovePropery.
            Where(moveType => this.moveOrder_AutoOrder[index, moveType]).
            Subscribe(moveType =>
            {
                OnInput?.Invoke(this, new InputEventArgs());
            }
            );

        moveEventHandler.OnGoal += Reset;

        InputSubscribe().Forget();
    }

    private async UniTask InputSubscribe()
    {
        await UniTask.WaitUntil(() => InputSystem.Instance is not null);
        disposable = InputSystem.Instance.readOnlyMoveProperty.Skip(1).Subscribe(_ => this.gameObject.SetActive(false));
    }

    public void Reset(object sender, GoalEventArgs goalEventArgs)
    {

    }

    public void MoveHandler(object sender, MoveEventArgs moveEventArgs)
    {
        index++;
        index = this.moveOrder_AutoOrder.AdjustmentIndex(index);
        StartCoroutine(AutoAnimation());

        if (moveEventArgs.isCrash)
        {
            return;
        }

        switch (moveEventArgs.moveType)
        {
            case MoveType.up:
                audioSource.PlayOneShot(SEAsset.DirectionAudioClip.UpAudioClip);
                break;
            case MoveType.down:
                audioSource.PlayOneShot(SEAsset.DirectionAudioClip.DownAudioClip);
                break;
            case MoveType.left:
                audioSource.PlayOneShot(SEAsset.DirectionAudioClip.LeftAudioClip);
                break;
            case MoveType.right:
                audioSource.PlayOneShot(SEAsset.DirectionAudioClip.RightAudioClip);
                break;
        }
    }

    public void Dispose()
    {
        disposable.Dispose();
    }

    private IEnumerator AutoAnimation()
    {
       this.transform.localScale = this.size;

        Vector3 size = this.transform.localScale;

        for (int i = 0; i < 10; i++)
        {
            size -= (this.size / 10f);
            this.transform.localScale = size;
            yield return waitForSeconds;
        }
    }

}
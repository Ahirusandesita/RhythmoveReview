/* 制作日 2023/11/28
*　製作者
*　最終更新日 2023/11/28
*/

using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using System.Collections;
using VContainer;
using System.Threading.Tasks;

/// <summary>
/// 仮実装
/// </summary>
public class PlayerView : MonoBehaviour, IDisposable
{

    

    public delegate void PlayerInputHandler(object sender, InputEventArgs e);
    public event PlayerInputHandler OnInput;

    //ここから仮実装-------------------------------------
    [SerializeField]
    private SpriteRenderer upImage;
    [SerializeField]
    private SpriteRenderer downImage;
    [SerializeField]
    private SpriteRenderer rightImage;
    [SerializeField]
    private SpriteRenderer leftImage;

    private SpriteRenderer nowImage;
    private Vector2 size;

    [SerializeField]
    private GameObject spriteParent;
    [SerializeField]
    private GameObject face;
    [SerializeField]
    private GameObject deahObject;
    [SerializeField]
    private GameObject missMoveObject;
    [SerializeField]
    private GameObject upClashObject;
    [SerializeField]
    private GameObject downClashObject;

    [SerializeField]
    private GameObject leftClashObject;
    [SerializeField]
    private GameObject rightClashObject;

    [SerializeField]
    private GameObject windUpObject;
    [SerializeField]
    private GameObject windDownObject;
    [SerializeField]
    private GameObject windRightObject;
    [SerializeField]
    private GameObject windLeftObject;

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioSource TestExsampleAudioSource;

    [SerializeField]
    private AudioClip missSE;
    [SerializeField]
    private AudioClip deathSE;

    [SerializeField]
    private GameObject upFace;
    private int upFaceIndex = 4;
    private bool isClash = false;
    private bool isDeath = false;
    //----------------------------------------------------

    private IDisposable disposable;
    private MoveOrder_AutoOrder moveOrder_AutoOrder;
    private int index;
    private SEAsset SEAsset;

    [Inject]
    public void Inject(MoveOrder_AutoOrder moveOrder_AutoOrder, SEAsset SEAsset)
    {
        this.moveOrder_AutoOrder = moveOrder_AutoOrder;
        this.SEAsset = SEAsset;
    }

    private void Awake()
    {
        deahObject.SetActive(false);
        missMoveObject.SetActive(false);

        upClashObject.SetActive(false);
        downClashObject.SetActive(false);
        rightClashObject.SetActive(false);
        leftClashObject.SetActive(false);

        windUpObject.SetActive(false);
        windDownObject.SetActive(false);
        windRightObject.SetActive(false);
        windLeftObject.SetActive(false);
        upFace.SetActive(false);

        InputSubscribe().Forget();
    }

    private async UniTask InputSubscribe()
    {
        await UniTask.WaitUntil(() => InputSystem.Instance is not null);
        disposable = InputSystem.Instance.readOnlyMoveProperty.Skip(1).Subscribe(_ => OnInput?.Invoke(this, new InputEventArgs()));
    }

    public void MoveDirectionSprite(MoveType moveType)
    {

        if (moveOrder_AutoOrder[index, moveType])
        {
            TestExsampleAudioSource.PlayOneShot(SEAsset.ExsampleAudioClip);
        }
        switch (moveType)
        {
            case MoveType.down:
                upFaceIndex++;
                SpriteAnimation(downImage);
                break;
            case MoveType.up:
                if (upFaceIndex >= 4 && !isClash)
                {
                    face.SetActive(false);
                    upFace.SetActive(true);
                }
                upFaceIndex++;
                SpriteAnimation(upImage);
                break;
            case MoveType.right:
                upFaceIndex++;
                SpriteAnimation(rightImage);
                break;
            case MoveType.left:
                upFaceIndex++;
                SpriteAnimation(leftImage);
                break;
        }
    }

    private void ArrowSpriteOff()
    {
        downImage.enabled = false;
        upImage.enabled = false;
        rightImage.enabled = false;
        leftImage.enabled = false;
    }

    public void ArrowSpriteOn()
    {
        downImage.enabled = true;
        upImage.enabled = true;
        rightImage.enabled = true;
        leftImage.enabled = true;
    }

    private void ArrowColor_Yellow()
    {
        downImage.color = Color.yellow;
        upImage.color = Color.yellow;
        rightImage.color = Color.yellow;
        leftImage.color = Color.yellow;
    }

    private void SpriteAnimation(SpriteRenderer spriteRenderer)
    {
        nowImage = spriteRenderer;
        ArrowColor_Yellow();
        spriteRenderer.color = Color.white;
        size = nowImage.gameObject.transform.localScale;
        nowImage.gameObject.transform.localScale = new Vector2(size.x * 1.3f, size.y * 1.3f);

        StartCoroutine(DirectionAnimation(spriteRenderer));
    }

    private IEnumerator DirectionAnimation(SpriteRenderer spriteRenderer)
    {
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.gameObject.transform.localScale = size;
        ArrowColor_Yellow();
        if (!isClash && !isDeath)
        {
            face.SetActive(true);
        }
        upFace.SetActive(false);
    }

    public void StageClearHandler(object sender, FinishEventArgs finishEventArgs)
    {
        ArrowSpriteOff();
    }


    /// <summary>
    /// 衝突時
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="clashEventArgs"></param>
    public void MistakeHandler(object sender, MistakeEventArgs clashEventArgs)
    {
        isClash = clashEventArgs.nowMissTime;
        if (!clashEventArgs.nowMissTime)
        {
            ArrowSpriteOn();
            deahObject.SetActive(false);
            missMoveObject.SetActive(false);
            face.SetActive(true);
            upClashObject.SetActive(false);
            downClashObject.SetActive(false);
            rightClashObject.SetActive(false);
            leftClashObject.SetActive(false);
        }
        else
        {
            ArrowSpriteOff();
            audioSource.PlayOneShot(missSE);
            if (clashEventArgs.missType == MissType.MissMove)
            {
                StartCoroutine(MissMoveAnimation());
            }

            if (clashEventArgs.missType == MissType.Clash)
            {
                switch (clashEventArgs.missDirection)
                {
                    case MoveType.down:
                        downClashObject.SetActive(true);
                        break;
                    case MoveType.up:
                        upClashObject.SetActive(true);
                        break;
                    case MoveType.left:
                        leftClashObject.SetActive(true);
                        break;
                    case MoveType.right:
                        rightClashObject.SetActive(true);
                        break;
                    case MoveType.nonMove:
                        StartCoroutine(MissMoveAnimation());
                        break;

                }
                face.SetActive(false);
            }
        }
    }

    private IEnumerator MissMoveAnimation()
    {
        face.SetActive(false);
        missMoveObject.SetActive(true);
        spriteParent.transform.position += Vector3.right / 20f;
        yield return new WaitForSeconds(0.03f);
        for (int i = 0; i < 8; i++)
        {
            spriteParent.transform.position -= Vector3.right / 10f;
            yield return new WaitForSeconds(0.03f);
            spriteParent.transform.position += Vector3.right / 10f;
            yield return new WaitForSeconds(0.03f);
        }
        spriteParent.transform.position -= Vector3.right / 20f;
    }


    public void DamageHandler(object sender, DamageEventArgs damageEventArgs)
    {
        if (!damageEventArgs.aLive)
        {
            ArrowSpriteOff();
            isDeath = true;
            if (face.activeSelf)
            {
                rightClashObject.SetActive(true);
            }
            face.SetActive(false);
            audioSource.PlayOneShot(deathSE);
        }
    }

    public async void MoveHandler(object sender, MoveEventArgs moveEventArgs)
    {
        if (moveEventArgs.isCrash)
        {
            return;
        }

        upFaceIndex = 0;
        if (moveOrder_AutoOrder[index, moveEventArgs.moveType])
        {
            index++;
            index = moveOrder_AutoOrder.AdjustmentIndex(index);
        }
        else
        {
            if (!moveEventArgs.isCrash)
            {
                index--;
                if(index <= 0)
                {
                    index = 0;
                }
            }
        }

        switch (moveEventArgs.moveType)
        {
            case MoveType.up:
                windUpObject.SetActive(true);
                break;
            case MoveType.down:
                windDownObject.SetActive(true);
                break;
            case MoveType.left:
                windRightObject.SetActive(true);
                break;
            case MoveType.right:
                windLeftObject.SetActive(true);
                break;
        }
        await Task.Delay(100);
        windUpObject.SetActive(false);
        windDownObject.SetActive(false);
        windLeftObject.SetActive(false);
        windRightObject.SetActive(false);
    }

    public void MoveSEHandler(object sender, MoveEventArgs moveEventArgs)
    {
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

    public void OnDisable()
    {
        disposable.Dispose();
    }

}
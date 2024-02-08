/* 制作日
*　製作者
*　最終更新日
*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VContainer;
using System;

public class ScreenManager : MonoBehaviour,IDisposable
{

    [SerializeField]
    private Image screenImage;
    private IProgressRegisterable progressManagement;

    [Inject]
    public void Inject(IProgressRegisterable progressManagement)
    {
        this.progressManagement = progressManagement;
        progressManagement.OnStart += StartHandler;
        progressManagement.OnNextStage += NextStageHandler;
    }

    public void Dispose()
    {
        progressManagement.OnStart -= StartHandler;
        progressManagement.OnNextStage -= NextStageHandler;
    }

    private void StartHandler(object sender, StartEventArgs startEventArgs)
    {
        StartCoroutine(FadeIn());
    }
    private void NextStageHandler(object sender, NextEventArgs nextStageEventArgs)
    {
        StartCoroutine(FadeOut());
    }



    private IEnumerator FadeIn()
    {
        float alpha = 1f;
        Color color = screenImage.color;
        color.a = alpha;
        while (alpha > 0f)
        {
            alpha -= 0.1f;
            color.a = alpha;
            screenImage.color = color;

            yield return new WaitForSeconds(0.02f);
        }

        alpha = 0f;
        color.a = alpha;
        screenImage.color = color;
    }

    private IEnumerator FadeOut()
    {
        float alpha = 0f;
        Color color = screenImage.color;
        color.a = alpha;
        while (alpha < 1f)
        {
            alpha += 0.1f;
            color.a = alpha;
            screenImage.color = color;

            yield return new WaitForSeconds(0.02f);
        }

        alpha = 1f;
        color.a = alpha;
        screenImage.color = color;
    }
}
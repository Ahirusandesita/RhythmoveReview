/* 制作日 
*　製作者
*　最終更新日
*/

using UnityEngine;
using VContainer;
using System.Collections.Generic;
using System;

/// <summary>
/// ノーツを管理するクラス
/// </summary>
public class NoteManager : INoteRegisteredable, IDisposable
{
    /// <summary>
    /// Note発生ハンドラ
    /// </summary>
    /// <param name="sender">発行者</param>
    /// <param name="e">イベント情報</param>
    public delegate void NoteOccurrenceHandler(object sender, NoteEventArgs e);
    /// <summary>
    /// Note発生時に発行される
    /// </summary>
    public event NoteOccurrenceHandler OnNoteOccurrence;

    private string BGMFileName;

    private class NoteInformation
    {
        public float noteTimeCount;
        public int noteTimeIndex;
        public NoteTimingType noteTimingType;
        public List<float> noteTimes;

        public List<float> NoteTimes
        {
            set
            {
                noteTimes = value;
            }
        }

        public NoteInformation(float noteTimeCount, int index, NoteTimingType noteTimingType, List<float> noteTimes)
        {
            this.noteTimeCount = noteTimeCount;
            this.noteTimeIndex = index;
            this.noteTimingType = noteTimingType;
            this.noteTimes = noteTimes;
        }
        public NoteInformation(NoteTimingType noteTimingType)
        {
            this.noteTimeCount = 0f;
            this.noteTimeIndex = 0;
            this.noteTimingType = noteTimingType;
            this.noteTimes = new List<float>();
        }
    }

    private NoteInformation noteJust;
    private NoteInformation noteEarly;
    private NoteInformation noteLate;

    public Action action;

    private const int NOTE_JUSTTIMING_INDWX = 0;
    private const int NOTE_EARLYTIMING_INDEX = 1;
    private const int NOTE_LATETIMING_INDEX = 2;

    [Inject]
    public NoteManager(StageInformationAsset stageInformationAsset)
    {
        this.BGMFileName = stageInformationAsset.BGMRhytmInformation;
        noteJust = new NoteInformation(NoteTimingType.just);
        noteEarly = new NoteInformation(NoteTimingType.early);
        noteLate = new NoteInformation(NoteTimingType.late);
        Load();
    }

    /// <summary>
    /// Jsonをロードする
    /// </summary>
    /// <returns></returns>
    private NoteManager Load()
    {
        //JSonファイルを読み取る
        string inputString = Resources.Load<TextAsset>(BGMFileName).ToString();
        Data inputJson = JsonUtility.FromJson<Data>(inputString);

        int noteNum = inputJson.notes.Length;

        float lastDelayTime = 0f;
        float lastDelayEarlyTime = 0f;
        float lastDelayLateTime = 0f;


        for (int i = 0; i < noteNum; i++)
        {
            //ノーツのタイミングを計算する
            //  ((60/(BPM * LPB)) * LPB) * NoteNum / LPB + offset

            float interval = 60 / (inputJson.BPM * (float)inputJson.notes[i].LPB);
            float beatSecond = interval * (float)inputJson.notes[i].LPB;
            float time = (beatSecond * inputJson.notes[i].num / (float)inputJson.notes[i].LPB) + inputJson.offset / 44100f;


            //タイミング毎でListにAdd

            //ジャストタイミング
            if (inputJson.notes[i].block == NOTE_JUSTTIMING_INDWX)
            {
                float workTime = time;
                time -= lastDelayTime;
                lastDelayTime = workTime;
                noteJust.noteTimes.Add(time);
            }

            //早タイミング
            else if (inputJson.notes[i].block == NOTE_EARLYTIMING_INDEX)
            {
                float workTime = time;
                time -= lastDelayEarlyTime;
                lastDelayEarlyTime = workTime;
                noteEarly.noteTimes.Add(time);
            }
            //遅タイミング
            else if (inputJson.notes[i].block == NOTE_LATETIMING_INDEX)
            {
                float workTime = time;
                time -= lastDelayLateTime;
                lastDelayLateTime = workTime;
                noteLate.noteTimes.Add(time);
            }

        }
        return this;
    }

    /// <summary>
    /// Note再生
    /// </summary>
    public void Play()
    {
        action += NoteJust;
        action += NoteEarly;
        action += NoteLate;
    }
    /// <summary>
    /// Noteジャストタイミング
    /// </summary>
    private void NoteJust()
    {
        if (NoteTimingCalculation(noteJust))
        {
            action -= NoteJust;
        }
    }
    /// <summary>
    /// ノーツ許容範囲　タイミング早
    /// </summary>
    private void NoteEarly()
    {
        if (NoteTimingCalculation(noteEarly))
        {
            action -= NoteEarly;
        }
    }
    /// <summary>
    /// ノーツ許容範囲　タイミング遅
    /// </summary>
    private void NoteLate()
    {
        if (NoteTimingCalculation(noteLate))
        {
            action -= NoteLate;
        }
    }

    /// <summary>
    /// ノーツの情報に合わせてイベントを発行する
    /// </summary>
    /// <param name="noteInformation"></param>
    /// <returns></returns>
    private bool NoteTimingCalculation(NoteInformation noteInformation)
    {
        noteInformation.noteTimeCount += Time.deltaTime;
        if (noteInformation.noteTimeCount > noteInformation.noteTimes[noteInformation.noteTimeIndex])
        {
            //ノーツ情報をイベントで発行
            NoteEventArgs noteEventArgs = new NoteEventArgs();
            noteEventArgs.noteTiming = noteInformation.noteTimingType;
            OnNoteOccurrence?.Invoke(this, noteEventArgs);

            //タイミングの誤差を調整する
            //前回のタイミングの計算から本来のタイミングを引いた時間から次のタイミング計算を始める//
            noteInformation.noteTimeCount -= noteInformation.noteTimes[noteInformation.noteTimeIndex];
            noteInformation.noteTimeIndex++;
        }
        if (noteInformation.noteTimeIndex >= noteInformation.noteTimes.Count)
        {
            return true;
        }
        return false;
    }
    public void Dispose()
    {
        OnNoteOccurrence = null;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager instance;

    private List<StageDataClass> stageDataList;
    private Dictionary<int, int, StageDataClass> stageDataDictionary = new Dictionary<int, int, StageDataClass>();

    private int currentStage = 1;
    private int currentChapter = 1;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        if (ReadSpreadSheet.instance.isLoadComplete)
            stageDataList = ReadSpreadSheet.instance.sheetDatas;
        else
            Debug.LogError("Error: SpreadSheet loading is not complete");
    }

    private void Start()
    {
        if (stageDataList != null)
        {
            MakeStageDataDictionary();
        }

        CurStageDataApply(currentStage,currentChapter);
    }

    private void MakeStageDataDictionary()
    {
        foreach (var data in stageDataList)
        {
            if (!stageDataDictionary.ContainsKey(data.stageNum))
                stageDataDictionary.AddOrSet(data.stageNum, new Dictionary<int, StageDataClass>());
            stageDataDictionary[data.stageNum].AddOrSet(data.chapterNum, data);
        }
    }

    public void NextStage()
    {
        currentChapter++;
        if (currentChapter > 4)
        {
            currentStage++;
            currentChapter = 1;
        }

        MainGameLogic.instance.GameReset();
        CurStageDataApply(currentStage, currentChapter);
        Debug.Log("Next Stage");
    }

    private void CurStageDataApply(int curStage, int curChapter)
    {
        var curData = stageDataDictionary[curStage][curChapter];

        UIManager.instance.StageUIApply(curData);
    }

    public StageDataClass GetCurStageData()
    {
        return GetStageData(currentStage, currentChapter);
    }

    public StageDataClass GetStageData(int stageNum, int chapterNum)
    {
        var curData = stageDataDictionary[stageNum][chapterNum];
        return curData;
    }
}
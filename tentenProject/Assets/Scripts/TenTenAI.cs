using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TenTenAI : MonoBehaviour
{
    public static TenTenAI instance;
    [SerializeField] private int EvolutionRepeatCount = 30;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EvolutionLearning();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("Pattern Load");
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Pattern Save");
            BlockArraySave();
        }
    }


    private void BlockArraySave()
    {
        var pool = new BlockInfoForSave(BlockManager.instance.blockQueue);
        var json = JsonUtility.ToJson(pool);

        JsonFileSave(json, "TenTenAI", "");
    }

    private void JsonFileSave(string json, string fileName, string path)
    {
        FileStream fileStream = new FileStream(Application.dataPath + $@"\{path}\{fileName}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(json);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    public void BlockArrayLoad()
    {
        if (!File.Exists(Application.dataPath + @"\TenTenAI.json"))
        {
            Debug.Log("Data File Not Found");
            return;
        }

        var json = File.ReadAllText(Application.dataPath + @"\TenTenAI.json");
        var data = JsonUtility.FromJson<BlockInfoForSave>(json);

        BlockManager.instance.blockQueue.Clear();
        foreach (var blockInfo in data.blockInfos)
        {
            BlockManager.instance.blockQueue.Enqueue(blockInfo);
        }
    }

    private void EvolutionLearning()
    {
        var mapinLogicInstance = MainGameLogic.instance;

        var jsonData = new List<EvolutionData>();
        int generationCount = 0;
        
        for (int i = 0; i < EvolutionRepeatCount; i++)
        {
            var randomEvolutionData = EvolutionAllRandomGenerate();
            jsonData.Add(randomEvolutionData);
            mapinLogicInstance.GameReset();
        }

        SaveEvolutionData(jsonData, generationCount);
    }

    private void SaveEvolutionData(List<EvolutionData> jsonData,int generationCount)
    {
        const string path = "EvolutionFolder";
        var json = JsonUtility.ToJson(new EvolutionDataListJson(jsonData));
        const string fileNameFormat = "GenerateData #{0}";
        var fileName = string.Format(fileNameFormat, generationCount);

        JsonFileSave(json, fileName, path);

        var list = jsonData.OrderByDescending((x) => x.scoreInfo.score);
        Debug.Log($"HighScore : {list.ToList().First().scoreInfo.score}");
        
    }

    private EvolutionData EvolutionAllRandomGenerate()
    {
        var mainLogicInstance = MainGameLogic.instance;
        List<EvolutionData.ActionData> actionData = new List<EvolutionData.ActionData>();

        while (!mainLogicInstance.FailCheck())
        {
            if (BlockManager.instance.blockQueue.Count <= 0)
            {
                BlockArrayLoad();
                BlockManager.instance.BlockRefill();
            }

            var curActionData = new EvolutionData.ActionData();
            Vector2 parseToTilePos;

            do
            {
                mainLogicInstance.PickBlockSet(BlockManager.instance.ingameCellBlocks
                    [Random.Range(0, BlockManager.instance.ingameCellBlocks.Count)]);

                var nowPickBlock = mainLogicInstance.nowPickBlock;
                curActionData.blockType = nowPickBlock.blockNum;
                curActionData.blockRot = nowPickBlock.rotNum;

                var randNumX = Mathf.RoundToInt(Random.Range(0, 10));
                var randNumY = Mathf.RoundToInt(Random.Range(0, 10));
                curActionData.placePosition = new Vector2Int(randNumX, randNumY);
                parseToTilePos = TileMapManager.ChangePosToTilePos(curActionData.placePosition);

                if (MainGameLogic.instance.FailCheck()) break;
            } while (!mainLogicInstance.BlockDrop(parseToTilePos));

            actionData.Add(curActionData);
        }

        var returnData = new EvolutionData(actionData, mainLogicInstance.scoreInfo);
        return returnData;
    }


    [System.Serializable]
    private struct EvolutionDataListJson
    {
        public List<EvolutionData> evolutionData;

        public EvolutionDataListJson(List<EvolutionData> evolutionData)
        {
            this.evolutionData = evolutionData;
        }
    }

    [System.Serializable]
    private struct EvolutionData
    {
        public List<ActionData> actionDataList;
        public ScoreInfo scoreInfo;

        [System.Serializable]
        public struct ActionData
        {
            public int blockType;
            public int blockRot;

            [FormerlySerializedAs("placePositionList")]
            public Vector2Int placePosition;

            public ActionData(int blockType, int blockRot, Vector2Int placePosition)
            {
                this.blockType = blockType;
                this.blockRot = blockRot;
                this.placePosition = placePosition;
            }
        }

        public EvolutionData(List<ActionData> actionDataList, ScoreInfo scoreInfo)
        {
            this.actionDataList = actionDataList;
            this.scoreInfo = scoreInfo;
        }
    }

    [System.Serializable]
    private struct BlockInfoForSave
    {
        public List<BlockManager.BlockInfo> blockInfos;

        public BlockInfoForSave(Queue<BlockManager.BlockInfo> blockInfos)
        {
            this.blockInfos = blockInfos.ToList();
        }
    }
}
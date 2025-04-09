using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class TenTenAI : MonoBehaviour
{
    public static TenTenAI instance;

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

    [System.Serializable]
    private struct BlockInfoForSave
    {
        public List<BlockManager.BlockInfo> blockInfos;

        public BlockInfoForSave(Queue<BlockManager.BlockInfo> blockInfos)
        {
            this.blockInfos = blockInfos.ToList();
        }
    }

    private void BlockArraySave()
    {
        var pool = new BlockInfoForSave(BlockManager.instance.blockQueue);
        var json = JsonUtility.ToJson(pool);

        FileStream fileStream = new FileStream(Application.dataPath + @"\TenTenAI.json", FileMode.Create);
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
            Debug.Log(BlockManager.instance.blockQueue);
            BlockManager.instance.blockQueue.Enqueue(blockInfo);
        }
    }

    private void EvolutionLearning()
    {
        var mainLogicInstance = MainGameLogic.instance;
        BlockArrayLoad();
        BlockManager.instance.BlockRefill();
        
        Debug.Log(EvolutionFirstStart().placePositionList);
    }

    [System.Serializable]
    private struct EvolutionData
    {
        public List<ActionData> placePositionList;
        public int score;

        [System.Serializable]
        public struct ActionData
        {
            public int blockType;
            public int blockRot;
            public Vector2Int placePositionList;

            public ActionData(int blockType, int blockRot, Vector2Int placePositionList)
            {
                this.blockType = blockType;
                this.blockRot = blockRot;
                this.placePositionList = placePositionList;
            }
        }

        public EvolutionData(List<ActionData> placePositionList, int score)
        {
            this.placePositionList = placePositionList;
            this.score = score;
        }
    }

    //json에다 달아두면 다시는 안씀
    private EvolutionData EvolutionFirstStart()
    {
        var mainLogicInstance = MainGameLogic.instance;
        List<Vector2Int> posData = new List<Vector2Int>();
        int nowPickBlockNum = 0;

        while (!mainLogicInstance.FailCheck())
        {
            Debug.Log(nowPickBlockNum);
            mainLogicInstance.PickBlockSet(BlockManager.instance.ingameCellBlocks[nowPickBlockNum]);

            Vector2 parseToTilePos;
            Vector2Int curVec;
            do
            {
                var randNumX = Mathf.RoundToInt(Random.Range(0, 10));
                var randNumY = Mathf.RoundToInt(Random.Range(0, 10));
                curVec = new Vector2Int(randNumX, randNumY);
                parseToTilePos = MainGameLogic.ChangePosToTilePos(curVec);
            } while (!mainLogicInstance.BlockDrop(parseToTilePos));

            nowPickBlockNum++;
            if (nowPickBlockNum >= 3)
                nowPickBlockNum = 0;

            posData.Add(curVec);
        }

        List<EvolutionData.ActionData> actionDataList = new List<EvolutionData.ActionData>();
        foreach (var curPos in posData)
        {
            var nowPickBlockData = mainLogicInstance.nowPickBlock;
            actionDataList.Add(new EvolutionData.ActionData(nowPickBlockData.blockNum, nowPickBlockData.rotNum,
                curPos));
        }

        var returnData = new EvolutionData(actionDataList, mainLogicInstance.score);
        return returnData;
    }
}
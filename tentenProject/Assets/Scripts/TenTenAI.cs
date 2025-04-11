using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
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
        var mapinLogicInstance = MainGameLogic.instance;
        BlockArrayLoad();
        BlockManager.instance.BlockRefill();

        Debug.Log(EvolutionFirstStart().placePositionList);
    }

    [System.Serializable]
    private struct EvolutionData
    {
        public List<ActionData> placePositionList;
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

        public EvolutionData(List<ActionData> placePositionList, ScoreInfo scoreInfo)
        {
            this.placePositionList = placePositionList;
            this.scoreInfo = scoreInfo;
        }
    }

    //json에다 달아두면 다시는 안씀
    private EvolutionData EvolutionFirstStart()
    {
        var mainLogicInstance = MainGameLogic.instance;
        List<EvolutionData.ActionData> actionData = new List<EvolutionData.ActionData>();

        while (!mainLogicInstance.FailCheck())
        {
            if (BlockManager.instance.ingameCellBlocks.Count <= 0)
                BlockManager.instance.BlockRefill();

            var curActionData = new EvolutionData.ActionData();
            Vector2 parseToTilePos;

            do
            {
                mainLogicInstance.PickBlockSet(BlockManager.instance.ingameCellBlocks
                    [Random.Range(0, BlockManager.instance.ingameCellBlocks.Count)]);

                var nowPickBlockData = mainLogicInstance.nowPickBlock;
                curActionData.blockType = nowPickBlockData.blockNum;
                curActionData.blockRot = nowPickBlockData.rotNum;

                var randNumX = Mathf.RoundToInt(Random.Range(0, 10));
                var randNumY = Mathf.RoundToInt(Random.Range(0, 10));
                curActionData.placePosition = new Vector2Int(randNumX, randNumY);
                parseToTilePos = MainGameLogic.ChangePosToTilePos(curActionData.placePosition);


                if (MainGameLogic.instance.FailCheck()) break;
            } while (!mainLogicInstance.BlockDrop(parseToTilePos));

            actionData.Add(curActionData);
        }

        var returnData = new EvolutionData(actionData, mainLogicInstance.scoreInfo);
        return returnData;
    }
}
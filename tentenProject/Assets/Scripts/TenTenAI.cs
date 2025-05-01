using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class TenTenAI : MonoBehaviour
{
    public static TenTenAI instance;
    [SerializeField] private int evolutionRepeatCount = 30;
    public int generationCount = 0;
    private List<EvolutionData> curEvolutionData;

    private MainGameLogic mainLogicInstance;
    private BlockManager blockManagerInstance;
    private const string SAVE_PATH = "EvolutionFolder";


    [SerializeField] private int generateCount;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainLogicInstance = MainGameLogic.instance;
        blockManagerInstance = BlockManager.instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            mainLogicInstance.GameReset();
            Debug.Log($"Generation #{generationCount}");
            EvolutionLearning();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            mainLogicInstance.GameReset();
            Debug.Log("Show Best Generation");
            StartCoroutine(PlayBestGeneration());
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            mainLogicInstance.GameReset();
            Debug.Log($"Load Generate #{generateCount}");
            LoadEvolutionData();
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

    private string JsonFileSave(string fileName, string path)
    {
        var fileStream = new StreamReader(Application.dataPath + $@"\{path}\{fileName}.json");
        var jsonStr = fileStream.ReadToEnd();
        fileStream.Close();

        return jsonStr;
    }

    private IEnumerator PlayBestGeneration()
    {
        var bestEvolution = curEvolutionData[0];

        foreach (var action in bestEvolution.actionDataList)
        {
            blockManagerInstance.BlockPick(action.selectBlockOrderNum);

            var isPlaced = mainLogicInstance.TryBlockDrop(action.placePosition);

            if (!isPlaced)
            {
                EditorApplication.isPaused = true;
            }

            yield return new WaitForSeconds(0.1f);
        }
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

    /*
        행동마다 오른 스코어를 기록
            스코어가 오르는 경우:
            BlockDrop후 빈셀의 총합   (셀 당 +10)
            설치한 블럭의 셀의 갯수   (셀당 +100)
            한줄 지움   (+10000)
    */
    /*
     진화 규칙
        행동당 돌연변이 확률 5%
        교차 기반 후 순위대로 상위 3개 유전자를 베이스기반으로 하여 모델 30개 생성
            - 더하여 5개 순수 돌연변이 모델 생성
            - 상위 3개 유전자는 변형없이 다음세대로
            - 유전자 생성중 진행이 불가능할경우 돌연변이로 대체
     */
    private void EvolutionLearning()
    {
        for (int j = 0; j < generateCount; j++)
        {
            if (curEvolutionData == null)
            {
                curEvolutionData = EvolutionGenerateFirst();
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(CrossEvolutionProcess(curEvolutionData[0], curEvolutionData[1]));
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(CrossEvolutionProcess(curEvolutionData[1], curEvolutionData[2]));
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(CrossEvolutionProcess(curEvolutionData[0], curEvolutionData[2]));
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(CrossEvolutionProcess(curEvolutionData[2], curEvolutionData[0]));
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(CrossEvolutionProcess(curEvolutionData[1], curEvolutionData[0]));
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(CrossEvolutionProcess(curEvolutionData[2], curEvolutionData[1]));
                for (int i = 0; i < 5; i++)
                    curEvolutionData.Add(EvolutionAllRandomGenerate());
            }

            EvolutionApply(curEvolutionData);
        }

        SaveEvolutionData();
    }

    private DisposableList<EvolutionData> EvolutionGenerateFirst()
    {
        var returnData = DisposableList<EvolutionData>.Get();
        for (int i = 0; i < evolutionRepeatCount; i++)
        {
            var randomEvolutionData = EvolutionAllRandomGenerate();
            returnData.Add(randomEvolutionData);
        }

        return returnData;
    }

    private EvolutionData CrossEvolutionProcess(EvolutionData evolutionData1, EvolutionData evolutionData2)
    {
        var actionDataList1 = evolutionData1.actionDataList.ToList();
        var actionDataList2 = evolutionData2.actionDataList.ToList();
        var returnEvolutionData = new EvolutionData();

        int crossIndex;
        do
        {
            crossIndex = Random.Range(1, actionDataList1.Count);
        } while (actionDataList2.Count < crossIndex || (crossIndex + 1) % 3 != 0);

        actionDataList1.RemoveRange(crossIndex, actionDataList1.Count - crossIndex - 1);
        actionDataList2.RemoveRange(0, crossIndex);
        actionDataList1.AddRange(actionDataList2);

        var newActionDataList = actionDataList1;
        var resultActionDataList = new List<EvolutionData.ActionData>();

        foreach (var actionData in newActionDataList)
        {
            while (!mainLogicInstance.IsFail())
            {
                var curActionData = actionData;
                var prevScore = mainLogicInstance.scoreInfo.score;

                blockManagerInstance.BlockPick(actionData.selectBlockOrderNum);

                if (actionData.selectBlockOrderNum > blockManagerInstance.ingameCellBlocks.Count ||
                    !mainLogicInstance.TryBlockDrop(actionData.placePosition) || Random.Range(0, 100) <= 5)
                {
                    var randomAction = PlaceBlockToRandomValue();
                    curActionData = randomAction;
                }

                curActionData.earnScoreValue = mainLogicInstance.scoreInfo.score - prevScore;

                resultActionDataList.Add(curActionData);
            }
        }

        returnEvolutionData.actionDataList = resultActionDataList.ToArray();
        returnEvolutionData.scoreInfo = mainLogicInstance.scoreInfo;
        mainLogicInstance.GameReset();

        return returnEvolutionData;
    }


    private void SaveEvolutionData()
    {
        var jsonData = new EvolutionDataListJson(curEvolutionData);
        var json = JsonUtility.ToJson(jsonData);
        var fileName = $"GenerateData #{generationCount}";

        JsonFileSave(json, fileName, SAVE_PATH);
    }

    private void LoadEvolutionData()
    {
        var fileName = $"GenerateData #{generateCount}";

        var jsonData = JsonFileSave(fileName, SAVE_PATH);
        var data = JsonUtility.FromJson<EvolutionDataListJson>(jsonData);
        curEvolutionData = data.evolutionData.ToList();
        generationCount = generateCount;
    }

    private void EvolutionApply(List<EvolutionData> evolutionDataList)
    {
        using var list = DisposableList<EvolutionData>.Get();

        var orderList = evolutionDataList.OrderByDescending(x => x.scoreInfo.score);
        foreach (var evolutionData in orderList)
        {
            list.Add(evolutionData);

            if (list.Count >= 3)
                break;
        }

        curEvolutionData.Clear();
        curEvolutionData.AddRange(list);

        generationCount++;
        Debug.Log(curEvolutionData[0].scoreInfo.score);
    }

    private EvolutionData EvolutionAllRandomGenerate()
    {
        List<EvolutionData.ActionData> actionData = new List<EvolutionData.ActionData>();

        while (!mainLogicInstance.IsFail())
        {
            EvolutionData.ActionData curActionData;
            var prevScore = mainLogicInstance.scoreInfo.score;

            curActionData = PlaceBlockToRandomValue();

            curActionData.earnScoreValue = mainLogicInstance.scoreInfo.score - prevScore;
            actionData.Add(curActionData);
        }

        var returnData = new EvolutionData(actionData, mainLogicInstance.scoreInfo);
        mainLogicInstance.GameReset();

        return returnData;
    }

    private EvolutionData.ActionData PlaceBlockToRandomValue()
    {
        EvolutionData.ActionData returnValue = new EvolutionData.ActionData();
        do
        {
            var selectIndex = Random.Range(0, blockManagerInstance.ingameCellBlocks.Count);
            var selectBlock = blockManagerInstance.ingameCellBlocks[selectIndex];
            mainLogicInstance.PickBlockSet(selectBlock);

            returnValue.blockInfo = selectBlock.blockInfo;
            returnValue.selectBlockOrderNum = selectIndex;

            returnValue.placePosition = GetRandomTile();
        } while (!MainGameLogic.instance.TryBlockDrop(returnValue.placePosition));

        return returnValue;
    }

    private Vector2Int GetRandomTile()
    {
        var randNumX = Mathf.RoundToInt(Random.Range(0, 10));
        var randNumY = Mathf.RoundToInt(Random.Range(0, 10));
        return new Vector2Int(randNumX, randNumY);
    }

    [System.Serializable]
    private struct EvolutionDataListJson
    {
        public EvolutionData[] evolutionData;

        public EvolutionDataListJson(List<EvolutionData> evolutionData)
        {
            this.evolutionData = evolutionData.ToArray();
        }
    }

    [System.Serializable]
    private struct EvolutionData
    {
        public ActionData[] actionDataList;
        public ScoreInfo scoreInfo;

        public EvolutionData(List<ActionData> actionDataList, ScoreInfo scoreInfo)
        {
            this.actionDataList = actionDataList.ToArray();
            this.scoreInfo = scoreInfo;
        }

        [System.Serializable]
        public struct ActionData
        {
            public BlockInfo blockInfo;
            public int selectBlockOrderNum;
            public int earnScoreValue;

            public Vector2Int placePosition;

            public ActionData(int selectBlockOrderNum, int blockType, int blockRot, Vector2Int placePosition)
            {
                this.selectBlockOrderNum = selectBlockOrderNum;
                this.placePosition = placePosition;
                blockInfo = new BlockInfo(blockType, blockRot);
                earnScoreValue = 0;
            }
        }
    }

    [System.Serializable]
    private struct BlockInfoForSave
    {
        public List<BlockInfo> blockInfos;

        public BlockInfoForSave(Queue<BlockInfo> blockInfos)
        {
            this.blockInfos = blockInfos.ToList();
        }
    }
}
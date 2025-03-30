using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class TenTenAI : MonoBehaviour
{
    public static TenTenAI instance;

    private void Awake()
    {
        instance = this;
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
            BlockManager.instance.blockQueue.Enqueue(blockInfo);
        }
    }

    private void EvoultionLearning()
    {
        var mainLogicInstance = MainGameLogic.instance;
        
        mainLogicInstance.
    }
}
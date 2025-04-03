using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public List<CellBlock> ingameCellBlocks;
    public Transform[] blockParent = new Transform[3];

    public Queue<BlockInfo> blockQueue = new Queue<BlockInfo>();
    private List<BlockInfo> blockPool = new List<BlockInfo>();
    [System.Serializable]
    public struct BlockInfo
    {
        public int blockNum;
        public int rotNum;

        public BlockInfo(int blockNum, int rotNum)
        {
            this.blockNum = blockNum;
            this.rotNum = rotNum;
        }
    }

    [SerializeField] private List<CellBlock> curCellBlockPool;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
            TenTenAI.instance.BlockArrayLoad();
    }

    private void Update()
    {
        if (ingameCellBlocks.Count <= 0)
            BlockRefill();
    }

    private void BlockRefill()
    {
        ingameCellBlocks.Clear();
        foreach (var curParent in blockParent)
        {
            //if (blockPool.Count <= 0)
            //    BlockQueueRefill();

            var curBlock = blockQueue.Dequeue();
            var obj = Instantiate(curCellBlockPool[curBlock.blockNum], curParent);
            
            obj.rotNum = curBlock.rotNum;
            obj.transform.Rotate(new Vector3(0, 0, obj.rotNum * 90));
            ingameCellBlocks.Add(obj);
        }
    }

    private void BlockQueueRefill()
    {
        blockPool.Clear();

        blockPool.Add(new BlockInfo(0, 0));
        blockPool.Add(new BlockInfo(0, 0));
        for (int i = 1; i < curCellBlockPool.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var curBlock = new BlockInfo(i, j);
                blockPool.Add(curBlock);
            }
        }

        Queue<BlockInfo> curQueue = new Queue<BlockInfo>();
        while (blockPool.Count > 0)
        {
            var randNum = Random.Range(0, blockPool.Count);
            curQueue.Enqueue(blockPool[randNum]);
            blockPool.RemoveAt(randNum);
        }
        blockQueue = curQueue;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public List<BlockInfo> blockPool = new List<BlockInfo>();
    public List<CellBlock> ingameCellBlocks;
    public Transform[] blockParent = new Transform[3];

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
            if (blockPool.Count <= 0)
                BlockQueueRefill();
            
            var randNum = Random.Range(0, blockPool.Count);
            var curBlockInfo = blockPool[randNum];
            blockPool.RemoveAt(randNum);
            
            var obj = Instantiate(curCellBlockPool[curBlockInfo.blockNum], curParent);
            obj.rotNum = curBlockInfo.rotNum;
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
    }
}
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public List<CellBlock> ingameCellBlocks;
    public Transform[] blockParent = new Transform[3];

    public Queue<BlockInfo> blockQueue = new Queue<BlockInfo>();
    private List<BlockInfo> blockPool = new List<BlockInfo>();

    [SerializeField] private List<CellBlock> curCellBlockPool;

    private void Awake()
    {
        instance = this;
    }

    public void BlockRefill()
    {
        ingameCellBlocks.Clear();
        foreach (var curParent in blockParent)
        {
            if (blockQueue.Count <= 0)
            {
                TenTenAI.instance.BlockArrayLoad();
            }

            var curBlock = blockQueue.Dequeue();
            var obj = Instantiate(curCellBlockPool[curBlock.blockNum], curParent);

            obj.blockInfo = curBlock;
            obj.transform.Rotate(new Vector3(0, 0, obj.blockInfo.rotNum * 90));
            ingameCellBlocks.Add(obj);
        }
    }

    public void BlockPick(int pickIndex)
    {
        int index = Mathf.Min(pickIndex, ingameCellBlocks.Count - 1);
        MainGameLogic.instance.PickBlockSet(ingameCellBlocks[index]);
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
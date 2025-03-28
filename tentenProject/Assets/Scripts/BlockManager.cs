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
     public List<CellBlock> blockPool = new List<CellBlock>();
    public List<CellBlock> ingameCellBlocks;
    public Transform[] blockParent = new Transform[3];

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
            var block = blockPool[Random.Range(0, blockPool.Count)];
            var obj = Instantiate(block, curParent);
            Debug.Log(obj.rotNum);
            obj.transform.Rotate(new Vector3(0, 0, obj.rotNum * 90));
            ingameCellBlocks.Add(obj);
        }
    }

    private void BlockQueueRefill()
    {
        blockPool.Clear();

        blockPool.Add(curCellBlockPool[0]);
        blockPool.Add(curCellBlockPool[0]);
        for (int i = 1; i < curCellBlockPool.Count; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                var curBlock = Instantiate( curCellBlockPool[i]);
                curBlock.rotNum = j;
                blockPool.Add(curBlock);
            }
        }
    }
}
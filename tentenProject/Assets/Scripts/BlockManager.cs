using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;
    public List<CellBlock> cellBlockPool;
    public List<CellBlock> curCellBlocks;
    public Transform[] blockParent = new Transform[3];

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (curCellBlocks.Count <= 0)
            BlockRefill();
    }

    public void BlockRefill()
    {
        curCellBlocks.Clear();
        foreach (var curParent in blockParent)
        {
            var randNum = Random.Range(0, cellBlockPool.Count);
            curCellBlocks.Add(Instantiate(cellBlockPool[randNum], curParent));
        }
    }
}
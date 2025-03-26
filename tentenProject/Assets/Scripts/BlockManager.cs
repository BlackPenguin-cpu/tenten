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
            var obj = Instantiate(cellBlockPool[randNum], curParent);
            obj.rotNum = Random.Range(0, 4);
            obj.transform.Rotate(new Vector3(0, 0, obj.rotNum * 90));
            curCellBlocks.Add(obj);
        }
    }
}
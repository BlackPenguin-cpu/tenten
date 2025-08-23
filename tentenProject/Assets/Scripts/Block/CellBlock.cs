using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellBlockDescInfo
{
    public string name;
    public string desc;
    public string blockInfos;
}

public class CellBlock : MonoBehaviour
{
    public Color curColor;
    public BlockInfo blockInfo;
    public List<Block> cells;
    public BlockInfoSO blockInfoSO;

    [SerializeField] private GameObject cellParent;

    private void Awake()
    {
        GetCells();
    }

    private void Start()
    {
        SetBlockColor();
    }


    private void SetBlockColor()
    {
        if (curColor == Color.white)
            SetRandomColor();

        BlockColorChange();
    }

    private void SetRandomColor()
    {
        curColor = Random.ColorHSV(0.5f, 1, 1, 1, 0.5f, 1);
    }

    public List<Vector2Int> GetThisBlockState()
    {
        var cols = cellParent.GetComponentsInChildren<BoxCollider2D>();

        return cols.Select(col => TileMapManager.ChangeCellPosToPos(col.transform.localPosition))
            .ToList();
    }

    public CellBlockDescInfo GetBlockDesc()
    {
        var returnDesc = new CellBlockDescInfo();

        returnDesc.name = blockInfoSO.name;
        returnDesc.desc = blockInfoSO.description;

        var infoStr = "";
        var curCells = cells.Select(cell => cell.Behaviour).ToList();

        var blocks = curCells.Distinct().ToList();
        infoStr += blocks.Select(info => $"{info.name} : {info.description}\n");

        returnDesc.blockInfos = infoStr;
        
        return returnDesc;
    }

    [ContextMenu("GetCells")]
    private void GetCells()
    {
        cells = new List<Block>();
        for (int i = 0; i < cellParent.transform.childCount; i++)
        {
            cells.Add(cellParent.transform.GetComponent<Block>());
        }
    }

    [ContextMenu("ApplyCustomColor")]
    private void BlockColorChange()
    {
        var renderers = cellParent.GetComponentsInChildren<SpriteRenderer>();
        foreach (var curRen in renderers)
        {
            curRen.color = curColor;
        }
    }
}
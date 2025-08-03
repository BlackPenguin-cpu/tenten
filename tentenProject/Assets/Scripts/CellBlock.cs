using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellBlock : MonoBehaviour
{
    public Color curColor;
    public BlockInfo blockInfo;
    public List<GameObject> cells = new List<GameObject>();

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

    [ContextMenu("GetCells")]
    private void GetCells()
    {
        cells = new List<GameObject>();
        for (int i = 0; i < cellParent.transform.childCount; i++)
        {
            cells.Add(cellParent.transform.GetChild(i).gameObject);
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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellBlock : MonoBehaviour
{
    public Color curColor;

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
        var cols = GetComponentsInChildren<BoxCollider2D>();

        return  cols.Select(col => MainGameLogic.ChangeBlockPosToPos(col.transform.localPosition, transform.eulerAngles.z)).ToList();
    }

    [ContextMenu("ApplyCustomColor")]
    private void BlockColorChange()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var curRen in renderers)
        {
            curRen.color = curColor;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellBlock : MonoBehaviour
{
    public Color curColor;

    private void Start()
    {
        if (curColor == Color.white)
            curColor = Random.ColorHSV(0.5f, 1, 1, 1, 0.5f, 1);

        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.color = curColor;
        }
    }

    public List<Vector2Int> GetThisBlockState()
    {
        var returnValue = new List<Vector2Int>();
        var cols = GetComponentsInChildren<BoxCollider2D>();
        foreach (var col in cols)
        {
            var pos = MainGameLogic.ChangeTilePosToPos(col.transform.localPosition,true);
            returnValue.Add(pos);
        }

        return returnValue;
    }

    [ContextMenu("ApplyCustomColor")]
    private void ColorChange()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.color = curColor;
        } 
    }
}
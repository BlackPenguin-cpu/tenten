using System;
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
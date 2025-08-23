using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;


public enum EBlockCellRank
{
    D,
    C,
    B,
    A,
    S
}

public enum EBlockType
{
    Survival,
    Score
}

public abstract class BlockBase : ScriptableObject
{
    public Sprite icon;
    public GameObject onClearEffect;
    public string cellName;
    public string description;
    public float baseScore = 50f;

    public abstract IBlockRuntime CreateRuntime();

    // public abstract void OnPlaced(Vector2Int pos);
    // public abstract void OnTurnPassed(Vector2Int pos);
    // public abstract UniTask OnClear(Vector2Int pos);
    // public abstract float GetScoreMultiply();
}
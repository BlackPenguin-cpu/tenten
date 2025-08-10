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
    public EBlockCellRank cellRank;
    public EBlockType cellType;
    public float baseScore = 50f;
    
    public abstract void OnPlaced(List<Block> cellList,Vector2Int cellPos);
    public abstract void OnTurnPassed(List<Block> cellList,Vector2Int cellPos);
    public abstract UniTask OnClear(List<Block> cellList,Vector2Int cellPos);
    public abstract float GetScoreMultiply(Block block);
    
}
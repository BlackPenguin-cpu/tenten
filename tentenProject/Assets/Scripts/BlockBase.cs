using UnityEngine;


public enum EBlockCellRank
{
    D,
    C,
    B,
    A,
    S
}

public abstract class BlockBase : ScriptableObject
{
    public Sprite icon;
    public string cellName;
    public string description;
    public EBlockCellRank cellRank;
    
    public abstract void OnPlaced(Block block);
    public abstract void OnTurnPassed(Block block);
    public abstract void OnClear(Block block);
    public abstract float GetScoreMultiply(Block block);
}
using UnityEngine;


public enum EBlockCellRank
{
    D,
    C,
    B,
    A,
    S
}

public abstract class BlockBase : MonoBehaviour
{
    public Sprite icon;
    public string cellName;
    public string description;
    public EBlockCellRank cellRank;
}
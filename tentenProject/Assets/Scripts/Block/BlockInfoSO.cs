using UnityEngine;

[CreateAssetMenu(menuName = "Block/BlockInfo")]
public class BlockInfoSO : ScriptableObject
{
    public string cellName;
    public string description;
    public EBlockCellRank cellRank;
    public EBlockType cellType;
}

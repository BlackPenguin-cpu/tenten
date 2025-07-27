using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/GoldenBlock")]
public class GoldenBlock : BlockBase
{
    public override void OnPlaced(Block block)
    {
        
    }

    public override void OnTurnPassed(Block block)
    {
    }

    public override void OnClear(Block block)
    {
    }

    public override float GetScoreMultiply(Block block)
    {
        return 2;
    }
}

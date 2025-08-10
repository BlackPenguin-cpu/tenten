using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/GoldenBlock")]
public class GoldenBlock : BlockBase
{
    public override void OnPlaced(List<Block> cellList, Vector2Int cellPos)
    {
        
    }

    public override void OnTurnPassed(List<Block> cellList, Vector2Int cellPos)
    {
    }

    public override async UniTask OnClear(List<Block> cellList, Vector2Int cellPos)
    {
        await UniTask.Delay(10);
    }

    public override float GetScoreMultiply(Block block)
    {
        return 2;
    }
}

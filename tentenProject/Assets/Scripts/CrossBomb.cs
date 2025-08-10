using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/CrossBomb")]
public class CrossBomb : BlockBase
{
    public override void OnPlaced(List<Block> cellList, Vector2Int cellPos)
    {
    }

    public override void OnTurnPassed(List<Block> cellList, Vector2Int cellPos)
    {
    }

    public override async UniTask OnClear(List<Block> blocks, Vector2Int cellPos)
    {
        List<Vector2Int> posList = new List<Vector2Int>();
        var xPos = cellPos.x;
        var yPos = cellPos.y;
        int targetPos = 0;
        while (targetPos < 10)
        {
            posList.Add(new Vector2Int(targetPos, yPos));
            targetPos++;
        }

        targetPos = 0;
        while (targetPos < 10)
        {
            posList.Add(new Vector2Int(xPos, targetPos));
            targetPos++;
        }

        var distinctPosList = posList.Distinct().ToList();
        distinctPosList.Remove(new Vector2Int(xPos, yPos));
        
        await UniTask.Delay(100);
        Instantiate(onClearEffect, TileMapManager.ChangePosToTilePos(new Vector2Int(xPos, yPos)), Quaternion.identity);
        await MainGameLogic.instance.BlockClear(distinctPosList);
    }


    public override float GetScoreMultiply(Block block)
    {
        return 1;
    }
}
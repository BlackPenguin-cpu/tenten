using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Block/Behavior/SquareBomb")]
public class SquareBomb : BlockBase
{
    public override void OnPlaced(List<Block> cellList, Vector2Int cellPos)
    {
    }

    public override void OnTurnPassed(List<Block> cellList, Vector2Int cellPos)
    {
    }

    public override async UniTask OnClear(List<Block> blocks, Vector2Int cellPos)
    {
        var posList = new List<Vector2Int>();
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                posList.Add(new Vector2Int(cellPos.x + i, cellPos.y + j));
            }
        }

        var distinctPosList = posList.Distinct().ToList();
        distinctPosList.Remove(new Vector2Int(cellPos.x, cellPos.y));


        await UniTask.Delay(100);
        Instantiate(onClearEffect, TileMapManager.ChangePosToTilePos(new Vector2Int(cellPos.x, cellPos.y)),
            Quaternion.identity);
        await MainGameLogic.instance.BlockClear(distinctPosList);
    }


    public override float GetScoreMultiply(Block block)
    {
        return 1;
    }
}
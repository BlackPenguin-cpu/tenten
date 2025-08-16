using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RuntimeSquareBomb : IBlockRuntime
{
    private readonly SquareBomb data;

    public RuntimeSquareBomb(SquareBomb data)
    {
        this.data = data;
    }

    public void OnPlaced(Vector2Int pos)
    {
    }

    public void OnTurnPassed(Vector2Int pos)
    {
    }

    public async UniTask OnClear(Vector2Int pos)
    {
        var posList = new List<Vector2Int>();
        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                posList.Add(new Vector2Int(pos.x + i, pos.y + j));
            }
        }

        var distinctPosList = posList.Distinct().ToList();
        distinctPosList.Remove(new Vector2Int(pos.x, pos.y));


        await UniTask.Delay(100);
        Object.Instantiate(data.onClearEffect, TileMapManager.ChangePosToTilePos(new Vector2Int(pos.x, pos.y)),
            Quaternion.identity);
        await MainGameLogic.instance.BlockClear(distinctPosList);
    }


    public float GetScoreMultiply()
    {
        return 1;
    }
}
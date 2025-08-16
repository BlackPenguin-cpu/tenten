using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RuntimeCrossBomb : IBlockRuntime
{
    private readonly CrossBomb data;

    public RuntimeCrossBomb(CrossBomb data)
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
        List<Vector2Int> posList = new List<Vector2Int>();
        var xPos = pos.x;
        var yPos = pos.y;
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

        Object.Instantiate(data.onClearEffect, TileMapManager.ChangePosToTilePos(new Vector2Int(xPos, yPos)),
            Quaternion.identity);
        await MainGameLogic.instance.BlockClear(distinctPosList);
        await UniTask.Delay(400);
    }


    public float GetScoreMultiply()
    {
        return 1;
    }
}
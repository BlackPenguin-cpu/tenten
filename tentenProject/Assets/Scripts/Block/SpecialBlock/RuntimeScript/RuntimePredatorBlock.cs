using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RuntimePredatorBlock : IBlockRuntime, IBlockHaveCleanEffect
{
    private readonly PredatorBlock data;
    private TMPro.TextMeshPro instText;
    private float predatorCount = 1;

    public RuntimePredatorBlock(PredatorBlock data)
    {
        this.data = data;
    }

    public void OnPlaced(Vector2Int pos)
    {
        var realPos = TileMapManager.ChangePosToTilePos(pos);
        instText = Object.Instantiate(data.multiplyText, realPos, Quaternion.identity);
    }

    public void OnTurnPassed(Vector2Int pos)
    {
        var returnList = new List<Vector2Int>();
        int randX = 0;
        int randY = 0;
        do
        {
            randX = Random.Range(-1, 2);
            randY = Random.Range(-1, 2);
        } while (randX == 0 && randY == 0
                 || randX + pos.x >= 10 || randY + pos.y >= 10
                 || randX + pos.x < 0 || randY + pos.y < 0);

        returnList.Add(new Vector2Int(pos.x + randX, pos.y + randY));

        if (MainGameLogic.instance.CellInfos[returnList[0].x, returnList[0].y].isBlockPlaced == true)
        {
            predatorCount *= 2;
            UIManager.TextAnim(instText, $"{predatorCount}x", 0.3f);
            Object.Instantiate(data.predatorText, TileMapManager.ChangePosToTilePos(returnList[0]),
                Quaternion.identity);
            MainGameLogic.instance.BlockClear(returnList, 0);
        }
    }

    public async UniTask OnClear(Vector2Int pos)
    {
        EffectClean();
        await UniTask.CompletedTask;
    }

    public float GetScoreMultiply() => predatorCount;

    public void EffectClean()
    {
        Object.Destroy(instText?.gameObject);
    }
}
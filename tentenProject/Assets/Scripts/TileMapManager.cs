using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Task = System.Threading.Tasks.Task;

public class TileMapManager : MonoBehaviour
{
    public List<Block> cellList;

    private void Start()
    {
        StartInitialize();
    }

    public void OnTurnPassAction()
    {
        foreach (var cell in cellList)
        {
            cell.OnTurnPass(cellList, ChangeTilePosToPos(cell.transform.position));
        }
    }

    private async UniTaskVoid StartInitialize()
    {
        var originScale = cellList[0].transform.localScale;
        foreach (var cell in cellList)
        {
            cell.transform.localScale = new Vector3(0, 0, 0);
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                cellList[(i - j) * 10 + j].transform.DOScale(originScale, 0.1f);
            }

            await Task.Delay(10);
        }

        int targetNum = 0;
        for (int i = 0; i < 10; i++)
        {
            targetNum = 90;
            while (targetNum > 0)
            {
                cellList[targetNum + i].transform.DOScale(originScale, 0.1f);
                targetNum -= 9;
            }

            await Task.Delay(10);
        }
    }

    public static Vector2 ChangePosToTilePos(Vector2Int pos)
    {
        var startXPos = -2.5f;
        var startYPos = 2.5f;
        var xyPos = 0.55f;

        return new Vector2(startXPos + pos.x * xyPos, startYPos + pos.y * -xyPos);
    }

    public static Vector2 ChangePosToCellPos(Vector2Int pos)
    {
        var xyPos = 0.55f;

        return new Vector2(pos.x * xyPos, pos.y * -xyPos);
    }

    public static Vector2Int ChangeCellPosToPos(Vector2 pos)
    {
        var xyPos = 0.55f;

        return new Vector2Int(Mathf.RoundToInt(pos.x / xyPos), Mathf.RoundToInt(pos.y / -xyPos));
    }

    public static Vector2Int ChangeBlockPosToPos(Vector2 blockPos, int rotNum)
    {
        var xyTilePos = 0.55f;
        var xPos = Mathf.RoundToInt((blockPos.x / xyTilePos));
        var yPos = Mathf.RoundToInt(blockPos.y / xyTilePos);
        Vector2Int newPos = new Vector2Int(xPos, yPos);


        switch (rotNum)
        {
            case 1:
                newPos.y = xPos;
                newPos.x = -yPos;
                break;
            case 3:
                newPos.y = -xPos;
                newPos.x = yPos;
                break;
            case 2:
                newPos.x = -xPos;
                newPos.y = -yPos;
                break;
        }

        return new Vector2Int(newPos.x, -newPos.y);
    }

    public static Vector2Int ChangeTilePosToPos(Vector2 tilePos)
    {
        var startTileXPos = -2.5f;
        var startTileYPos = 2.5f;

        var xyTilePos = 0.55f;

        var xPos = Mathf.RoundToInt(((tilePos.x - startTileXPos) / xyTilePos));
        var yPos = Mathf.RoundToInt((tilePos.y - startTileYPos) / xyTilePos);

        return new Vector2Int(Mathf.Abs(xPos), Mathf.Abs(yPos));
    }
}
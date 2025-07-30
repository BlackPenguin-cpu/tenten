using UnityEngine;
using System.Collections.Generic;

public class TileMapManager : MonoBehaviour
{
    public List<Block> cellList;

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
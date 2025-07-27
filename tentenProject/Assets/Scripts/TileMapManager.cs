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

    public static Vector2Int ChangeBlockPosToPos(Vector2 blockPos, int rotNum)
    {
        var xyTilePos = 0.55f;
        var xPos = Mathf.RoundToInt((blockPos.x / xyTilePos));
        var yPos = Mathf.RoundToInt(blockPos.y / xyTilePos);
        Vector2Int newPos = new Vector2Int(xPos, yPos);


        if (rotNum == 1)
        {
            newPos.y = xPos;
            newPos.x = -yPos;
        }

        if (rotNum == 3)
        {
            newPos.y = -xPos;
            newPos.x = yPos;
        }

        if (rotNum == 2)
        {
            newPos.x = -xPos;
            newPos.y = -yPos;
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

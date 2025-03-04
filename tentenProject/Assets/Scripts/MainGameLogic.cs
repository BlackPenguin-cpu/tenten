using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainGameLogic : MonoBehaviour
{
    [SerializeField] private GameObject tileMap;

    private BoxCollider2D[,] cells = new BoxCollider2D[10, 10];

    public bool[,] isBlockPlaced = new bool[10, 10];

    private void Start()
    {
        foreach (var col in tileMap.GetComponentsInChildren<BoxCollider2D>())
        {
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    cells[j, i] = col;
                }
            }
        }
    }

    public static Vector2 ChangePosToTilePos(int pos)
    {
        var startXPos = -2.5f;
        var startYPos = 2.5f;
        var xyPos = 0.55f;

        var xPos = pos % 10;
        var yPos = pos / 10;
        
        return new Vector2(startXPos + xPos * xyPos, startYPos+ yPos * xyPos); 
    }
    public static int ChangeTilePosToPos(Vector2 tilePos)
    {
        var startTileXPos = -2.5f;
        var startTileYPos = 2.5f;
        var xyTilePos = 0.55f;

        var xPos = (int)((tilePos.x - startTileXPos) / xyTilePos);
        var yPos = (int)((tilePos.y - startTileYPos) / xyTilePos);
        
        return  (yPos * 10 + xPos); 
    }
}
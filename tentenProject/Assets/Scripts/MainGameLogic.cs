using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public struct ScoreInfo
{
    public int score;
    public int lineClearCount;
    public int cellPlaceCount;
    public int blockPlaceCount;
}

public class MainGameLogic : MonoBehaviour
{
    public static MainGameLogic instance;
    [SerializeField] private GameObject tileMap;

    private CellInfo[,] cellInfos;

    public CellInfo[,] CellInfos
    {
        get
        {
            if (cellInfos == null)
            {
                cellInfos = new CellInfo[10, 10];
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        cellInfos[i, j] = new CellInfo();
                        cellInfos[i, j].cellCol = tileMapManager.cellList[i + j * 10];
                    }
                }
            }

            return cellInfos;
        }
        set { cellInfos = value; }
    }

    public TileMapManager tileMapManager;
    public ScoreInfo scoreInfo;

    public class CellInfo
    {
        public BoxCollider2D cellCol;
        public bool isBlockPlaced = false;
    }

    public CellBlock nowPickBlock { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverImg;


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        return;
        if (Input.GetMouseButtonDown(0))
            BlockPicking();
        if (nowPickBlock != null)
        {
            if (Input.GetMouseButtonUp(0))
                BlockDrop();
            if (Input.GetMouseButton(0))
                BlockMoving();
        }
    }

    private void BlockMoving()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        nowPickBlock.transform.position = pos;
    }

    private void BlockPicking()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var col = Physics2D.Raycast(pos, Vector3.forward, 100, 1 << LayerMask.NameToLayer("CellBlock"));

        if (!col.collider || !col.collider.GetComponentInParent<CellBlock>()) return;
        PickBlockSet(col.collider.GetComponentInParent<CellBlock>());
        nowPickBlock.transform.DOScale(2, 0.1f).SetEase(Ease.InOutBounce);
    }

    public void PickBlockSet(CellBlock block)
    {
        nowPickBlock = block;
    }

    private void BlockDrop()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        BlockDrop(pos);
    }

    public bool BlockDrop(Vector2 pos)
    {
        var col = Physics2D.Raycast(pos, Vector3.forward, 100, 1 << LayerMask.NameToLayer("Tilemap"));

        if (nowPickBlock == null) return false;
        if (col.collider == null)
        {
            nowPickBlock?.transform.DOKill();
            CurBlockReset();
            return false;
        }

        var vec = ChangeTilePosToPos(col.transform.position);
        var blockVecList = nowPickBlock.GetThisBlockState();
        List<CellInfo> infos = new List<CellInfo>();

        foreach (var blockVec in blockVecList)
        {
            var targetPos = vec + blockVec;

            if (targetPos.x >= 10 || targetPos.y >= 10 || targetPos.x < 0 || targetPos.y < 0
                || CellInfos[targetPos.x, targetPos.y].isBlockPlaced == true)
            {
                CurBlockReset();
                return false;
            }

            infos.Add(CellInfos[targetPos.x, targetPos.y]);
        }

        foreach (var info in infos)
        {
            info.isBlockPlaced = true;
            scoreInfo.score += 100;
            scoreInfo.cellPlaceCount++;
            info.cellCol.GetComponent<SpriteRenderer>().color = nowPickBlock.curColor;
        }

        BlockManager.instance.ingameCellBlocks.Remove(nowPickBlock);
        Destroy(nowPickBlock.gameObject);
        nowPickBlock = null;
        scoreInfo.blockPlaceCount++;

        BlockClearCheck();
        FailCheck();

        UIManager.instance.InfoApply(scoreInfo);
        
        return true;
    }

    private void BlockClearCheck()
    {
        var horClearBlockList = new List<CellInfo>();
        var verClearBlockList = new List<CellInfo>();
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (CellInfos[j, i].isBlockPlaced == true)
                {
                    horClearBlockList.Add(CellInfos[j, i]);
                }

                if (CellInfos[i, j].isBlockPlaced == true)
                {
                    verClearBlockList.Add(CellInfos[i, j]);
                }
            }

            if (horClearBlockList.Count >= 10)
            {
                BlockClear(horClearBlockList);
                BlockClearCheck();
            }

            if (verClearBlockList.Count >= 10)
            {
                BlockClear(verClearBlockList);
                BlockClearCheck();
            }

            horClearBlockList.Clear();
            verClearBlockList.Clear();
        }
    }

    public bool FailCheck()
    {
        var blockList = BlockManager.instance.ingameCellBlocks;
        bool isBlockPlacedImpossible = false;
        foreach (var block in blockList)
        {
            if (block == null) continue;
            var blockStates = block.GetThisBlockState();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    foreach (var vec in blockStates)
                    {
                        var targetPos = new Vector2Int(i, j) + vec;
                        if (targetPos.x >= 10 || targetPos.y >= 10 || targetPos.x < 0 || targetPos.y < 0
                            || CellInfos[targetPos.x, targetPos.y].isBlockPlaced == true)
                        {
                            isBlockPlacedImpossible = true;
                            break;
                        }
                        else
                        {
                            isBlockPlacedImpossible = false;
                        }
                    }

                    if (isBlockPlacedImpossible == false)
                        return false;
                }
            }
        }

        if (isBlockPlacedImpossible)
        {
            GameOver();
            return true;
        }

        return false;
    }

    private void GameOver()
    {
        gameOverImg.SetActive(true);
    }

    private void BlockClear(List<CellInfo> blockList, bool isOver = false)
    {
        if (!isOver)
        {
            scoreInfo.score += 1000;
            scoreInfo.lineClearCount++;
        }

        foreach (var block in blockList)
        {
            block.isBlockPlaced = false;
            var target = block.cellCol.transform;
            // target.DORotate(360 * Vector3.forward, 0.4f);
            // target.DOScale(0, 0.4f).onComplete = () =>
            // {
            target.GetComponent<SpriteRenderer>().color = Color.white;
            //     target.DOScale(0.5f, 0.1f);
            // };
        }
    }

    private void CurBlockReset()
    {
        nowPickBlock.transform.localPosition = Vector3.zero;
        nowPickBlock.transform.localScale = Vector3.one;
        nowPickBlock = null;
    }

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
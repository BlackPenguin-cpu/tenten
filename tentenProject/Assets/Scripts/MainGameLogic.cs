using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

[System.Serializable]
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

    private int blockDropCount;

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

    private void Start()
    {
        GameReset();
    }

    private void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (Input.GetMouseButtonDown(0))
            BlockPicking();
        if (nowPickBlock != null)
        {
            if (Input.GetMouseButtonUp(0))
                TryBlockDrop();
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

    private void TryBlockDrop()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TryBlockDrop(pos);
    }

    public bool TryBlockDrop(Vector2Int pos)
    {
        return TryBlockDrop(TileMapManager.ChangePosToTilePos(pos));
    }

    public bool TryBlockDrop(Vector2 pos)
    {
        var col = Physics2D.Raycast(pos, Vector3.forward, 100, 1 << LayerMask.NameToLayer("Tilemap"));

        if (nowPickBlock == null) return false;
        if (col.collider == null)
        {
            nowPickBlock?.transform.DOKill();
            CurBlockReset();
            return false;
        }

        var vec = TileMapManager.ChangeTilePosToPos(col.transform.position);
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

        foreach (var cell in CellInfos)
        {
            if (cell.isBlockPlaced == false) scoreInfo.score += 5;
        }

        BlockManager.instance.ingameCellBlocks.Remove(nowPickBlock);
        Destroy(nowPickBlock.gameObject);
        nowPickBlock = null;
        scoreInfo.blockPlaceCount++;

        if (BlockManager.instance.ingameCellBlocks.Count <= 0)
            BlockManager.instance.BlockRefill();

        BlockClearCheck();
        IsFail();

        UIManager.instance.InfoApply(scoreInfo);

        blockDropCount++;
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

            var clearBlockList = new List<CellInfo>();

            if (horClearBlockList.Count >= 10)
                clearBlockList.AddRange(horClearBlockList);

            if (verClearBlockList.Count >= 10)
                clearBlockList.AddRange(verClearBlockList);

            if (clearBlockList.Count > 0)
            {
                BlockClear(clearBlockList);
                BlockClearCheck();
            }

            horClearBlockList.Clear();
            verClearBlockList.Clear();
        }
    }

    public bool IsFail()
    {
        var blockList = BlockManager.instance.ingameCellBlocks;
        bool isBlockPlacedImpossible = false;
        if (blockDropCount >= 100)
            return true;

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
            scoreInfo.score += 10000;
            scoreInfo.lineClearCount++;
        }

        StartCoroutine(LineClearCoroutine(blockList));
        // foreach (var block in blockList)
        // {
        //     block.isBlockPlaced = false;
        //     var target = block.cellCol.transform;
        //      target.DORotate(360 * Vector3.forward, 0.4f);
        //      target.DOScale(0, 0.4f).onComplete = () =>
        //      {
        //     target.GetComponent<SpriteRenderer>().color = Color.white;
        //          target.DOScale(0.5f, 0.1f);
        //      };
        // }
    }

    private IEnumerator LineClearCoroutine(List<CellInfo> blockList)
    {
        foreach (var block in blockList)
        {
            block.isBlockPlaced = false;
            var target = block.cellCol.transform;
            target.DORotate(360 * Vector3.forward, 0.4f).Delay();
            target.DOScale(0, 0.4f).onComplete = () =>
            {
                target.GetComponent<SpriteRenderer>().color = Color.white;
                target.DOScale(0.5f, 0.1f);
            };
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void CurBlockReset()
    {
        nowPickBlock.transform.localPosition = Vector3.zero;
        nowPickBlock.transform.localScale = Vector3.one;
        nowPickBlock = null;
    }

    public void GameReset()
    {
        blockDropCount = 0;
        //Tilemap CellInfo Reset
        foreach (var tile in tileMapManager.cellList)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }

        CellInfos = null;

        //ScoreInfo Reset
        scoreInfo = new ScoreInfo();

        //ingameCellBlock Reset    
        BlockManager.instance.blockQueue.Clear();
        foreach (var obj in BlockManager.instance.ingameCellBlocks)
        {
            Destroy(obj.gameObject);
        }

        BlockManager.instance.ingameCellBlocks.Clear();
        BlockManager.instance.BlockRefill();

        //GameOver UI Active false
        gameOverImg.SetActive(false);
    }
}
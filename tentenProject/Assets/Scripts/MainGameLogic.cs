using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public struct ScoreInfo
{
    public BigInteger score;
    public int lineClearCount;
    public int cellPlaceCount;
    public int blockPlaceCount;
}

public class MainGameLogic : MonoBehaviour
{
    public static MainGameLogic instance;
    [SerializeField] private GameObject tileMap;
    [SerializeField] private Sprite normalBlockSprite;
    [SerializeField] private TextMeshPro multiplyText;
    [SerializeField] private Button nextStageButton;

    private CellInfo[,] cellInfos;

    public CellInfo[,] CellInfos
    {
        get
        {
            if (cellInfos != null) return cellInfos;

            cellInfos = new CellInfo[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    cellInfos[i, j] = new CellInfo
                    {
                        cellInfo = tileMapManager.cellList[i + j * 10]
                    };
                }
            }

            return cellInfos;
        }
        private set => cellInfos = value;
    }

    public TileMapManager tileMapManager;
    public ScoreInfo scoreInfo;

    private int blockDropCount;

    public class CellInfo
    {
        public Block cellInfo;
        public bool isBlockPlaced = false;
    }

    public CellBlock NowPickBlock { get; private set; }

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
        if (NowPickBlock != null)
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
        NowPickBlock.transform.position = pos;
    }

    private void BlockPicking()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var col = Physics2D.Raycast(pos, Vector3.forward, 100, 1 << LayerMask.NameToLayer("CellBlock"));

        if (!col.collider || !col.collider.GetComponentInParent<CellBlock>()) return;
        PickBlockSet(col.collider.GetComponentInParent<CellBlock>());
        NowPickBlock.transform.DOScale(2, 0.1f).SetEase(Ease.InOutBounce);
        foreach (var sr in NowPickBlock.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingOrder = 1;
        }
    }

    public void PickBlockSet(CellBlock block)
    {
        NowPickBlock = block;
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

        if (NowPickBlock == null) return false;
        if (col.collider == null)
        {
            NowPickBlock?.transform.DOKill();
            ResetCurPickBlock();
            return false;
        }

        var vec = TileMapManager.ChangeTilePosToPos(col.transform.position);
        var blockVecList = NowPickBlock.GetThisBlockState();
        List<CellInfo> infos = new List<CellInfo>();

        foreach (var blockVec in blockVecList)
        {
            var targetPos = vec + blockVec;

            if (targetPos.x >= 10 || targetPos.y >= 10 || targetPos.x < 0 || targetPos.y < 0
                || CellInfos[targetPos.x, targetPos.y].isBlockPlaced == true)
            {
                ResetCurPickBlock();
                return false;
            }

            infos.Add(CellInfos[targetPos.x, targetPos.y]);
        }

        int cellNum = 0;
        foreach (var info in infos)
        {
            info.isBlockPlaced = true;
            scoreInfo.score += 10;
            scoreInfo.cellPlaceCount++;
            var curCell = NowPickBlock.cells[cellNum];
            curCell.transform.Rotate(0, 0, NowPickBlock.blockInfo.rotNum * 90);
            if (curCell.TryGetComponent(out Block block))
            {
                info.cellInfo.Behaviour = block.Behaviour;
            }

            info.cellInfo.GetComponent<SpriteRenderer>().color = curCell.GetComponent<SpriteRenderer>().color;
            info.cellInfo.GetComponent<SpriteRenderer>().sprite = curCell.GetComponent<SpriteRenderer>().sprite;

            info.cellInfo.OnPlace();
            cellNum++;
        }

        BlockManager.instance.ingameCellBlocks.Remove(NowPickBlock);
        Destroy(NowPickBlock.gameObject);
        NowPickBlock = null;
        scoreInfo.blockPlaceCount++;

        if (BlockManager.instance.ingameCellBlocks.Count <= 0)
            BlockManager.instance.BlockRefill();

        BlockClearCheck();

        UIManager.instance.InfoApply(scoreInfo);

        blockDropCount++;
        tileMapManager.OnTurnPassAction();
        return true;
    }

    private async UniTaskVoid BlockClearCheck()
    {
        var horClearBlockList = new List<CellInfo>();
        var verClearBlockList = new List<CellInfo>();
        var clearBlockList = new List<CellInfo>();
        for (int j = 0; j < 10; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                if (CellInfos[j, i].isBlockPlaced)
                {
                    horClearBlockList.Add(CellInfos[j, i]);
                }

                if (CellInfos[i, j].isBlockPlaced)
                {
                    verClearBlockList.Add(CellInfos[i, j]);
                }
            }


            if (horClearBlockList.Count >= 10)
                clearBlockList.AddRange(horClearBlockList);

            if (verClearBlockList.Count >= 10)
                clearBlockList.AddRange(verClearBlockList);


            horClearBlockList.Clear();
            verClearBlockList.Clear();
        }

        if (clearBlockList.Count > 0)
        {
            await BlockClear(clearBlockList);
            BlockClearCheck();
        }

        IsFail();
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

        if (isBlockPlacedImpossible ||
            StageManager.instance.GetCurStageData().canPlaceBlock < blockDropCount)
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

    public async UniTask BlockClear(List<Vector2Int> posList, float multiplier = 1.0f)
    {
        var cellList = new List<CellInfo>();
        foreach (var pos in posList)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x >= 10 || pos.y >= 10) continue;
            cellList.Add(CellInfos[pos.x, pos.y]);
        }

        await BlockClear(cellList, true, multiplier);
    }

    private async UniTask BlockClear(List<CellInfo> blockList, bool isNonDelay = false, float multiplier = 1.0f)
    {
        foreach (var block in blockList)
        {
            if (!block.cellInfo) continue;
            var target = block.cellInfo.transform;
            var curMultiplier = block.cellInfo.GetScoreMultiply();

            target.GetComponent<SpriteRenderer>().sprite = normalBlockSprite;
            ClearBlockAnim(target, 0.2f);

            if (!block.isBlockPlaced) continue;

            multiplier *= curMultiplier;
            scoreInfo.score += BigInteger.Parse($"{block.cellInfo.GetBaseScore() * multiplier}");
            if (Mathf.RoundToInt(curMultiplier) > 1 && multiplier > 1)
            {
                MultiplyTextCreate(target.transform, multiplier);
                if (!isNonDelay)
                    await Task.Delay(200);
                UIManager.instance.InfoApply(scoreInfo);
            }

            if (!isNonDelay)
                await Task.Delay(10);
            await block.cellInfo.OnClearBlock();
            block.isBlockPlaced = false;
            block.cellInfo.Behaviour = null;
        }

        var curStageData = StageManager.instance.GetCurStageData();
        if (curStageData.targetScore <= scoreInfo.score &&
            curStageData.canPlaceBlock >= blockDropCount)
        {
            //다음 스테이지로
            nextStageButton.gameObject.SetActive(true);
        }

        UIManager.instance.InfoApply(scoreInfo);
    }

    private void ClearBlockAnim(Transform target, float duration)
    {
        target.DORotate(180 * Vector3.forward, duration / 2).onComplete = () =>
        {
            target.DORotate(360 * Vector3.forward, duration / 2).onComplete = () =>
                target.Rotate(0, 0, 0);
        };
        target.DOScale(0, duration / 2).onComplete = () =>
        {
            target.GetComponent<SpriteRenderer>().color = Color.white;
            target.DOScale(0.5f, duration / 4);
        };
    }

    private void MultiplyTextCreate(Transform pos, float multiplier)
    {
        var text = Instantiate(multiplyText, pos.position, Quaternion.identity);
        text.text = $" {Mathf.Round(multiplier * 10) / 10}x";
    }

    private void ResetCurPickBlock()
    {
        NowPickBlock.transform.localPosition = Vector3.zero;
        NowPickBlock.transform.localScale = Vector3.one;
        NowPickBlock = null;
    }

    public void GameReset()
    {
        blockDropCount = 0;
        //Tilemap CellInfo Reset
        foreach (var cell in CellInfos)
        {
            cell.isBlockPlaced = false;
            cell.cellInfo.GetComponent<SpriteRenderer>().color = Color.white;
            cell.cellInfo.GetComponent<SpriteRenderer>().sprite = normalBlockSprite;
            cell.cellInfo.OnClearBlock();
            cell.cellInfo.Behaviour = null;
        }

        CellInfos = null;

        //ScoreInfo Reset
        scoreInfo = new ScoreInfo();
        UIManager.instance.InfoApply(scoreInfo);

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
        nextStageButton.gameObject.SetActive(false);
    }
}
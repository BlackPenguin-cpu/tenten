using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI lineClearCountText;
    public TextMeshProUGUI blockPlaceCountText;
    public TextMeshProUGUI cellPlaceCountText;

    private const string scoreTextOriginal = "Score: {0}";
    private const string lineClearCountTextOriginal = "LineClearCount: {0}";
    private const string blockPlaceCountTextOriginal = "BlockPlaceCount: {0}";
    private const string cellPlaceCountTextOriginal = "CellPlaceCount: {0}";

    private void Awake()
    {
        instance = this;
    }

    public void InfoApply(ScoreInfo scoreInfo)
    {
        scoreText.text = string.Format(scoreTextOriginal, scoreInfo.score);
        lineClearCountText.text = string.Format(lineClearCountTextOriginal, scoreInfo.lineClearCount);
        blockPlaceCountText.text = string.Format(blockPlaceCountTextOriginal, scoreInfo.blockPlaceCount);
        cellPlaceCountText.text = string.Format(cellPlaceCountTextOriginal, scoreInfo.cellPlaceCount);
    }
}
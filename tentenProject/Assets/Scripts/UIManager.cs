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

    private const string scoreTextOriginal = "Score:";
    private const string lineClearCountTextOriginal = "LineClearCount:";
    private const string blockPlaceCountTextOriginal = "BlockPlaceCount:";
    private const string cellPlaceCountTextOriginal = "CellPlaceCount:";

    private void Awake()
    {
        instance = this;
    }

    public void InfoApply(ScoreInfo scoreInfo)
    {
        scoreText.text = $"{scoreTextOriginal} {scoreInfo.score}";
        lineClearCountText.text = $"{lineClearCountTextOriginal} {scoreInfo.lineClearCount}";
        blockPlaceCountText.text = $"{blockPlaceCountTextOriginal} {scoreInfo.blockPlaceCount}";
        cellPlaceCountText.text = $"{cellPlaceCountTextOriginal} {scoreInfo.cellPlaceCount}";
    }
}